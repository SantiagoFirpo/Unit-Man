using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source {
    public class Enemy : Actor {
       private readonly Timer _directionTimer = new Timer(targetOneShot: false);
       private const int MOVE_SPEED = 5;

       protected override void Awake() {
           base.Awake();
           _directionTimer.OnEnd += ChangeDirection;
       }

       private void ChangeDirection()
       {
           Debug.Log(_directionTimer.paused);
           motion = GetRandomCardinal();
           while (motion == Vector2.zero)
           {
               motion = GetRandomCardinal();
           }
       }
       
       private void Start() {
           _directionTimer.Begin();
       }

       private void FixedUpdate()
       {
           rigidBody.velocity = motion;
           if (rigidBody.velocity == Vector2.zero) {
               rigidBody.velocity =  (Vector2) (GetRandomCardinal());
           }
       }
       
       private static Vector2Int GetRandomCardinal()
       {
           return MOVE_SPEED * new Vector2Int(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
       }
       
    }
}


