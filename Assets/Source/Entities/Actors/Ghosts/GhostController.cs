using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management.Audio;
using UnitMan.Source.Management.Session;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors.Ghosts {
    public class GhostController : Actor, IStateMachine<GhostController.GhostState> {
        // Power pellet animation and sound bugs


        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");
        

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection => currentDirection * -1;

        [Header("Physics Parameters")]
        
        protected const float PINKY_MOVE_SPEED = 3.5f;
        protected float standardMoveSpeed = 4.5f;
        private float _slowMoveSpeed;
        private const float RETREAT_MOVE_SPEED = 8f;



        [Header("Map Parameters")]
        private Vector2Int _hubPosition;
        
        private Vector2Int _topLeftMapBound;
        

        private Vector2Int _topRightMapBound;
        

        protected Vector2Int bottomLeftMapBound;
        
        private Vector2Int _bottomRightMapBound;

        [Header("Pathfinding Parameters")]
        
        private Vector2Int _hubExitTarget;
        
        protected Vector2Int scatterTargetPosition;
        
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
        
        private Timer _fleeingDurationTimer;

        private Observer _pelletEatenObserver;
        private Observer _powerPelletObserver;

        public enum GhostState {
            Resting, ExitingHub,
            Chase, Scatter, Fleeing, Eaten,
        }

        private GhostState _state; //DO NOT ASSIGN DIRECTLY, USE SetState(State.TargetState)
        public GhostState previousState;

        [Header("State Parameters")]
       
        protected int pelletThreshold = -1;
        private const float CHASE_DURATION_SECONDS = 20f; //original: 20f
        private const float SCATTER_DURATION_SECONDS = 7f;
        
        
        [Header("Dependencies")]
        
        //Other GameObjects/Components
        
        private int _inactiveLayer;
        private int _defaultLayer;
        protected PlayerController playerController;
        
        //Components

        [Header("Debug Parameters")]
        [SerializeField]
        private Color debugColor;
        
        [Header("Utilities")]
        private static readonly Func<bool,bool> IsElementTrue = element => element;


        private Vector2Int _restingTarget;
        private static readonly int PowerPelletCollectTrigger = Animator.StringToHash("OnPowerPelletCollect");
        private static readonly int OnFleeNearEndTrigger = Animator.StringToHash("OnFleeNearEnd");
        private static readonly int OnArrivedGhostHouseTrigger = Animator.StringToHash("OnArrivedGhostHouse");
        private static readonly int OnFleeEndTrigger = Animator.StringToHash("OnFleeEnd");
        private static readonly int OnEatenTrigger = Animator.StringToHash("OnEaten");


        protected override void ResolveDependencies() {
           base.ResolveDependencies();

           ResolveGhostDependencies();
            
            SubscribeToEvents();

            currentMoveSpeed = standardMoveSpeed;
            _slowMoveSpeed = standardMoveSpeed/2f;
            pelletThreshold = 1;
        }
       private void ResolveGhostDependencies()
       {
           CreateTimers();
           ResolveMapMarkers();
       }

       protected override void Initialize()
       {
           base.Initialize();
           playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           _pelletEatenObserver = new Observer(PollThreshold);
           _powerPelletObserver = new Observer(OnPowerPelletCollect);
       }

       private void CreateTimers()
       {
           _chasePollStepTimer =
               new Timer(PlayerController.PLAYER_STEP_TIME, false, false); //old: chasePollSeconds as waitTime
           _hubExitTimer = new Timer(6f, false, true);

           _fleeingDurationTimer = new Timer(PlayerController.INVINCIBLE_TIME_SECONDS, false, true);
           _chaseDurationTimer = new Timer(CHASE_DURATION_SECONDS, false, true);
           _scatterDurationTimer = new Timer(SCATTER_DURATION_SECONDS, false, true);
       }

       private void ResolveMapMarkers()
       {
           _topLeftMapBound = LevelGridController.Instance.level.topLeftPosition;
           _topRightMapBound = LevelGridController.Instance.level.topRightPosition;
           bottomLeftMapBound = LevelGridController.Instance.level.bottomLeftPosition;
           _bottomRightMapBound = LevelGridController.Instance.level.bottomRightPosition;
           _hubPosition = LevelGridController.Instance.level.ghostHousePosition;
           _restingTarget = VectorUtil.ToVector2Int(StartPosition);
           
           
           scatterTargetPosition = LevelGridController.Instance.level.topRightPosition;
           
           _hubExitTarget = LevelGridController.Instance.level.ghostDoorPosition;
       }
       private void SubscribeToEvents()
       {
           _hubExitTimer.OnEnd += SetStateToChase;
           _chasePollStepTimer.OnEnd += PollChaseTarget;
           _fleeingDurationTimer.OnEnd += SetStateToChase;
           SessionManagerSingle.Instance.powerPelletObservable.Attach(_powerPelletObserver);

           _chaseDurationTimer.OnEnd += SetStateToScatter;
           _scatterDurationTimer.OnEnd += SetStateToChase;
       }

       private void SetStateToChase()
       {
           SetState(GhostState.Chase);
       }

       private void SetStateToScatter()
       {
           if (_state != GhostState.Chase) return;
           SetState(GhostState.Scatter);
       }

       private void TargetHubExit()
       {
           currentTargetPosition = _hubExitTarget;
       }
       private void PollThreshold()
       {
           if (SessionViewModel.Instance.pelletsEaten >= pelletThreshold && _state == GhostState.Resting)
               SetState(GhostState.ExitingHub);
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
                != _hubExitTarget && _state == GhostState.Chase
                                          && pelletThreshold >= SessionViewModel.Instance.pelletsEaten)
            {
                StartChasePollStepTimer();
            }
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
        }
        
       private void OnPowerPelletCollect()
       {
           if (_state == GhostState.Eaten) return;
           animator.ResetTrigger(PowerPelletCollectTrigger);
           animator.SetTrigger(PowerPelletCollectTrigger);
           SetState(GhostState.Fleeing);
           _fleeingDurationTimer.Start();
       }
       
       
        private const int DEFAULT_DISTANCE_MAX = 999;


        protected override void FixedUpdate() {
            if (!thisRigidbody.simulated) return;
           UpdateGridPosition();
           UpdatePossibleTurns();
           _possibleTurnsTotal = GetTrueCount(possibleTurns);
           
           isInTileCenter = IsInTileCenter;

           if (!isInTileCenter && _pathResolved) _pathResolved = false;

           
           _isInIntersection = _possibleTurnsTotal > 2;
           
           if (isInTileCenter && !_pathResolved)
           {
               ResolvePath();
           }

           StateStep();

           UpdateMotion((Vector2) currentDirection * currentMoveSpeed);
        }

        protected override void UpdateAnimation()
        {
            animator.SetInteger(DirectionXAnimator, currentDirection.x);
            animator.SetInteger(DirectionYAnimator, currentDirection.y);
        }

        private void StateStep()
        {
            switch (_state)
            {
                case GhostState.ExitingHub:
                    HubExitStep();
                    break;
                case GhostState.Fleeing:
                    FleeingStep();

                    break;
                case GhostState.Eaten:
                    EatenStep();
                    break;
                case GhostState.Resting:
                    break;
                case GhostState.Chase:
                    break;
                case GhostState.Scatter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EatenStep()
        {
            if (!IsCenteredAt(_hubPosition)) return;
            SetStateToChase();
            // animator.Play("Up");
            // animator.ResetTrigger(OnFleeEndTrigger);
        }

        private void FleeingStep()
        {
            if (Math.Abs(_fleeingDurationTimer.currentTime - 0.7f * PlayerController.INVINCIBLE_TIME_SECONDS) < 0.1f)
            {
                animator.SetTrigger(OnFleeNearEndTrigger);
            }

            if (Math.Abs(_fleeingDurationTimer.currentTime - 0.81f * PlayerController.INVINCIBLE_TIME_SECONDS) < 0.1f)
            {
                animator.ResetTrigger(OnFleeNearEndTrigger);
            }
        }

        private void HubExitStep()
        {
            if (IsCenteredAt(_hubExitTarget))
            {
                SetState(GhostState.Chase);
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
                if (_state == GhostState.Resting)
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
              if (_state == GhostState.Fleeing)
                  SetState(GhostState.Eaten);
              else
                  SessionManagerSingle.Instance.Die();
          }

          private void SetTargetAwayFromPlayer() {
           Quadrant playerQuadrant = GetQuadrant(playerController.gridPosition,
               VectorUtil.ToVector2Int(LevelGridController.Instance.wallTilemap.cellBounds.center));
           Vector2Int finalPosition = playerQuadrant switch {
               Quadrant.UpRight => bottomLeftMapBound,
               Quadrant.UpLeft => _bottomRightMapBound,
               Quadrant.DownLeft => _topRightMapBound,
               Quadrant.DownRight => _topLeftMapBound,
               _ => bottomLeftMapBound
           };
                currentTargetPosition = finalPosition;
       }

          private static Quadrant GetQuadrant(Vector2Int position, Vector2Int centralPosition) {
           bool isUp = position.y >= centralPosition.y;
           bool isRight = position.x >= centralPosition.x;
           if (isUp) {
               return isRight ? Quadrant.UpRight : Quadrant.UpLeft;
           }

           return isRight ? Quadrant.DownRight : Quadrant.DownLeft;
       }

          public void SetState(GhostState newState)
       {
           if (_state == newState && (_state != newState || _state != GhostState.Fleeing)) return;
           previousState = _state;
           _state = newState;
           OnStateExit();

           OnStateEntered(newState);
       }

       public void OnStateExit()
       {
           switch (previousState)
           {
               case GhostState.Resting:
                   SessionManagerSingle.Instance.onPelletEatenObservable.Detach(_pelletEatenObserver);
                   break;
               case GhostState.ExitingHub:
                   _hubExitTimer.Stop();
                   break;
               case GhostState.Chase:
                   StopChasePollTimer();
                   break;
               case GhostState.Scatter:
                   _scatterDurationTimer.Stop();
                   break;
               case GhostState.Fleeing:
                   _fleeingDurationTimer.Stop();
                   currentMoveSpeed = standardMoveSpeed;
                   SessionManagerSingle.Instance.fleeingGhostsTotal--;
                   break;
               case GhostState.Eaten:
                   EnableCollisionsWithPlayer();
                   currentMoveSpeed = standardMoveSpeed;
                   // animator.runtimeAnimatorController = _standardAnimController;
                   animator.SetTrigger(OnArrivedGhostHouseTrigger);
                   SessionManagerSingle.Instance.eatenGhostsTotal--;
                   if (SessionManagerSingle.Instance.eatenGhostsTotal == 0)
                   {
                       AudioManagerSingle.Instance.PlayClip(
                           SessionManagerSingle.Instance.fleeingGhostsTotal == 0
                               ? AudioManagerSingle.AudioEffectType.Siren
                               : AudioManagerSingle.AudioEffectType.Fleeing, 1, true);
                   }
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }
       }

       public GhostState GetState()
       {
           return _state;
       }

       public void OnStateEntered(GhostState newState) {
           animator.ResetTrigger(OnFleeEndTrigger);
           animator.ResetTrigger(OnFleeNearEndTrigger);
           
           switch (_state)
           {
               case GhostState.Resting:
                   OnRestingEntered();
                   break;
               case GhostState.ExitingHub:
                   OnExitingHubEntered();
                   break;
               case GhostState.Chase:
               {
                   OnChaseEntered();
                   break;
               }

               case GhostState.Scatter:
               {
                   currentTargetPosition = scatterTargetPosition;
                   _scatterDurationTimer.Start();
                   break;
               }
               
               case GhostState.Fleeing:
               {
                   OnFleeingEntered();
                   break;
               }
               case GhostState.Eaten:
               {
                   OnEatenEntered();
                   break;
               }
               default: {
                   return;
               }
           }
       }

       private void OnFleeingEntered()
       {
           thisGameObject.layer = _defaultLayer;
           currentMoveSpeed = _slowMoveSpeed;
           SetTargetAwayFromPlayer();


           SessionManagerSingle.Instance.fleeingGhostsTotal++;
       }

       private void OnEatenEntered()
       {
           thisGameObject.layer = _inactiveLayer;
           currentMoveSpeed = RETREAT_MOVE_SPEED;
           currentTargetPosition = _hubPosition;
           animator.SetTrigger(OnEatenTrigger);
           SessionManagerSingle.Instance.eatenGhostsTotal++;
           AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.EatGhost, 1, false);
           SessionViewModel.Instance.IncrementScore(100 * (int) Mathf.Pow(2, SessionManagerSingle.Instance.eatenGhostsTotal));

           SessionManagerSingle.Instance.freezeTimer.Start();
           SessionManagerSingle.Instance.Freeze();
       }

       private void OnChaseEntered()
       {
           StartChasePollStepTimer();
           StartChaseDuration();
           // if (animator.runtimeAnimatorController != _standardAnimController)
           // {
           //     animator.runtimeAnimatorController = _standardAnimController;
           // }
           if (playerController.isInvincible)
           {
               AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
           }

           if (previousState == GhostState.Fleeing)
           {
               AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
               animator.ResetTrigger(OnFleeEndTrigger);
               animator.SetTrigger(OnFleeEndTrigger);
           }
       }

       private void OnExitingHubEntered()
       {
           TargetHubExit();
           _hubExitTimer.Start();
       }

       private void OnRestingEntered()
       {
           currentTargetPosition = _restingTarget;
           SessionManagerSingle.Instance.onPelletEatenObservable.Attach(_pelletEatenObserver);
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
            base.ResetActor();
            _pathResolved = false;
            FollowPath();
            SetState(GhostState.ExitingHub);
            SessionManagerSingle.Instance.eatenGhostsTotal = 0;
            animator.Play("Up");
        }

        protected override void Freeze()
        {
            base.Freeze();
            StopChasePollTimer();
            if (_state == GhostState.Eaten) animator.enabled = true;
        }
    }
}