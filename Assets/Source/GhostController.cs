using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public class GhostController : Actor {
        //TODO: refactor/organize this class
        
        //TODO: Blinky doesn't target initialTarget at start
        
        //TODO: add scatter state
        
        //TODO: wrong music playing

        public static readonly Direction[] HorizontalDirections = {Direction.Left, Direction.Right};
        public static readonly Direction[] VerticalDirections = {Direction.Up, Direction.Down};

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection => currentDirection * -1;

        [Header("Physics Parameters")]

        [SerializeField]
        protected float standardMoveSpeed;
        private float _slowMoveSpeed;

        [Header("Pathfinding State")]
        
        private bool _isInIntersection;
        private bool _pathResolved;


        [Header("Pathfinding Parameters")]
        
        [SerializeField]
        private Transform initialTargetTransform;
        
        [SerializeField]
        protected float chasePollSeconds = 4f;

        [SerializeField]
        private Transform topLeft;
        private Vector3 _topLeftMapBound;

        [SerializeField]

        private Transform topRight;
        private Vector3 _topRightMapBound;

        [SerializeField]

        private Transform bottomLeft;
        protected Vector3 bottomLeftMapBound;
        
        [SerializeField]
        private Transform bottomRight;
        private Vector3 _bottomRightMapBound;
        
        
        
        [Header("State Management")]

        private Timer _chasePollDelay;

        private float _currentMoveSpeed;
        
        public enum State {
            Alive, Fleeing, Eaten
        }
       
        [Header("Dependencies")]
        
        //Other GameObjects/Components
        
        private int _inactiveLayer;
        private int _defaultLayer;
        protected Transform playerTransform;
        protected PlayerController playerController;
        
        //Components
        
        [SerializeField]
        private RuntimeAnimatorController eatenController;
       
        private RuntimeAnimatorController _standardController;
        
       // private Vector2Int NextTile => gridPosition + currentDirection;


       
       public State state = State.Alive;
       
       private const float MOVE_SPEED_INACTIVE = 6f;

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
       
       private static readonly int FleeingAnimator = Animator.StringToHash("Fleeing");
       protected Vector2Int currentTargetPosition;

       protected const float CLYDE_MOVE_SPEED = 3.5f;
        
        private readonly int[] _neighborHeuristics = new int[] {
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX};

       public enum Quadrant
       {
           UpRight, UpLeft, DownLeft, DownRight
       }

       protected override void Awake() {
           base.Awake();

            GetMapMarkers();

            ResolveDependencies();
            
            SubscribeToEvents();
           
            _currentMoveSpeed = standardMoveSpeed;
       }

       private void SubscribeToEvents()
       {
           // _retreatTimer.OnEnd += ResetPositionAndState;
           _chasePollDelay.OnEnd += PollChasePosition;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void ResolveDependencies()
       {
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           playerTransform = GameManager.Instance.player.transform;
           playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _chasePollDelay = new Timer(0.2f, 0f, true, false); //old: chasePollSeconds as waitTime
       }

       private void GetMapMarkers()
       {
           _topLeftMapBound = topLeft.position;
           _topRightMapBound = topRight.position;
           bottomLeftMapBound = bottomLeft.position;
           _bottomRightMapBound = bottomRight.position;

           currentTargetPosition = PathGrid.VectorToVector2Int(initialTargetTransform.position);
       }

       protected virtual void PollChasePosition() => currentTargetPosition = playerController.gridPosition;

       private void UpdateState(bool isInvincible) {
           SetState(isInvincible ? State.Fleeing : State.Alive);
       }

       private void Start() {
           _chasePollDelay.Start();
           
           _slowMoveSpeed = standardMoveSpeed/2f;
           
           _standardController = animator.runtimeAnimatorController;
       }
       
        private const int DEFAULT_DISTANCE_MAX = 999;

        private Direction GetBestTurn(Vector2Int initialPosition, Vector2Int goalPosition, bool[] viableTurns, Direction originDirection)
        {
            Direction bestTurn;
            for (int i = 0; i < 4; i++)
            {
                _neighborHeuristics[i] = DEFAULT_DISTANCE_MAX;
            }

            void ThreadStart() {

                for (int i = 0; i < viableTurns.Length; i++)
                {
                    bool isDirectionValid = viableTurns[i]
                                            && (Direction) i != originDirection;
                                            // && DirectionToVector2Int(i) != currentDirection;
                    if (isDirectionValid) {
                        _neighborHeuristics[i] = PathGrid.TaxiCabDistance(
                                                                        initialPosition + DirectionToVector2Int(i),
                                                                        goalPosition);
                    }

                }

                bestTurn = (Direction) FindSmallestNumberIndex(_neighborHeuristics);

           }
           //Toggle these two blocks to toggle multithreading:
           
            ThreadStart();
           
           // Thread pathFindThread = new Thread(ThreadStart);
           // pathFindThread.Start();
           return bestTurn;
       }

        protected override void FixedUpdate() {
           // base.FixedUpdate();
           UpdateGridPosition();
           PathGrid.Instance.CheckPossibleTurns(gridPosition, possibleTurns);
           
           
           if (!rigidBody.simulated) return;
           _possibleTurnsTotal = GetTrueCount(possibleTurns);

           bool isInTileCenter = IsInTileCenter;
           
           if (!isInTileCenter && _pathResolved) _pathResolved = false;

           _isInIntersection = _possibleTurnsTotal > 2;
           if (isInTileCenter && !_pathResolved) {
                if (_isInIntersection) {
                    currentDirection  = DirectionToVector2Int(
                                        GetBestTurn(gridPosition,
                                                    currentTargetPosition,
                                                    possibleTurns,
                                                    (Direction) VectorToInt(OriginDirection)));
                    // Debug.Log(currentDirection);
                    _pathResolved = true;
                    
                }
                else {
                    FollowPath();
                    _pathResolved = true;
                }
                
           }

           if (state == State.Eaten && PathGrid.VectorApproximately(StartPosition, gridPosition, 0.1f))
           {
               SetState(State.Alive);
           }

           motion = (Vector2) currentDirection * _currentMoveSpeed;
           rigidBody.velocity = motion;
       }
        private void SetDirection(int directionNumber) => currentDirection = DirectionToVector2Int((Direction) directionNumber);
          private void FollowPath()
          {
              Direction originDirection = (Direction) VectorToInt(OriginDirection);
              for (int i = 0; i < 4; i++)
              {
                  if ((_possibleTurnsTotal > 1 && (Direction) i == originDirection) || !possibleTurns[i]) continue;
                  SetDirection(i);
                  return;
              }
          }
      
      
          private static readonly Func<bool,bool> IsElementTrue = element => element;
          
          private static int GetTrueCount(IEnumerable<bool> boolArray) => boolArray.Count(IsElementTrue);

          private void OnCollisionEnter2D(Collision2D other) {
           if (!other.gameObject.CompareTag("Player")) return;
           switch (state) {
               case State.Fleeing:
                   SetState(State.Eaten);
                   GameManager.Instance.Freeze();
                   break;
               case State.Alive:
                   GameManager.Instance.Die();
                   break;
               case State.Eaten:
                   break;
               default:
                   return;
           }
           // thisTransform.position = startPosition;
       }


       //    private void ComputePathToPlayer() {
    //        MultithreadedPath(thisTransform.position, _playerTransform.position);
    //        RemoveFirstPosition();
    //    }
       
    //    private void ComputePathToHub() {
    //        MultithreadedPath(thisTransform.position, _hubPosition);
    //        RemoveFirstPosition();
    //    }
       
       private void SetTargetAwayFromPlayer() {
           Quadrant playerQuadrant = GetQuadrant(playerTransform.position, _mapCentralPosition);
           Vector3 finalPosition = playerQuadrant switch {
               Quadrant.UpRight => bottomLeftMapBound,
               Quadrant.UpLeft => _bottomRightMapBound,
               Quadrant.DownLeft => _topRightMapBound,
               Quadrant.DownRight => _topLeftMapBound,
               _ => bottomLeftMapBound
           };
                currentTargetPosition = PathGrid.VectorToVector2Int(finalPosition);
       }

       private static Quadrant GetQuadrant(Vector2 position, Vector2 centralPosition) {
           bool isUp = position.y >= centralPosition.y;
           bool isRight = position.x >= centralPosition.x;
           if (isUp) {
               return isRight ? Quadrant.UpRight : Quadrant.UpLeft;
           }

           return isRight ? Quadrant.DownRight : Quadrant.DownLeft;
       }

       private void SetState(State targetState) {
           state = targetState;
           OnStateEntered();
       }

       private void OnStateEntered() {
           switch (state)
           {
               case State.Alive: {
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = standardMoveSpeed;
                   _chasePollDelay.Start();
                   animator.SetBool(FleeingAnimator, false);
                   if (animator.runtimeAnimatorController != _standardController)
                   {
                       animator.runtimeAnimatorController = _standardController;
                   }
                   if (playerController.isInvincible)
                   {
                       AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Retreating, 1, true);
                   }
                   break;
               }
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   _chasePollDelay.Stop();
                   animator.SetBool(FleeingAnimator, true);
                   break;
               }
               case State.Eaten: {
                   thisGameObject.layer = _inactiveLayer;
                   _currentMoveSpeed = MOVE_SPEED_INACTIVE;
                   currentTargetPosition = PathGrid.VectorToVector2Int(StartPosition);
                   _chasePollDelay.Stop();
                   animator.runtimeAnimatorController = eatenController;
                   // ComputePathToHub();
                   // thisTransform.position = startPosition;
                   
                   break;
               }
               default: {
                   return;
               }
           }
       }

        private static int FindSmallestNumberIndex(int[] array) {
            int currentSmallest = array[0];
            for (int i = 1; i < array.Length; i++) {
                if (currentSmallest > array[i])
                    currentSmallest = array[i];
            }

            return Array.IndexOf(array, currentSmallest);
        }
    }
}