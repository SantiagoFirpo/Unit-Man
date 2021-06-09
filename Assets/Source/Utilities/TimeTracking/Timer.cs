using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public delegate void TimerChange();
    public event System.Action OnEnd;
    private readonly float _waitTime;
    private readonly float _delay;
    public float currentTime;
    private readonly bool _autoStart;
    private readonly bool _oneShot;

    public bool paused;

    private void Update(float deltaTime) {
        switch (paused) {
            case false when currentTime < _waitTime:
                currentTime += Time.deltaTime;
                break;
            case false when currentTime >= _waitTime: {
                OnEnd?.Invoke();
                if (_oneShot) {
                    //Timer ended but is oneShot
                    paused = true;
                    currentTime = Mathf.Round(currentTime);
                
                }
                else {
                    //Timer ended and reset
                    currentTime = 0f;
                    paused = false;
                }

                break;
            }
        }
    }
    public void Begin() {
        currentTime = 0f;
        paused = false;
    }
    public void Stop() {
        currentTime = 0f;
        paused = true;
    }

    private void Setup() {
        currentTime = 0f;
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
