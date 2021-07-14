using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors {
    public class GhostController : Actor {
        //TODO: refactor/organize this class
        //BUG: ghosts stay blue after reset
        // Power pellet animation and sound bugs


        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");
        

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection => currentDirection * -1;

        [Header("Physics Parameters")]
        
        protected const float INKY_BLINKY_PINKY_MOVE_SPEED = 3.5f;
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

        private Observer _pelletEatenObserver;
        private Observer _powerPelletObserver;

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
        protected PlayerController playerController;
        
        //Components

        [Header("Debug Parameters")]
        [SerializeField]
        private Color debugColor;
        
        [Header("Utilities")]
        private static readonly Func<bool,bool> IsElementTrue = element => element;

        private Timer _fleeingDurationTimer;
        private static readonly int OnNearEndAnimTrigger = Animator.StringToHash("OnNearEnd");
        private static readonly int OnFleeEndAnimTrigger = Animator.StringToHash("OnFleeEnd");
        
        private Vector2Int _restingTarget;


        public override void Initialize() {
           base.Initialize();

           ResolveDependencies();
            
            SubscribeToEvents();

            currentMoveSpeed = standardMoveSpeed;
            _slowMoveSpeed = standardMoveSpeed/2f;
            pelletThreshold = 1;
           
            // _standardAnimController = animator.runtimeAnimatorController;
        }
       private void ResolveDependencies()
       {
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
           _chasePollStepTimer = new Timer(PlayerController.PLAYER_STEP_TIME, false, false); //old: chasePollSeconds as waitTime
           _hubExitTimer = new Timer(6f, false, true);
           
           _fleeingDurationTimer = new Timer(PlayerController.INVINCIBLE_TIME_SECONDS, false, false);
           _chaseDurationTimer = new Timer(CHASE_DURATION_SECONDS, false, true);
           _scatterDurationTimer = new Timer(SCATTER_DURATION_SECONDS, false, true);

           _pelletEatenObserver = new Observer(PollThreshold);
           _powerPelletObserver = new Observer(SetStateToFleeing);
           ResolveMapMarkers();
       }
       private void ResolveMapMarkers()
       {
           _topLeftMapBound = LevelGridController.Instance.mazeData.topLeftMapPosition;
           _topRightMapBound = LevelGridController.Instance.mazeData.topRightMapPosition;
           bottomLeftMapBound = LevelGridController.Instance.mazeData.bottomLeftMapPosition;
           _bottomRightMapBound = LevelGridController.Instance.mazeData.bottomRightMapPosition;
           hubPosition = LevelGridController.Instance.mazeData.hubPosition;
           _restingTarget = LevelGridController.Instance.mazeData.restingTargetPosition;
           
           
           scatterTargetPosition = LevelGridController.Instance.mazeData.topRightMapPosition;
           
           _hubExitTarget = LevelGridController.Instance.mazeData.hubExitMarker;
       }
       private void SubscribeToEvents()
       {
           // _retreatTimer.OnEnd += ResetPositionAndState;
           _hubExitTimer.OnEnd += SetStateToChase;
           _chasePollStepTimer.OnEnd += PollChaseTarget;
           _fleeingDurationTimer.OnEnd += SetStateToChase;
           SessionManagerSingle.Instance.powerPelletEmitter.Attach(_powerPelletObserver);

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
           currentTargetPosition = _hubExitTarget;
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
                != _hubExitTarget && state == State.Chase
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
            switch (state)
            {
                case State.ExitingHub:
                    if (IsCenteredAt(_hubExitTarget))
                    {
                        SetState(State.Chase);
                    }
                    break;
                case State.Fleeing:
                    if (_fleeingDurationTimer.currentTime > 0.7f * PlayerController.INVINCIBLE_TIME_SECONDS)
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
                case State.Resting:
                    break;
                case State.Chase:
                    break;
                case State.Scatter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
           Quadrant playerQuadrant = GetQuadrant(playerController.gridPosition,
               LevelGridController.Instance.mazeData.mapCentralPosition);
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
                   SessionManagerSingle.Instance.onPelletEatenEmitter.Detach(_pelletEatenObserver);
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

                   SessionManagerSingle.Instance.fleeingGhostsTotal--;
                   break;
               case State.Eaten:
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

       private void OnStateEntered() {
           // Debug.Log($"Entered state {state}", thisGameObject);
           switch (state)
           {
               case State.Resting:
                   currentTargetPosition = _restingTarget;
                   SessionManagerSingle.Instance.onPelletEatenEmitter.Attach(_pelletEatenObserver);
                   break;
               case State.ExitingHub:
                   TargetHubExit();
                   _hubExitTimer.Start();
                   break;
               case State.Chase: {

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
                   break;
               }

               case State.Scatter:
               {
                   currentTargetPosition = scatterTargetPosition;
                   _scatterDurationTimer.Start();
                   break;
               }
                   
               
               
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   animator.SetBool(FleeingAnimator, true);

                   SessionManagerSingle.Instance.fleeingGhostsTotal++;
                   break;
               }
               case State.Eaten: {
                   thisGameObject.layer = _inactiveLayer;
                   currentMoveSpeed = RETREAT_MOVE_SPEED;
                   currentTargetPosition = _hubPosition;
                   animator.SetTrigger(OnEatenTrigger);
                   SessionManagerSingle.Instance.eatenGhostsTotal++;
                   AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.EatGhost, 1, false);
                   SessionDataModel.Instance.IncrementScore(100 * (int) Mathf.Pow(2, SessionManagerSingle.Instance.eatenGhostsTotal));
                   
                   SessionManagerSingle.Instance.freezeTimer.Start();
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
            base.ResetActor();
            SetState(State.ExitingHub);
            if (_possibleTurnsTotal == 1) FollowPath();
            SessionManagerSingle.Instance.eatenGhostsTotal = 0;
        }

        protected override void Freeze()
        {
            base.Freeze();
            StopChasePollTimer();
            if (state == State.Eaten) animator.enabled = true;
        }
    }
    //BUG: if ghost is fleeing and player dies, animation takes a while to change
}