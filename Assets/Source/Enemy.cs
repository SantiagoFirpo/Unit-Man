using System;
using System.Collections.Generic;
using System.Threading;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Timer = UnitMan.Source.Utilities.TimeTracking.Timer;

namespace UnitMan.Source {
    public abstract class Enemy : Actor {
       private Timer _directionTimer;
       protected float moveSpeed;
       private Queue<Vector2Int> lockedPositionQueue = new Queue<Vector2Int>();
       private Queue<Vector2Int> _positionQueue = new Queue<Vector2Int>();
       
       private Transform _playerTransform;

       private Vector2Int _gridPosition;
       private Vector2Int _direction;

       protected float pathfindingIntervalSeconds = 4f;
       private PlayerController _playerController;
       private float _currentMoveSpeed;
       private float _slowMoveSpeed;

       private readonly Vector3 _upRightMap = new Vector3(9f, 2f, 0f);
       private readonly Vector3 _upLeftMap = new Vector3(-9f, 3f, 0f);
       private readonly Vector3 _downLeftMap = new Vector3(-9f, -20f, 0f);
       private readonly Vector3 _downRightMap = new Vector3(9f, -20f, 0f);

       protected override void Awake() {
           base.Awake();
           startPosition = new Vector3(2f, 0f, 0f);
           _playerTransform = GameManager.Instance.player.transform;
           _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
           _directionTimer = new Timer(pathfindingIntervalSeconds, 0f, true, false);
           ComputePathToPlayer();
           _directionTimer.OnEnd += ComputePathToPlayer;
           PlayerController.OnInvincibleChanged += UpdateState;
       }

       private void UpdateState(bool isInvincible) {
           _currentMoveSpeed = isInvincible ? _slowMoveSpeed : moveSpeed;
       }

       private void Start() {
           _directionTimer.Begin();
           _currentMoveSpeed = moveSpeed;
           _slowMoveSpeed = moveSpeed/2f;
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
           if (rigidBody.velocity == Vector2.zero) {
               TurnToValidDirection();
           }

           motion = (Vector2) _direction * _currentMoveSpeed;
           rigidBody.velocity = motion;
       }

       private void UpdateGridPosition() {
           _gridPosition = Vector2Int.RoundToInt(_transform.position);
       }

       private void MoveThroughPath() {
           Vector2Int nextPosition = _positionQueue.Peek();
           Vector2Int actualDirection = nextPosition - _gridPosition;
           _direction = actualDirection == Vector2Int.zero ? _direction : actualDirection;
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           if (VectorApproximately(_transform.position, nextPosition, 0.05f)) {
               _positionQueue.Dequeue();
           }
       }

       private void OnCollisionEnter2D(Collision2D other) {
           if (!other.gameObject.CompareTag("Player")) return;
           if (_playerController.isInvincible) {
               // gameObject.SetActive(false);
               _transform.position = startPosition;
           }
       }

       private void ComputePathToPlayer() {
           MultithreadedPath(_transform.position, _playerTransform.position);

       }

       private void TurnToValidDirection() {
           Vector2Int originDirection = _direction * -1;
           for (int i = 0; i <= 3; i++) {
               if (!possibleTurns[i]) continue;
               if (Actor.IntToDirection(i) != originDirection) {
                   _direction = Actor.IntToDirection(i);
               }
           }
       }

       
    }
}