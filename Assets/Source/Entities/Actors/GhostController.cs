using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors {
    public class GhostController : Actor {
        //TODO: refactor/organize this class
        
        //TODO: add scatter state
        
        //TODO: add pellet threshold
        

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection => currentDirection * -1;

        [Header("Physics Parameters")]
        
        protected const float INKY_BLINKY_PINKY_MOVE_SPEED = 3.5f;
        protected float standardMoveSpeed = 4.5f;
        private float _slowMoveSpeed;
        private const float RETREAT_MOVE_SPEED = 8f;



        [Header("Map Parameters")]
        
        
        private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
        [SerializeField]
        private Transform hubMarker;
        private Vector3 _hubPosition;

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

        [Header("Pathfinding Parameters")]

        [SerializeField]
        private Transform initialTargetTransform;
        private Vector2Int _initialTargetPosition;
        
        [SerializeField]
        private Transform scatterTarget;
        private Vector2Int _scatterTargetPosition;
        
        private enum Quadrant
       {
           UpRight, UpLeft, DownLeft, DownRight
       }
        
        [Header("Pathfinding State")]
        
        private bool _isInIntersection;
        private bool _pathResolved;
        
        private readonly int[] _neighborHeuristics = {
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX,
            DEFAULT_DISTANCE_MAX};
        
        protected Vector2Int currentTargetPosition;
        
        
        [Header("State Management")]
        
        private Timer _initialTargetChaseTime;
        private Timer _chasePollTimer;
        
        private Timer _chaseDurationTimer;
        private Timer _scatterDurationTimer;

        public enum State {
            Chase, Fleeing, Eaten,
            Scatter
        }
        public State state = State.Scatter;
        public State previousState = State.Chase;

        [Header("State Parameters")]
       
        protected int pelletThreshold = 0;
        private const float CHASE_DURATION_SECONDS = 20f; //original: 20f
        private const float SCATTER_DURATION_SECONDS = 7f;
        
        
        [Header("Dependencies")]
        
        //Other GameObjects/Components
        
        private int _inactiveLayer;
        private int _defaultLayer;
        private Transform _playerTransform;
        protected PlayerController playerController;
        
        //Components
        
        [Header("Animation Parameters")]
        
        [SerializeField]
        private RuntimeAnimatorController eatenAnimController;
        private RuntimeAnimatorController _standardAnimController;
        private static readonly int FleeingAnimator = Animator.StringToHash("Fleeing");
        
        [Header("Debug Parameters")]
        [SerializeField]
        private Color debugColor;
        
        [Header("Utilities")]
        private static readonly Func<bool,bool> IsElementTrue = element => element;
        

        public override void Initialize() {
           base.Initialize();

           ResolveDependencies();
            
            SubscribeToEvents();
            
            TargetInitialPosition();
            
            currentMoveSpeed = standardMoveSpeed;
            _slowMoveSpeed = standardMoveSpeed/2f;
           
            _standardAnimController = animator.runtimeAnimatorController;
        }
       private void ResolveDependencies()
       {
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           _playerTransform = SessionManagerSingle.Instance.player.transform;
           playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
           _chasePollTimer = new Timer(PlayerController.PLAYER_STEP_TIME, false, false); //old: chasePollSeconds as waitTime
           _initialTargetChaseTime = new Timer(8f, true, true);
           _chaseDurationTimer = new Timer(CHASE_DURATION_SECONDS, false, true);
           _scatterDurationTimer = new Timer(SCATTER_DURATION_SECONDS, false, true);
           ResolveMapMarkers();
       }
       private void ResolveMapMarkers()
       {
           _topLeftMapBound = topLeft.position;
           _topRightMapBound = topRight.position;
           bottomLeftMapBound = bottomLeft.position;
           _bottomRightMapBound = bottomRight.position;
           _hubPosition = hubMarker.position;
           _scatterTargetPosition = LevelGridController.VectorToVector2Int(scatterTarget.position);
           
           _initialTargetPosition = LevelGridController.VectorToVector2Int(initialTargetTransform.position);
       }
       private void SubscribeToEvents()
       {
           // _retreatTimer.OnEnd += ResetPositionAndState;
           _initialTargetChaseTime.OnEnd += StartChasePollTimer;
           _chasePollTimer.OnEnd += PollChasePosition;
           PlayerController.OnInvincibleChanged += UpdateState;
           SessionManagerSingle.OnReset += ResetActor;
           PelletController.OnPelletEaten += PollThreshold;

           _chaseDurationTimer.OnEnd += SetStateToScatter;
           _scatterDurationTimer.OnEnd += SetStateToChase;
       }

       private void SetStateToChase()
       {
           if (SessionManagerSingle.Instance.playerController.isInvincible && !IsCenteredAt(_hubPosition)) return;
           SetState(State.Chase);
       }

       private void SetStateToScatter()
       {
           if (state != State.Chase) return;
           SetState(State.Scatter);
       }

       private void TargetInitialPosition()
       {
           currentTargetPosition = LevelGridController.VectorToVector2Int(initialTargetTransform.position);
       }
       private void PollThreshold()
       {
           if (!IsCenteredAt(_hubPosition) || SessionDataModel.Instance.pelletsEaten != pelletThreshold) return;
           if (_chasePollTimer.Active) return;
           Debug.Log("Starting Chase Poll Timer!");
           StartChasePollTimer();
       }
       private void StartChasePollTimer()
       {
           _chasePollTimer.Start();
       }
       private void StopChasePollTimer()
       {
           _chasePollTimer.Stop();
       }
       
       protected virtual void PollChasePosition() => currentTargetPosition = playerController.gridPosition;
       
        protected override void Unfreeze()
        {
            base.Unfreeze();
            if (currentTargetPosition
                != _initialTargetPosition && state == State.Chase)
            {
                StartChasePollTimer();
            }
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
        }
        
       private void UpdateState(bool isInvincible)
       {
           if (state == State.Eaten) return;
           SetState(isInvincible ? State.Fleeing : State.Chase);
       }
       
       
        private const int DEFAULT_DISTANCE_MAX = 999;


        protected override void FixedUpdate() {
            if (!thisRigidbody.simulated) return;
           UpdateGridPosition();
           UpdatePossibleTurns();
           _possibleTurnsTotal = GetTrueCount(possibleTurns);

           bool isInTileCenter = IsInTileCenter;
           
           if (!isInTileCenter && _pathResolved) _pathResolved = false;

           
           _isInIntersection = _possibleTurnsTotal > 2;
           
           if (isInTileCenter && !_pathResolved)
           {
               ResolvePath();
           }

           CheckIfCameToHub();
           
           UpdateMotion(new Vector2(currentDirection.x, currentDirection.y) * currentMoveSpeed);
        }

        private void CheckIfCameToHub()
        {
            if (state == State.Eaten && IsCenteredAt(_hubPosition))
            {
                SetStateToChase();
            }
        }


        private void UpdatePossibleTurns()
        {
            LevelGridController.Instance.CheckPossibleTurns(gridPosition, possibleTurns);
        }

        private void ResolvePath()
        {
            if (_isInIntersection)
            {
                currentDirection = DirectionToVector2Int(
                    GetBestTurn(gridPosition,
                        currentTargetPosition,
                        possibleTurns,
                        (Direction) VectorToInt(OriginDirection)));
                _pathResolved = true;
            }
            else
            {
                FollowPath();
                _pathResolved = true;
            }
        }
        
        
        private Direction GetBestTurn(Vector2Int initialPosition,
                                    Vector2Int goalPosition,
                                    bool[] viableTurns,
                                    Direction originDirection)
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
                        _neighborHeuristics[i] = LevelGridController.TaxiCabDistance(
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
        
        private bool IsCenteredAt(Vector3 position)
        {
            return LevelGridController.VectorApproximately(position, thisTransform.position, 0.1f);
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
          
          


          private static int GetTrueCount(IEnumerable<bool> boolArray) => boolArray.Count(IsElementTrue);

          private void OnCollisionEnter2D(Collision2D other) {
           if (!other.gameObject.CompareTag("Player")) return;
           switch (state) {
               case State.Fleeing:
                   SetState(State.Eaten);
                   animator.runtimeAnimatorController = eatenAnimController;
                   AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.EatGhost, 1, false);
                   
                   SessionManagerSingle.Instance. freezeTimer.Start();
                   SessionManagerSingle.Instance.Freeze();
                   
                   break;
               case State.Chase:
                   SessionManagerSingle.Instance.Die();
                   break;
               case State.Eaten:
                   break;
               case State.Scatter:
                   SessionManagerSingle.Instance.Die();
                   break;
               default:
                   return;
           }
          }

          private void SetTargetAwayFromPlayer() {
           Quadrant playerQuadrant = GetQuadrant(_playerTransform.position, _mapCentralPosition);
           Vector3 finalPosition = playerQuadrant switch {
               Quadrant.UpRight => bottomLeftMapBound,
               Quadrant.UpLeft => _bottomRightMapBound,
               Quadrant.DownLeft => _topRightMapBound,
               Quadrant.DownRight => _topLeftMapBound,
               _ => bottomLeftMapBound
           };
                currentTargetPosition = LevelGridController.VectorToVector2Int(finalPosition);
       }

       private static Quadrant GetQuadrant(Vector2 position, Vector2 centralPosition) {
           bool isUp = position.y >= centralPosition.y;
           bool isRight = position.x >= centralPosition.x;
           if (isUp) {
               return isRight ? Quadrant.UpRight : Quadrant.UpLeft;
           }

           return isRight ? Quadrant.DownRight : Quadrant.DownLeft;
       }

       private void SetState(State targetState)
       {
           if (state == targetState) return;
           previousState = state;
           state = targetState;
           // OnStateExit();
           OnStateEntered();
       }

       private void OnStateExit()
       {
           throw new NotImplementedException();
       }

       private void OnStateEntered() {
           switch (state)
           {
               case State.Chase: {
                   EnableCollisionsWithPlayer();
                   StartChasePollTimer();
                   StartChaseDuration();
                   animator.SetBool(FleeingAnimator, false);
                   if (animator.runtimeAnimatorController != _standardAnimController)
                   {
                       animator.runtimeAnimatorController = _standardAnimController;
                   }
                   if (playerController.isInvincible)
                   {
                       AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
                   }
                   break;
               }
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   StopChasePollTimer();
                   animator.SetBool(FleeingAnimator, true);
                   break;
               }
               case State.Eaten: {
                   thisGameObject.layer = _inactiveLayer;
                   currentMoveSpeed = RETREAT_MOVE_SPEED;
                   currentTargetPosition = LevelGridController.VectorToVector2Int(_hubPosition);
                   StopChasePollTimer();
                   animator.runtimeAnimatorController = eatenAnimController;

                   break;
               }
               case State.Scatter:
                   _scatterDurationTimer.Start();
                   _chasePollTimer.Stop();
                   currentTargetPosition = _scatterTargetPosition;
                   break;
               default: {
                   return;
               }
           }
       }

       private void StartChaseDuration()
       {
           _chaseDurationTimer.Start();
       }

       private void EnableCollisionsWithPlayer()
       {
           thisGameObject.layer = _defaultLayer;
           currentMoveSpeed = standardMoveSpeed;
       }

       private static int FindSmallestNumberIndex(int[] array) {
            int currentSmallest = array[0];
            for (int i = 1; i < array.Length; i++) {
                if (currentSmallest > array[i])
                    currentSmallest = array[i];
            }

            return Array.IndexOf(array, currentSmallest);
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = debugColor;
            Gizmos.DrawSphere(LevelGridController.Vector2ToVector3(currentTargetPosition), 0.3f);
        }

        protected override void ResetActor()
        {
            SetStateToChase();
            TargetInitialPosition();
        }

        protected override void Freeze()
        {
            base.Freeze();
            StopChasePollTimer();
            if (state == State.Eaten) animator.enabled = true;
        }
    }
}