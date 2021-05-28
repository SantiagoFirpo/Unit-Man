using System.Collections.Generic;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private readonly Timer _directionTimer = new Timer(PATHFINDING_INTERVAL_SECONDS, 0f, true, false);
       private const float MOVE_SPEED = 1f;
       private const float FIXED_MOVE_SPEED = 0.05f;
       private Agent _agent;
       private Queue<PathNode> _nodeQueue = new Queue<PathNode>();

       [SerializeField]
       private Transform playerTransform;

       private Vector2 _currentDirection;
       private Vector2 _previousGridPosition;

       private const float PATHFINDING_INTERVAL_SECONDS = 4f;

       protected override void Awake() {
           base.Awake();
           UpdatePath();
           _directionTimer.OnEnd += UpdatePath;
           _previousGridPosition = _transform.position;
       }

       private void Start() {
           _directionTimer.Begin();
       }

       private Queue<PathNode> ShortestPathToPlayer() {
           return AStar.ShortestPathBetween(Vector2Int.RoundToInt(_transform.position), Vector2Int.RoundToInt(playerTransform.position));
       }

       protected override void FixedUpdate() {
           base.FixedUpdate();
           if (_nodeQueue.Count == 0) return;
           MoveThroughPath();
       }

       private void MoveThroughPath() {
           Vector2 nextPosition = _nodeQueue.Peek().position;
           Vector3 position = _transform.position;
           _currentDirection = nextPosition - _previousGridPosition;
           _transform.position = Vector2.MoveTowards(position, nextPosition, FIXED_MOVE_SPEED);
           motion = _currentDirection * MOVE_SPEED;
           if ((Vector2) _transform.position != nextPosition) return;
           _nodeQueue.Dequeue();
           _previousGridPosition = nextPosition;
       }

       private void UpdatePath() {
           _nodeQueue = ShortestPathToPlayer();
       }
    }
}