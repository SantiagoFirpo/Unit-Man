using System.Collections.Generic;
using System.Threading;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Timer = UnitMan.Source.Utilities.TimeTracking.Timer;

namespace UnitMan.Source {
    public abstract class Enemy : Actor {
       private Timer _pathToPlayerTimer;
       protected float standardMoveSpeed;
       private Queue<Vector2Int> _positionQueue = new Queue<Vector2Int>();
       
       private Transform _playerTransform;

       private Vector2Int _gridPosition;
       private Vector2Int _direction;

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
       
       public State state = State.Alive;

       private bool _isAlive = true;
       private const float MOVE_SPEED_INACTIVE = 15f;

       private readonly Vector3 _upRightMap = new Vector3(9f, 2f, 0f);
       private readonly Vector3 _upLeftMap = new Vector3(-9f, 3f, 0f);
       private readonly Vector3 _downLeftMap = new Vector3(-9f, -20f, 0f);
       private readonly Vector3 _downRightMap = new Vector3(9f, -20f, 0f);

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);
       private Vector3 _hubPosition;

       protected override void Awake() {
           base.Awake();
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");
           startPosition = thisTransform.position;
           _hubPosition = new Vector3(2f, 1f, 0f);
           _playerTransform = GameManager.Instance.player.transform;
           _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _pathToPlayerTimer = new Timer(pathfindingIntervalSeconds, 0f, true, false);
           ComputePathToPlayer();
           _pathToPlayerTimer.OnEnd += ComputePathToPlayer;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void UpdateState(bool isInvincible) {
           SetState(isInvincible ? State.Fleeing : State.Alive);
       }

       private void Start() {
           _pathToPlayerTimer.Begin();
           _currentMoveSpeed = standardMoveSpeed;
           _slowMoveSpeed = standardMoveSpeed/2f;
       }

       private void MultithreadedPath(Vector3 initialPosition, Vector3 finalPosition) {
           void ThreadStart() {
               Queue<Vector2Int> path = AStar.ShortestPathBetween(
                   Vector2Int.RoundToInt(initialPosition),
                   Vector2Int.RoundToInt(finalPosition));
               
               lock (_positionQueue) {
                   _positionQueue = path;
               }
           }

           Thread thread = new Thread(ThreadStart);
           thread.Start();
       }

       protected override void FixedUpdate() {
           base.FixedUpdate();
           UpdateGridPosition();
           if (_positionQueue.Count > 0) {
               MoveThroughPath();
           }
           if (rigidBody.velocity == Vector2.zero) { //else 
               TurnToValidDirection();
           }

           if (thisTransform.position == startPosition && state == State.Dead) {
               SetState(State.Alive);
           }

           motion = (Vector2) _direction * _currentMoveSpeed;
           rigidBody.velocity = motion;
       }

       private void UpdateGridPosition() {
           _gridPosition = Vector2Int.RoundToInt(thisTransform.position);
       }

       private void MoveThroughPath() {
           Vector2Int nextPosition = _positionQueue.Peek();
           Vector2Int actualDirection = nextPosition - _gridPosition;
           _direction = actualDirection == Vector2Int.zero ? _direction : actualDirection;
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           if (VectorApproximately(thisTransform.position, nextPosition, 0.08f)) { // previous value: 0.05f
               _positionQueue.Dequeue();
           }
       }

       private void OnCollisionEnter2D(Collision2D other) {
           if (!other.gameObject.CompareTag("Player")) return;
           if (state == State.Fleeing) {
               SetState(State.Dead);
               // thisTransform.position = startPosition;
           }
           else {
               GameManager.Instance.Die();
           }
       }

       private void ComputePathToPlayer() {
           MultithreadedPath(thisTransform.position, _playerTransform.position);
       }
       
       private void ComputePathToHub() {
           MultithreadedPath(thisTransform.position, _hubPosition);
       }
       
       private void ComputePathAwayFromPlayer() {
           int playerQuadrant = GetQuadrant(_playerTransform.position, _mapCentralPosition);
           Vector3 finalPosition = playerQuadrant switch {
               1 => _downLeftMap,
               2 => _downRightMap,
               3 => _upRightMap,
               4 => _upLeftMap,
               _ => startPosition
           };
           
           MultithreadedPath(thisTransform.position, finalPosition);
       }

       private void TurnToValidDirection() {
           Vector2Int originDirection = _direction * -1;
           int possibleTurnsTotal = 0;
           for (int i = 0; i <= 3; i++) {
               if (!possibleTurns[i]) continue;
               possibleTurnsTotal++;
               if (Actor.IntToDirection(i) != originDirection || possibleTurnsTotal == 1) {
                   _direction = Actor.IntToDirection(i);
               }
           }
       }

       private static int GetQuadrant(Vector2 position, Vector2 centralPosition) {
           bool isUp = position.y >= centralPosition.y;
           bool isRight = position.x >= centralPosition.x;
           return isRight switch {
               true when isUp => 1,
               false when isUp => 2,
               false => 3,
               true => 4
           };
       }

       private void SetState(State state) {
           this.state = state;
           OnStateEntered();
       }

       private void OnStateEntered() {
           switch (state)
           {
               case State.Alive: {
                   _positionQueue.Clear();
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = standardMoveSpeed;
                   _pathToPlayerTimer.paused = false;
                   ComputePathToPlayer();
                   
                   break;
               }
               case State.Fleeing: {
                   _positionQueue.Clear();
                   thisGameObject.layer = _defaultLayer;
                   _currentMoveSpeed = _slowMoveSpeed;
                   _pathToPlayerTimer.paused = true;
                   ComputePathAwayFromPlayer();
                   
                   break;
               }
               case State.Dead: {
                   thisGameObject.layer = _inactiveLayer;
                   _positionQueue.Clear();
                   _currentMoveSpeed = MOVE_SPEED_INACTIVE;
                   _pathToPlayerTimer.paused = true;
                   // ComputePathToHub();
                   thisTransform.position = startPosition;
                   
                   break;
               }
               default: {
                   return;
               }
           }
       }
    }
}