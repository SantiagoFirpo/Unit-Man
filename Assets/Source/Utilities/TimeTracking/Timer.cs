﻿using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public event System.Action OnEnd;
    public readonly float waitTime;
    public float currentTime;
    private readonly bool _autoStart;
    private readonly bool _oneTime;

    public bool Active { get; private set;}

    private void Update(float deltaTime) {
        if (!Active) return;
        if (currentTime < waitTime) {
            currentTime += Time.deltaTime;
        }
        else {
            OnEnd?.Invoke();
            if (_oneTime) {
                //Timer ended but is oneTime
                Active = false;
                currentTime = Mathf.Round(currentTime);
            }
            else {
                //Timer ended and reset
                currentTime = 0f;
                Active = true;
            }
        }

    }
    public void Start() {
        currentTime = 0f;
        Active = true;
    }
    public void Stop() {
        currentTime = 0f;
        Active = false;
    }

    private void Setup() {
        currentTime = 0f;
        Active = _autoStart;
    }

    public Timer(float waitTime, bool autoStart, bool oneTime, float delay = 0)  { // prev was waitTime = 1, autoStart = false, oneTime = true
        this.waitTime = waitTime;
        _autoStart = autoStart;
        _oneTime = oneTime;
        TimerManager.OnFrameUpdate += Update;
        TimerManager.Initialized += Setup;
    }
     ~Timer() {
         TimerManager.OnFrameUpdate -= Update;
         TimerManager.Initialized -= Setup;
    }
     
     //TODO: make TimerManager Singleton
     //TODO: manage Timer Manager with SessionManager
     //observer/dispose
     
     //https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-5.0//gameprogrammingpatterns.com/observer.html
}
}
