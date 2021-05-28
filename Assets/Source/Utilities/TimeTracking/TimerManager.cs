using System;
using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class TimerManager : MonoBehaviour
    {
        public static event Action<float> OnFrameUpdate;
        public static event Action Initialized;
        private void Update() => OnFrameUpdate?.Invoke(Time.deltaTime);

        private void Start() => Initialized?.Invoke();
    }
}