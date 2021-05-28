using System;
using System.Collections.Generic;
using UnitMan.Source.Utilities;
using UnitMan.Source.Utilities.AI;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private readonly Timer _directionTimer = new Timer(PATHFIND_INTERVAL_SECONDS, 0f, true, false);
       private const float MOVE_SPEED = 0.05f;
       private const float FIXED_MOVE_SPEED = MOVE_SPEED / 50f;
       private Agent _agent;
       private Queue<PathNode> _nodeQueue = new Queue<PathNode>();

       [SerializeField]
       private Transform playerTransform;

       private const float PATHFIND_INTERVAL_SECONDS = 4f;

       protected override void Awake() {
           base.Awake();
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

       private void FixedUpdate() {
           if (_nodeQueue.Count == 0) return;
           MoveThroughPath();
       }

       private void MoveThroughPath() {
           Vector2 nextPosition = _nodeQueue.Peek().position;
           _transform.position = Vector2.MoveTowards(_transform.position, nextPosition, MOVE_SPEED);
           _rigidBody.velocity = motion;
           if ((Vector2) _transform.position == nextPosition) {
               _nodeQueue.Dequeue();
           }
       }

       private void UpdatePath() {
           _nodeQueue = ShortestPathToPlayer();
       }
    }
}