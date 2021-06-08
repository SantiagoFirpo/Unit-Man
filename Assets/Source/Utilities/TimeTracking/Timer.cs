using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public delegate void TimerChange();
    public event System.Action OnEnd;
    private readonly float _waitTime;
    private readonly float _delay;
    private float _currentTime;
    private readonly bool _autoStart;
    private readonly bool _oneShot;

    public bool paused;

    private void Update(float deltaTime) {
        switch (paused) {
            case false when _currentTime < _waitTime:
                _currentTime += Time.deltaTime;
                break;
            case false when _currentTime >= _waitTime: {
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

                break;
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

    public Timer(float waitTime = 1, float delay = 0, bool autoStart = false, bool isOneShot = true) {
        _waitTime = waitTime;
        _delay = delay;
        _autoStart = autoStart;
        _oneShot = isOneShot;
        TimerManager.OnFrameUpdate += Update;
        TimerManager.Initialized += Setup;
    }
     ~Timer() {
         TimerManager.OnFrameUpdate -= Update;
         TimerManager.Initialized -= Setup;
    }
}
}
