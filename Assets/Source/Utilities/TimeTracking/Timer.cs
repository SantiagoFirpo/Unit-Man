using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public delegate void TimerChange();
    public event System.Action OnEnd;
    public readonly float waitTime;
    private readonly float _delay;
    public float currentTime;
    private readonly bool _autoStart;
    private readonly bool _oneTime;

    public bool active;

    private void Update(float deltaTime) {
        if (!active) return;
        if (currentTime < waitTime) {
            currentTime += Time.deltaTime;
        }
        else {
            OnEnd?.Invoke();
            if (_oneTime) {
                //Timer ended but is oneTime
                active = false;
                currentTime = Mathf.Round(currentTime);
            }
            else {
                //Timer ended and reset
                currentTime = 0f;
                active = true;
            }
        }

    }
    public void Begin() {
        currentTime = 0f;
        active = true;
    }
    public void Stop() {
        currentTime = 0f;
        active = false;
    }

    private void Setup() {
        currentTime = 0f;
        active = _autoStart;
    }

    public Timer(float waitTime = 1, float delay = 0, bool autoStart = false, bool oneTime = true) {
        this.waitTime = waitTime;
        _delay = delay;
        _autoStart = autoStart;
        _oneTime = oneTime;
        TimerManager.OnFrameUpdate += Update;
        TimerManager.Initialized += Setup;
    }
     ~Timer() {
         TimerManager.OnFrameUpdate -= Update;
         TimerManager.Initialized -= Setup;
    }
}
}
