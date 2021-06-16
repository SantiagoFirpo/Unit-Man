using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Timer = UnitMan.Source.Utilities.TimeTracking.Timer;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private Timer _pathFindingDelay;
       
       [SerializeField]
       protected float standardMoveSpeed;
       private Queue<Vector2Int> _positionQueue = new Queue<Vector2Int>();
       
       private readonly Timer _retreatTimer = new Timer(MAX_RETREAT_SECONDS);
       
       private Transform _playerTransform;

       private Vector2Int _gridPosition;

       [SerializeField]
       protected float pathfindingIntervalSeconds = 4f;
       private PlayerController _playerController;
       private float _currentMoveSpeed;
       private float _slowMoveSpeed;

       private int _inactiveLayer;
       private int _defaultLayer;

       public enum State
       {
           Alive, Fleeing, Dead
       }
       
       public static readonly Direction[] horizontalDirections = new Direction[] {Direction.Left, Direction.Right};
       public static readonly Direction[] verticalDirections = new Direction[] {Direction.Up, Direction.Down};
       
       public State state = State.Alive;
       
       private const float MOVE_SPEED_INACTIVE = 6f;
       private const float SPEED_TOLERANCE_CONVERSION = 0.017f;

       private readonly Vector3 _upRightMap = new Vector3(9f, 2f, 0f);
       private readonly Vector3 _upLeftMap = new Vector3(-9f, 3f, 0f);
       private readonly Vector3 _downLeftMap = new Vector3(-9f, -20f, 0f);
       private readonly Vector3 _downRightMap = new Vector3(9f, -20f, 0f);

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
       private Vector3 _hubPosition;
       
       [SerializeField]
       private Color debugColor;

       private Queue<Vector2Int> _directionQueue = new Queue<Vector2Int>();
       private static readonly int FleeingAnimator = Animator.StringToHash("Fleeing");
       private int _possibleTurnsTotal;
       private bool _canPathfind;
       private const float MAX_RETREAT_SECONDS = 6f;

       public enum Quadrant
       {
           UpRight, UpLeft, DownLeft, DownRight
       }

       protected override void Awake() {
           base.Awake();
           _positionQueue.Clear();
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");
           startPosition = thisTransform.position;
           _hubPosition = new Vector3(2f, 0f, 0f);
           _playerTransform = GameManager.Instance.player.transform;
           _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _pathFindingDelay = new Timer(pathfindingIntervalSeconds, 0f, true, true);
           _retreatTimer.OnEnd += ResetPositionAndState;
           // ComputePathToPlayer();
           _pathFindingDelay.OnEnd += UpdateCanPathfind;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void UpdateCanPathfind() {
           _canPathfind = true;
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
           _pathFindingDelay.Begin();
           _currentMoveSpeed = standardMoveSpeed;
           _slowMoveSpeed = standardMoveSpeed/2f;
       }

       private void MultithreadedPath(Vector3 initialPosition, Vector3 finalPosition) {
           void ThreadStart() {
               Queue<Vector2Int> path = AStar.ShortestPathBetween(
                   Vector2Int.RoundToInt(initialPosition),
                   Vector2Int.RoundToInt(finalPosition));
               _positionQueue = path;
               _directionQueue = PositionsToTurns(path.ToArray());
           }
            
           //Toggle these two blocks to toggle multithreading:
           
           // ThreadStart();
           
           Thread thread = new Thread(ThreadStart);
           thread.Start();
       }

       private void MultithreadedNextTurn(Vector3 initialPosition, Vector3 finalPosition) {
           _directionQueue.Clear();
           Direction idealDirection = GetBestDirection(initialPosition, finalPosition);
           Direction[] neighborDirections = GetNeighborDirections(idealDirection);
           Direction directionBuffer = idealDirection;
           if (!possibleTurns[(int) idealDirection]) {
               if (!possibleTurns[(int) neighborDirections[0]]) {
                   directionBuffer = neighborDirections[1];
               }
           }
           _directionQueue.Enqueue( DirectionToVector2Int(directionBuffer));
       }

       private Direction[] GetNeighborDirections(Direction idealDirection) {
           bool isVerticalDirection = (int) idealDirection < 2;
           return isVerticalDirection ? horizontalDirections : verticalDirections;
       }

       private static Direction GetBestDirection(Vector3 initialPosition, Vector3 finalPosition) {
           float offsetX = finalPosition.x - initialPosition.x;
           float offsetY = finalPosition.y - initialPosition.y;

           if (Mathf.Abs(offsetX) > Mathf.Abs(offsetY)) {
               return offsetY > 0f ? Direction.Up : Direction.Down;
           }

           return offsetX > 0f ? Direction.Right : Direction.Left;
       }

       protected override void FixedUpdate() {
           base.FixedUpdate();
           UpdateGridPosition();
           _possibleTurnsTotal = GetTrueCount(possibleTurns);

           bool isInIntersection = _possibleTurnsTotal > 2;
           if (_canPathfind && isInIntersection) {
               _pathFindingDelay.Begin();
               _canPathfind = false;
               ComputePathToPlayer();
           }

           // if (_canPathfind && isInIntersection) {
           //     MultithreadedNextTurn(_gridPosition, _playerTransform.position);
           // }
           
           if (_directionQueue.Count > 0 && _positionQueue.Count > 0) {
               // MoveThroughPath();
               FollowPath();
           }
           if (rigidBody.velocity == Vector2.zero) { //else 
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

       private void UpdateGridPosition() {
           _gridPosition = Vector2Int.RoundToInt(thisTransform.position);
       }
        
       private void MoveThroughPath() {
           Vector2Int nextPosition = _positionQueue.Peek();
           Vector2Int actualDirection = nextPosition - _gridPosition;
           // Debug.Log();
               
           currentDirection = !IsCardinalDirection(actualDirection) ? currentDirection : actualDirection;
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           if (PathGrid.VectorApproximately(thisTransform.position, nextPosition, _currentMoveSpeed * SPEED_TOLERANCE_CONVERSION)) { // previous value: 0.05f
               _positionQueue.Dequeue();
           }
       }
       
       private void FollowPath() {
           Vector2Int nextPosition = _positionQueue.Peek();
           Vector2Int nextDirection = _directionQueue.Peek();
           if (possibleTurns[VectorToInt(nextDirection)]) {
               currentDirection = nextDirection;
           }
           // Debug.Log(_direction, thisGameObject);
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           if (!PathGrid.VectorApproximately(thisTransform.position, nextPosition,
               _currentMoveSpeed * SPEED_TOLERANCE_CONVERSION)) return; // previous value: 0.05f
           _positionQueue.Dequeue();
           _directionQueue.Dequeue();
       }

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
           return (!_pathFindingDelay.active) && _pathFindingDelay.currentTime == _pathFindingDelay.waitTime;
       }

       private void ComputePathToPlayer() {
           MultithreadedPath(thisTransform.position, _playerTransform.position);
           RemoveFirstPosition();
       }
       
       private void ComputePathToHub() {
           MultithreadedPath(thisTransform.position, _hubPosition);
           RemoveFirstPosition();
       }
       
       private void ComputePathAwayFromPlayer() {
           Quadrant playerQuadrant = GetQuadrant(_playerTransform.position, _mapCentralPosition);
           Vector3 finalPosition = playerQuadrant switch {
               Quadrant.UpRight => _downLeftMap,
               Quadrant.UpLeft => _downRightMap,
               Quadrant.DownLeft => _upRightMap,
               Quadrant.DownRight => _upLeftMap,
               _ => startPosition
           };
           
           MultithreadedPath(thisTransform.position, finalPosition);
           RemoveFirstPosition();
       }

       private void RemoveFirstPosition() {
           if (_positionQueue.Count == 0) return;
           _positionQueue.Dequeue();
       }

       private void TurnToValidDirection() {
           Vector2Int originDirection = currentDirection * -1;
           int possibleTurnsTotal = 0;
           for (int i = 0; i <= 3; i++) {
               if (!possibleTurns[i]) continue;
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
           _positionQueue.Clear();
           state = targetState;
           OnStateEntered();
       }

       private void OnStateEntered() {
           switch (state)
           {
               case State.Alive: {
                   _positionQueue.Clear();
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = standardMoveSpeed;
                   _pathFindingDelay.Begin();
                   animator.SetBool(FleeingAnimator, false);
                   // ComputePathToPlayer();

                   break;
               }
               case State.Fleeing: {
                   _positionQueue.Clear();
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = _slowMoveSpeed;
                   _pathFindingDelay.active = false;
                   animator.SetBool(FleeingAnimator, true);
                   ComputePathAwayFromPlayer();
                   break;
               }
               case State.Dead: {
                   thisGameObject.layer = _inactiveLayer;
                   _positionQueue.Clear();
                   _currentMoveSpeed = MOVE_SPEED_INACTIVE;
                   _pathFindingDelay.active = false;
                   // ComputePathToHub();
                   // thisTransform.position = startPosition;
                   
                   break;
               }
               default: {
                   return;
               }
           }
       }

       protected override void OnDrawGizmos() {
           base.OnDrawGizmos();
           if (_positionQueue.Count == 0) return;
           Vector2 previousPosition = _positionQueue.Peek();
           foreach (Vector2Int position in _positionQueue) {
               Gizmos.color = debugColor;
               Gizmos.DrawLine(previousPosition, (Vector2) position);
               previousPosition = position;
           }
       }
    }
}