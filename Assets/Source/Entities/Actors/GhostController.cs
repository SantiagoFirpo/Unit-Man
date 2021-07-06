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
        protected Vector3 hubPosition;

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
        private Transform hubExitMarker;
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
        
        private Timer _hubExitTimer;
        private Timer _chasePollStepTimer;
        
        private Timer _chaseDurationTimer;
        private Timer _scatterDurationTimer;

        public enum State {
            Resting, ExitingHub,
            Chase, Scatter, Fleeing, Eaten,
            
        }

        public State state; //DO NOT ASSIGN DIRECTLY, USE SetState(State.TargetState)
        public State previousState;

        [Header("State Parameters")]
       
        protected int pelletThreshold = -1;
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

        private readonly Timer _fleeingDurationTimer = new Timer(FLEEING_TIME_SECONDS, false, false);
        private static readonly int OnNearEndAnimTrigger = Animator.StringToHash("OnNearEnd");
        private static readonly int OnFleeEndAnimTrigger = Animator.StringToHash("OnFleeEnd");

        [SerializeField]
        private Transform restingMarker;
        private Vector3 _restingTarget;
        private static int _eatenGhostsTotal;
        private static int _fleeingGhostsTotal;

        private const float FLEEING_TIME_SECONDS = 10f;


        public override void Initialize() {
           base.Initialize();

           ResolveDependencies();
            
            SubscribeToEvents();

            currentMoveSpeed = standardMoveSpeed;
            _slowMoveSpeed = standardMoveSpeed/2f;
            pelletThreshold = 1;
           
            _standardAnimController = animator.runtimeAnimatorController;
        }
       private void ResolveDependencies()
       {
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           _playerTransform = SessionManagerSingle.Instance.player.transform;
           playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
           _chasePollStepTimer = new Timer(PlayerController.PLAYER_STEP_TIME, false, false); //old: chasePollSeconds as waitTime
           _hubExitTimer = new Timer(6f, false, true);
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
           hubPosition = hubMarker.position;
           _restingTarget = restingMarker.position;
           
           
           _scatterTargetPosition = LevelGridController.VectorToVector2Int(scatterTarget.position);
           
           _initialTargetPosition = LevelGridController.VectorToVector2Int(hubExitMarker.position);
       }
       private void SubscribeToEvents()
       {
           // _retreatTimer.OnEnd += ResetPositionAndState;
           _hubExitTimer.OnEnd += SetStateToChase;
           _chasePollStepTimer.OnEnd += PollChaseTarget;
           _fleeingDurationTimer.OnEnd += SetStateToChase;
           PowerPelletController.OnPowerPelletCollected += SetStateToFleeing;
           SessionManagerSingle.OnReset += ResetActor;
           PelletController.OnPelletEaten += PollThreshold;

           _chaseDurationTimer.OnEnd += SetStateToScatter;
           _scatterDurationTimer.OnEnd += SetStateToChase;
       }

       private void SetStateToChase()
       {
           SetState(State.Chase);
       }

       private void SetStateToScatter()
       {
           if (state != State.Chase) return;
           SetState(State.Scatter);
       }

       private void TargetHubExit()
       {
           currentTargetPosition = LevelGridController.VectorToVector2Int(hubExitMarker.position);
       }
       private void PollThreshold()
       {
           if (SessionDataModel.Instance.pelletsEaten >= pelletThreshold && state == State.Resting)
               SetState(State.ExitingHub);
       }
       private void StartChasePollStepTimer()
       {
           _chasePollStepTimer.Start();
       }
       private void StopChasePollTimer()
       {
           _chasePollStepTimer.Stop();
       }
       
       protected virtual void PollChaseTarget() => currentTargetPosition = playerController.gridPosition;
       
        protected override void Unfreeze()
        {
            base.Unfreeze();
            if (currentTargetPosition
                != _initialTargetPosition && state == State.Chase
                                          && pelletThreshold >= SessionDataModel.Instance.pelletsEaten)
            {
                StartChasePollStepTimer();
            }
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
        }
        
       private void SetStateToFleeing()
       {
           if (state == State.Eaten) return;
           SetState(State.Fleeing);
           _fleeingDurationTimer.Start();
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

           StateStep();

           UpdateMotion(new Vector2(currentDirection.x, currentDirection.y) * currentMoveSpeed);
        }

        private void StateStep()
        {
            switch (state)
            {
                case State.ExitingHub:
                    if (IsCenteredAt(hubExitMarker.position))
                    {
                        SetState(State.Chase);
                    }
                    break;
                case State.Fleeing:
                    if (_fleeingDurationTimer.currentTime > 0.7f * FLEEING_TIME_SECONDS)
                    {
                        animator.SetTrigger(OnNearEndAnimTrigger);
                    }
                    break;
                case State.Eaten:
                    if (IsCenteredAt(hubPosition))
                    {
                        SetStateToChase();
                    }
                    break;
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
                if (state == State.Resting)
                {
                    possibleTurns[(int) Direction.Up] = false;
                }
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

          private void OnCollisionEnter2D(Collision2D other)
          {
              if (!other.gameObject.CompareTag("Player")) return;
              if (state == State.Fleeing)
                  SetState(State.Eaten);
              else
                  SessionManagerSingle.Instance.Die();
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

       protected void SetState(State targetState)
       {
           if (state == targetState) return;
           previousState = state;
           state = targetState;
           OnStateExit();
           OnStateEntered();
       }

       private void OnStateExit()
       {
           switch (previousState)
           {
               case State.Resting:
                   PelletController.OnPelletEaten -= PollThreshold;
                   break;
               case State.ExitingHub:
                   _hubExitTimer.Stop();
                   break;
               case State.Chase:
                   StopChasePollTimer();
                   break;
               case State.Scatter:
                   _scatterDurationTimer.Stop();
                   break;
               case State.Fleeing:
                   _fleeingDurationTimer.Stop();
                   currentMoveSpeed = standardMoveSpeed;
                   animator.SetTrigger(OnFleeEndAnimTrigger);

                   _fleeingGhostsTotal--;
                   break;
               case State.Eaten:
                   EnableCollisionsWithPlayer();
                   currentMoveSpeed = standardMoveSpeed;
                   animator.runtimeAnimatorController = _standardAnimController;
                   _eatenGhostsTotal--;
                   if (_eatenGhostsTotal == 0)
                   {
                       AudioManagerSingle.Instance.PlayClip(
                           _fleeingGhostsTotal == 0
                               ? AudioManagerSingle.AudioEffectType.Siren
                               : AudioManagerSingle.AudioEffectType.Fleeing, 1, true);
                   }
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }
       }

       private void OnStateEntered() {
           // Debug.Log($"Entered state {state}", thisGameObject);
           switch (state)
           {
               case State.Resting:
                   currentTargetPosition = LevelGridController.VectorToVector2Int(_restingTarget);
                   PelletController.OnPelletEaten += PollThreshold;
                   break;
               case State.ExitingHub:
                   TargetHubExit();
                   _hubExitTimer.Start();
                   break;
               case State.Chase: {

                   StartChasePollStepTimer();
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

               case State.Scatter:
               {
                   currentTargetPosition = _scatterTargetPosition;
                   _scatterDurationTimer.Start();
                   break;
               }
                   
               
               
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   animator.SetBool(FleeingAnimator, true);

                   _fleeingGhostsTotal++;
                   break;
               }
               case State.Eaten: {
                   thisGameObject.layer = _inactiveLayer;
                   currentMoveSpeed = RETREAT_MOVE_SPEED;
                   currentTargetPosition = LevelGridController.VectorToVector2Int(hubPosition);
                   animator.runtimeAnimatorController = eatenAnimController;
                   _eatenGhostsTotal++;
                   AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.EatGhost, 1, false);
                   
                   SessionManagerSingle.Instance. freezeTimer.Start();
                   SessionManagerSingle.Instance.Freeze();

                   break;
               }
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
            Gizmos.DrawSphere(new Vector3(currentTargetPosition.x, currentTargetPosition.y), 0.3f);
        }

        protected override void ResetActor()
        {
            SetState(State.ExitingHub);
            if (_possibleTurnsTotal == 1) FollowPath();
            _eatenGhostsTotal = 0;
        }

        protected override void Freeze()
        {
            base.Freeze();
            StopChasePollTimer();
            if (state == State.Eaten) animator.enabled = true;
        }
    }
}