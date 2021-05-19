using System;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;

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
           base.rigidBody.velocity = GetRandomRoundedVector2();
       }
       
       private void Start() {
           _directionTimer.Begin();
       }

       private void Update() {
           if (base.rigidBody.velocity == Vector2.zero) {
               base.rigidBody.velocity = Vector2Int.Vector2(GetRandomRoundedVector2());
           }
       }
       
       private Vector2Int GetRandomRoundedVector2()
       {
           return MOVE_SPEED * new Vector2Int(
               GetRandomInt(-1, 1),
               GetRandomInt(-1, 1));
       }

       private static int GetRandomInt(int smallestInclusive, int biggestInclusive)
       {
           return Mathf.RoundToInt(
               UnityEngine.Random.Range(
                   smallestInclusive,
                   biggestInclusive));
       }
    }
}


