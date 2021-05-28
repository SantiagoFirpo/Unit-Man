using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public delegate void TimerChange();
    public event System.Action OnEnd;
    private readonly float _waitTime;
    private readonly float _delay;
    private float _currentTime = 0f;
    private readonly bool _autoStart;
    private readonly bool _oneShot;

    public bool paused;

    private void Update(float deltaTime) {
        if (!paused && _currentTime < _waitTime) {
            _currentTime += Time.deltaTime;
        }
        else if (!paused && _currentTime >= _waitTime){
            OnEnd?.Invoke();
            if (_oneShot) {
                //Timer ended but is oneShot
                paused = true;
                _currentTime = Mathf.Round(_currentTime);
                
            }
            else {
                //Timer ended and reset
                _currentTime = 0f;
                paused = false;
            }
        }
            
    }
    public void Begin() {
        _currentTime = 0f;
        paused = false;
    }
    public void Stop() {
        _currentTime = 0f;
        paused = true;
    }

    private void Setup() {
        _currentTime = 0f;
        paused = !_autoStart;
    }

    public Timer(float targetWaitTime = 1, float targetDelay = 0, bool targetAutoStart = false, bool targetOneShot = true) {
        _waitTime = targetWaitTime;
        _delay = targetDelay;
        _autoStart = targetAutoStart;
        _oneShot = targetOneShot;
        TimerManager.OnFrameUpdate += Update;
        TimerManager.Initialized += Setup;
    }
     ~Timer() {
         TimerManager.OnFrameUpdate -= Update;
         TimerManager.Initialized -= Setup;
    }
}
}
