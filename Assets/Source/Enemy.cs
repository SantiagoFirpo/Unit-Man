using System;
using System.Collections.Generic;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Timer = UnitMan.Source.Utilities.TimeTracking.Timer;

namespace UnitMan.Source {
    public class Enemy : Actor {
        //TODO: refactor/organize this class
        //TODO: use current behavior to change current target position
        //TODO: fix Clyde

        [Header("Physics State")]
        private int _possibleTurnsTotal;
        private Vector2Int OriginDirection {get {
            return currentDirection * -1;
        }}

        [Header("Physics Parameters")]

        [SerializeField]
        protected float standardMoveSpeed;

        [Header("Pathfinding Parameters")]
        public static readonly Direction[] horizontalDirections = new Direction[] {Direction.Left, Direction.Right};
        public static readonly Direction[] verticalDirections = new Direction[] {Direction.Up, Direction.Down};

        [SerializeField]
        private Transform initialTargetTransform;


        [Header("State Management")]
        private Timer _pathFindingDelay;
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
        private Vector3 _bottomLeftMapBound;
        
        [SerializeField]
        private Transform bottomRight;
        private Vector3 _bottomRightMapBound;

        [SerializeField]
       private Transform _playerTransform;


       private PlayerController _playerController;
       private float _currentMoveSpeed;

       private Vector2Int NextTile {get {
           return gridPosition + currentDirection;
       }}
       private float _slowMoveSpeed;


       public enum State
       {
           Alive, Fleeing, Dead
       }
       
       
       public State state = State.Alive;
       
       private const float MOVE_SPEED_INACTIVE = 6f;

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
       
       [SerializeField]
       private static readonly int FleeingAnimator = Animator.StringToHash("Fleeing");
       private Vector3 _currentTargetPosition;
        private bool isResolvingIntersection = false;
        private const float MAX_RETREAT_SECONDS = 6f;

       public enum Quadrant
       {
           UpRight, UpLeft, DownLeft, DownRight
       }

       protected override void Awake() {
           base.Awake();

            _topLeftMapBound = topLeft.position;
            _topRightMapBound = topRight.position;
            _bottomLeftMapBound = bottomLeft.position;
            _bottomRightMapBound = bottomRight.position;

           _currentTargetPosition = initialTargetTransform.position;

           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");

           _playerTransform = GameManager.Instance.player.transform;
           _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _pathFindingDelay = new Timer(pathfindingIntervalSeconds, 0f, true, false);
           _retreatTimer.OnEnd += ResetPositionAndState;
           _pathFindingDelay.OnEnd += PollPlayerPosition;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void PollPlayerPosition() {
           _currentTargetPosition = _playerTransform.position;
       }

       private void ResetPositionAndState() {
           thisTransform.position = startPosition;
           SetState(State.Alive);
           AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Siren, 1, true);
       }

       private void UpdateState(bool isInvincible) {
           SetState(isInvincible ? State.Fleeing : State.Alive);
       }

       private void Start() {
           _pathFindingDelay.Start();
           _currentMoveSpeed = standardMoveSpeed;
           _slowMoveSpeed = standardMoveSpeed/2f;
       }

    //    private void MultithreadedPath(Vector3 initialPosition, Vector3 finalPosition) {
    //        void ThreadStart() {
    //            Queue<Vector2Int> path = AStar.ShortestPathBetween(
    //                Vector2Int.RoundToInt(initialPosition),
    //                Vector2Int.RoundToInt(finalPosition));
    //            _positionQueue = path;
    //            _directionQueue = PositionsToTurns(path.ToArray());
    //        }
            
    //        //Toggle these two blocks to toggle multithreading:
           
    //        // ThreadStart();
           
    //        Thread thread = new Thread(ThreadStart);
    //        thread.Start();
    //    }

        private const int DEFAULT_DISTANCE_MAX = 999;

        private Direction MultithreadBestTurn(Vector2Int initialPosition, Vector2Int goalPosition) {
           Direction bestTurn = Direction.Right;
           Vector2Int originDirection = OriginDirection;
           void ThreadStart() {
               // _directionQueue.Clear();
               bool[] viableTurns = (bool[]) possibleTurns.Clone();
               int[] neighborHeuristics = new int[4] {
                                                        DEFAULT_DISTANCE_MAX,
                                                        DEFAULT_DISTANCE_MAX,
                                                        DEFAULT_DISTANCE_MAX,
                                                        DEFAULT_DISTANCE_MAX};

                for (int i = 0; i < viableTurns.Length; i++) {
                    if (!viableTurns[i] || DirectionToVector2Int((Direction) i) == originDirection) {
                        continue;
                    }
                    neighborHeuristics[i] = PathGrid.TaxiCabDistance(initialPosition + DirectionToVector2Int((Direction) i), goalPosition);
                }

                bestTurn = (Direction) FindSmallestNumberIndex(neighborHeuristics);

           }
           //Toggle these two blocks to toggle multithreading:
           
           ThreadStart();
           
        //    pathFindThread = new Thread(ThreadStart);
        //    pathFindThread.Start();
            // Debug.Log(bestTurn);
           return bestTurn;
       }

       private Direction[] GetNeighborDirections(Direction idealDirection) {
           bool isVerticalDirection = (int) idealDirection < 2;
           return isVerticalDirection ? horizontalDirections : verticalDirections;
       }

       protected override void FixedUpdate() {
           base.FixedUpdate();
           _possibleTurnsTotal = GetTrueCount(possibleTurns);

           bool isInIntersection =_possibleTurnsTotal > 2;
           if (!isInIntersection && isResolvingIntersection) {
               isResolvingIntersection = false;
           }
           
           if (isInIntersection && !isResolvingIntersection) { //_canPathfind && 
            isResolvingIntersection = true;
            //    _pathFindingDelay.Start();
            //    _canPathfind = false;
            //    // ComputePathToPlayer();
            if (PathGrid.VectorApproximately(thisTransform.position, gridPosition, 0.1f)) {
                currentDirection = DirectionToVector2Int(
                                MultithreadBestTurn(gridPosition, PathGrid.VectorToVector2Int(_currentTargetPosition)));
            }
           }
           
        //    if (_directionQueue.Count > 0 && _positionQueue.Count > 0) {
        //        // MoveThroughPath();
        //        FollowPath();
        //    }
            else if (rigidBody.velocity == Vector2.zero) { //else 
               TurnToValidDirection();
           }

           if (PathGrid.VectorApproximately(thisTransform.position, startPosition, POSITION_CHECK_TOLERANCE)  && state == State.Dead) {
               SetState(State.Alive);
           }

           motion = (Vector2) currentDirection * _currentMoveSpeed;
           rigidBody.velocity = motion;
       }

       private static int GetTrueCount(bool[] boolArray) {
           int result = 0;
           foreach (bool item in boolArray) {
               if (item) {
                   result++;
               }
           }
           return result;
       }

       private const float POSITION_CHECK_TOLERANCE = 0.07f; //_currentMoveSpeed * SPEED_TOLERANCE_CONVERSION;
        
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

       

       

       private static Queue<Vector2Int> PositionsToTurns(Vector2Int[] positions) {
           int turnsLength = positions.Length - 1;
           if (turnsLength <= 0) return null;
           Queue<Vector2Int> result = new Queue<Vector2Int>(turnsLength);
           for (int i = 0; i < turnsLength; i++) {
               result.Enqueue(positions[i+1] - positions[i]);
           }
           return result;
       }

       private bool CanPathfind() {
           return (!_pathFindingDelay.Active) && _pathFindingDelay.currentTime == _pathFindingDelay.waitTime;
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
           Quadrant playerQuadrant = GetQuadrant(_playerTransform.position, _mapCentralPosition);
           Vector3 finalPosition = playerQuadrant switch {
               Quadrant.UpRight => _bottomLeftMapBound,
               Quadrant.UpLeft => _bottomRightMapBound,
               Quadrant.DownLeft => _topRightMapBound,
               Quadrant.DownRight => _topLeftMapBound,
               _ => _bottomLeftMapBound
           };
                _currentTargetPosition = finalPosition;
       }

       private void TurnToValidDirection() {
           Vector2Int originDirection = currentDirection * -1;
           int possibleTurnsTotal = 0;
           for (int i = 0; i <= 3; i++) {
               if (!possibleTurns[i] || VectorToInt(OriginDirection) == i) continue;
               possibleTurnsTotal++;
               if (Actor.DirectionToVector2Int(i) != originDirection || possibleTurnsTotal == 1) {
                   currentDirection = Actor.DirectionToVector2Int(i);
               }
           }
       }

       private static Quadrant GetQuadrant(Vector2 position, Vector2 centralPosition) {
           bool isUp = position.y >= centralPosition.y;
           bool isRight = position.x >= centralPosition.x;
           if (isUp) {
               return isRight ? Quadrant.UpRight : Quadrant.UpLeft;
           }
           else {
               return isRight ? Quadrant.DownRight : Quadrant.DownLeft;
           }
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
                   _pathFindingDelay.Start();
                   animator.SetBool(FleeingAnimator, false);
                   _currentTargetPosition = _playerTransform.position;

                   break;
               }
               case State.Fleeing: {
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = _slowMoveSpeed;
                   SetTargetAwayFromPlayer();
                   _pathFindingDelay.Stop();
                   animator.SetBool(FleeingAnimator, true);
                   break;
               }
               case State.Dead: {
                   thisGameObject.layer = _inactiveLayer;
                   _currentMoveSpeed = MOVE_SPEED_INACTIVE;
                   _currentTargetPosition = startPosition;
                   _pathFindingDelay.Stop();
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