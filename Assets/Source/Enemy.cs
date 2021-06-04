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
       
       private bool _isAlive = true;
       private const float MOVE_SPEED_INACTIVE = 15f;

       private readonly Vector3 _upRightMap = new Vector3(9f, 2f, 0f);
       private readonly Vector3 _upLeftMap = new Vector3(-9f, 3f, 0f);
       private readonly Vector3 _downLeftMap = new Vector3(-9f, -20f, 0f);
       private readonly Vector3 _downRightMap = new Vector3(9f, -20f, 0f);

       private readonly Vector2 _mapCentralPosition = new Vector2(0, -8.5f);

       protected override void Awake() {
           base.Awake();
           _inactiveLayer = LayerMask.NameToLayer("Dead");
           _defaultLayer = LayerMask.NameToLayer("Enemies");
           startPosition = thisTransform.position;
           _playerTransform = GameManager.Instance.player.transform;
           _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _pathToPlayerTimer = new Timer(pathfindingIntervalSeconds, 0f, true, false);
           ComputePathToPlayer();
           _pathToPlayerTimer.OnEnd += ComputePathToPlayer;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void UpdateState(bool isInvincible) {
           if (!_isAlive) return;
           _currentMoveSpeed = isInvincible ? _slowMoveSpeed : standardMoveSpeed;
           _pathToPlayerTimer.paused = isInvincible;
           if (!isInvincible) return;
           _positionQueue.Clear();
           ComputePathAwayFromPlayer();
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

           if (!_isAlive && _positionQueue != null) {
               _isAlive = true;
               thisGameObject.layer = _inactiveLayer;
               _currentMoveSpeed = standardMoveSpeed;
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
           if (_playerController.isInvincible) {
               _isAlive = false;
               _pathToPlayerTimer.paused = true;
               _positionQueue.Clear();
               _currentMoveSpeed = MOVE_SPEED_INACTIVE;
               ComputePathToHub();
               // thisTransform.position = startPosition;
           }
       }

       private void ComputePathToPlayer() {
           MultithreadedPath(thisTransform.position, _playerTransform.position);
       }
       
       private void ComputePathToHub() {
           MultithreadedPath(thisTransform.position, startPosition);
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
       
    }
}