using System;
using System.Collections.Generic;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public abstract class Enemy : Actor {
       private Timer _directionTimer;
       protected float moveSpeed;
       private float _fixedMoveSpeed;
       private Agent _agent;
       private Queue<PathNode> _nodeQueue = new Queue<PathNode>();

       [SerializeField]
       private Transform playerTransform;

       private Vector2Int _gridPosition;
       private Vector2Int _direction;

       protected float pathfindingIntervalSeconds = 4f;

       protected override void Awake() {
           base.Awake();
           startPosition = new Vector3(1f, 0f, 0f);
           _fixedMoveSpeed = moveSpeed / 50f;
           _agent = GetComponent<Agent>();
           _directionTimer = new Timer(pathfindingIntervalSeconds, 0f, true, false);
           UpdatePath();
           _directionTimer.OnEnd += UpdatePath;
       }

       private void Start() {
           _directionTimer.Begin();
       }

       private Queue<PathNode> ShortestPathToPlayer() {
           return AStar.ShortestPathBetween(Vector2Int.RoundToInt(_transform.position), Vector2Int.RoundToInt(playerTransform.position));
       }

       protected override void FixedUpdate() {
           base.FixedUpdate();
           UpdateGridPosition();
           if (_nodeQueue.Count != 0) {
               MoveThroughPath();
           }
           _rigidBody.velocity = motion;
       }

       

       private void UpdateGridPosition() {
           _gridPosition = Vector2Int.RoundToInt(_transform.position);
       }

       private void MoveThroughPath() {
           Vector2Int nextPosition = _nodeQueue.Peek().position;
           Vector2Int actualDirection = nextPosition - _gridPosition;
           _direction = actualDirection == Vector2Int.zero ? _direction : actualDirection;
           motion = _direction * (int) moveSpeed;
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           if (VectorApproximately(_transform.position, nextPosition, 0.05f)) {
               _nodeQueue.Dequeue();
           }
       }

       private static bool VectorApproximately(Vector3 v1, Vector2Int v2, float maxDelta) {
           return (Mathf.Abs(v1.x - v2.x) <= maxDelta && Mathf.Abs(v1.y - v2.y) <= maxDelta);
       }

       private void UpdatePath() {
           _nodeQueue = ShortestPathToPlayer();
       }

       // private void OnCollisionEnter2D(Collision2D other) {
       //     if (other.gameObject.layer != wallLayer) return;
       //     if (_nodeQueue.Count == 0 || _possibleTurnsAmount > 2) return;
       //     for (int i = 0; i < 3; i++) {
       //         if (!possibleTurns[i]) continue;
       //         if (allDirections[i] == _direction) continue;
       //         _direction = allDirections[i];
       //         return;
       //     }
       // }

       
    }
}