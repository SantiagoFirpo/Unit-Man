using System;
using System.Collections.Generic;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private readonly Timer _directionTimer = new Timer(PATHFINDING_INTERVAL_SECONDS, 0f, true, false);
       private const float MOVE_SPEED = 1f;
       private const float FIXED_MOVE_SPEED = MOVE_SPEED / 50f;
       private Agent _agent;
       private Queue<PathNode> _nodeQueue = new Queue<PathNode>();

       [SerializeField]
       private Transform playerTransform;

       private Vector2Int _gridPosition;
       private Vector2Int _direction;

       private const float PATHFINDING_INTERVAL_SECONDS = 4f;

       protected override void Awake() {
           base.Awake();
           startPosition = new Vector3(1f, 0f, 0f);
           _agent = GetComponent<Agent>();
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
           if (_nodeQueue.Count == 0) return;
           MoveThroughPath();
       }

       

       private void UpdateGridPosition() {
           _gridPosition = Vector2Int.RoundToInt(_transform.position);
       }

       private void MoveThroughPath() {
           Vector2Int nextPosition = _nodeQueue.Peek().position;
           Vector2Int actualDirection = nextPosition - _gridPosition;
           _direction = actualDirection == Vector2Int.zero ? _direction : actualDirection;
           motion = _direction * (int) MOVE_SPEED;
           // _transform.position = Vector2.MoveTowards(_transform.position, _gridPosition + _direction, FIXED_MOVE_SPEED);
           _rigidBody.velocity = motion;
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
       
    }
}