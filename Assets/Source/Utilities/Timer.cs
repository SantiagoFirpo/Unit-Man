using UnityEngine;

namespace FirpyTime {
    public class Timer
{
    //Responsibility: Abstract model for finite timers inside other classes
    public delegate void TimerChange();
    public event TimerChange OnEnd;
    float waitTime;
    float delay;
    float currentTime = 0f;
    bool autoStart;
    bool oneShot;

    public bool paused;
    public void Update(float deltaTime) {
        if (!paused && currentTime < waitTime) {
            currentTime += Time.deltaTime;
        }
        else if (!paused && currentTime >= waitTime){
            if (oneShot) {
                //Timer ended but is oneShot
                paused = true;
                currentTime = Mathf.Round(currentTime);
                OnEnd?.Invoke();
            }
            else {
                //Timer ended and reset
                OnEnd?.Invoke();
                currentTime = 0f;
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
    public void Start() {
        currentTime = 0f;
        paused = !autoStart;
    }

    public Timer(float targetWaitTime = 1, float targetDelay = 0, bool targetAutoStart = false, bool targetOneShot = true) {
        waitTime = targetWaitTime;
        delay = targetDelay;
        autoStart = targetAutoStart;
        oneShot = targetOneShot;
        TimerManager.OnFrameUpdate += Update;
        TimerManager.Initialized += Start;
    }
     ~Timer() {
         TimerManager.OnFrameUpdate -= Update;
         TimerManager.Initialized -= Start;
    }
}
}
