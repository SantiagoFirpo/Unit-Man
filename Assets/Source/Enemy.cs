using System;
using System.Collections.Generic;
using UnitMan.Source.Utilities.AI;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private readonly Timer _directionTimer = new Timer(targetOneShot: false);
       private const int MOVE_SPEED = 5;
       private Agent _agent;
       private Queue<PathNode> _nodeQueue = new Queue<PathNode>();

       [SerializeField] private Transform _playerTransform;

       protected override void Awake() {
           base.Awake();
           _agent = GetComponent<Agent>();
       }

       private void Start() {
           _directionTimer.Begin();
           _nodeQueue = ShortestPathToPlayer();
       }

       private Queue<PathNode> ShortestPathToPlayer() {
           return _agent.ShortestPathBetween(Vector2Int.RoundToInt(_transform.position), Vector2Int.RoundToInt(_playerTransform.position));
       }

       private void FixedUpdate() {
           Vector2 nextPosition = _nodeQueue.Peek().position;
           motion = nextPosition;
           _rigidBody.velocity = motion;
           if ((Vector2) _transform.position == nextPosition) {
               _nodeQueue.Dequeue();
           }
       }
       // private static Vector2Int GetRandomCardinal()
       // {
       //     return MOVE_SPEED * new Vector2Int(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
       // }
       
       
       
    }
}