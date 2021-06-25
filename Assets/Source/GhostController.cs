using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Timer = UnitMan.Source.Utilities.TimeTracking.Timer;

namespace UnitMan.Source {
    public class GhostController : Actor {
        //TODO: refactor/organize this class
        //TODO: use current behavior to change current target position
        
        private readonly Vector2 _zero = Vector2.zero;

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection => currentDirection * -1;

        [Header("Physics Parameters")]

        [SerializeField]
        protected float standardMoveSpeed;

        [Header("Pathfinding Parameters")]
        public static readonly Direction[] HorizontalDirections = {Direction.Left, Direction.Right};
        public static readonly Direction[] VerticalDirections = {Direction.Up, Direction.Down};

        [SerializeField]
        private Transform initialTargetTransform;

        [Header("Pathfinding Parameters")]
        
        [Header("State Management")]
        private Timer _playerPollDelay;
        private readonly Timer _retreatTimer = new Timer(MAX_RETREAT_SECONDS);
       
        [Header("Dependencies")]
        private int _inactiveLayer;
        private int _defaultLayer;

        [SerializeField]
        protected float pathfindingIntervalSeconds = 4f;

        [SerializeField]
        private Transform topLeft;
        private Vector3 _topLeftMapBound;

        [SerializeField]

        private Transform topRight;
        private Vector3 _topRightMapBound;

        [SerializeField]

        private Transform bottomLeft;
        protected Vector3 _bottomLeftMapBound;
        
        [SerializeField]
        private Transform bottomRight;
        private Vector3 _bottomRightMapBound;
        protected Transform playerTransform;


       protected PlayerController playerController;
       private float _currentMoveSpeed;

       private Vector2Int NextTile => gridPosition + currentDirection;
       private float _slowMoveSpeed;


       public enum State
       {
           Alive, Fleeing, Dead
       }

       private bool _isInIntersection;
       private bool _pathResolved;
       
       public State state = State.Alive;
       
       private const float MOVE_SPEED_INACTIVE = 6f;

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
       
       private static readonly int FleeingAnimator = Animator.StringToHash("Fleeing");
       protected Vector2Int currentTargetPosition;
        private const float MAX_RETREAT_SECONDS = 6f;
        
        protected const float CLYDE_MOVE_SPEED = 3.5f;

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
           _retreatTimer.OnEnd += ResetPositionAndState;
           _playerPollDelay.OnEnd += PollPlayerPosition;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void ResolveDependencies()
       {
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           playerTransform = GameManager.Instance.player.transform;
           playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _playerPollDelay = new Timer(pathfindingIntervalSeconds, 0f, true, false);
       }

       private void GetMapMarkers()
       {
           _topLeftMapBound = topLeft.position;
           _topRightMapBound = topRight.position;
           _bottomLeftMapBound = bottomLeft.position;
           _bottomRightMapBound = bottomRight.position;

           currentTargetPosition = PathGrid.VectorToVector2Int(initialTargetTransform.position);
       }

       protected virtual void PollChasePosition() {
           currentTargetPosition = playerController.gridPosition;
       }

       private void ResetPositionAndState() {
           SetState(State.Alive);
           AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Siren, 1, true);
       }

       private void UpdateState(bool isInvincible) {
           SetState(isInvincible ? State.Fleeing : State.Alive);
       }

       private void Start() {
           _playerPollDelay.Start();
           _currentMoveSpeed = standardMoveSpeed;
           _slowMoveSpeed = standardMoveSpeed/2f;
       }
       
        private const int DEFAULT_DISTANCE_MAX = 999;

        private readonly int[] _neighborHeuristics = new int[4] {
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX};
        private Direction GetBestTurn(Vector2Int initialPosition, Vector2Int goalPosition, bool[] viableTurns, Direction originDirection)
        {
            Direction bestTurn = Direction.Up;
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

            motion = (Vector2) currentDirection * _currentMoveSpeed;
           rigidBody.velocity = motion;
       }

      private void FollowPath() {
          Direction originDirection = (Direction) VectorToInt(OriginDirection);
          if (_possibleTurnsTotal == 1) {
              for (int i = 0; i < 4; i++)
              {
                  if (possibleTurns[i]) currentDirection = DirectionToVector2Int((Direction) i);
              }
              
          }
          else {
              for (int i = 0; i < 4; i++)
              {
                  if ((Direction) i == originDirection || !possibleTurns[i]) continue;

                  currentDirection = DirectionToVector2Int((Direction) i);
              }
          }
      }
      private static readonly Func<bool,bool> IsElementTrue = element => element;  
      
      private static int GetTrueCount(IEnumerable<bool> boolArray)
      {
          return boolArray.Count(IsElementTrue);
      }

        
    //    private void MoveThroughPath() {
    //        Vector2Int nextPosition = _positionQueue.Peek();
    //        Vector2Int actualDirection = nextPosition - gridPosition;
    //        // Debug.Log();
               
    //        currentDirection = !IsCardinalDirection(actualDirection) ? currentDirection : actualDirection;
    //        // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
    //        if (PathGrid.VectorApproximately(thisTransform.position, nextPosition, _currentMoveSpeed * SPEED_TOLERANCE_CONVERSION)) { // previous value: 0.05f
    //            _positionQueue.Dequeue();
    //        }
    //    }
       
    //    private void FollowPath() {
    //        Vector2Int nextPosition = _positionQueue.Peek();
    //        Vector2Int nextDirection = _directionQueue.Peek();
    //        if (possibleTurns[VectorToInt(nextDirection)]) {
    //            currentDirection = nextDirection;
    //        }
    //        // Debug.Log(_direction, thisGameObject);
    //        // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
    //        if (!PathGrid.VectorApproximately(thisTransform.position, nextPosition,
    //            _currentMoveSpeed * SPEED_TOLERANCE_CONVERSION)) return; // previous value: 0.05f
    //        _positionQueue.Dequeue();
    //        _directionQueue.Dequeue();
    //    }

       private void OnCollisionEnter2D(Collision2D other) {
           if (!other.gameObject.CompareTag("Player")) return;
           switch (state) {
               case State.Fleeing:
                   SetState(State.Dead);
                   GameManager.Instance.Freeze();
                   break;
               case State.Alive:
                   GameManager.Instance.Die();
                   break;
               case State.Dead:
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
               Quadrant.UpRight => _bottomLeftMapBound,
               Quadrant.UpLeft => _bottomRightMapBound,
               Quadrant.DownLeft => _topRightMapBound,
               Quadrant.DownRight => _topLeftMapBound,
               _ => _bottomLeftMapBound
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
                   _playerPollDelay.Start();
                   animator.SetBool(FleeingAnimator, false);

                   break;
               }
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   _playerPollDelay.Stop();
                   animator.SetBool(FleeingAnimator, true);
                   break;
               }
               case State.Dead: {
                   thisGameObject.layer = _inactiveLayer;
                   _currentMoveSpeed = MOVE_SPEED_INACTIVE;
                   currentTargetPosition = PathGrid.VectorToVector2Int(StartPosition);
                   _playerPollDelay.Stop();
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