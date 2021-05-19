using System;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.Source
{
    public class Enemy : Actor
    {
       private readonly Timer _directionTimer = new Timer(targetOneShot: false);
       private const float MOVE_SPEED = 5f;

       protected override void Awake()
       {
           base.Awake();
           _directionTimer.OnEnd += ChangeDirection;
       }

       private void ChangeDirection()
       {
           base.rigidBody.velocity = MOVE_SPEED *
                                     new Vector2(Mathf.Round(UnityEngine.Random.Range(0f,
                                             1f)),
                                         Mathf.Round(UnityEngine.Random.Range(0f,
                                             1f)));
       }

       private void Start()
       {
           _directionTimer.Begin();
           
       }
    }
}


