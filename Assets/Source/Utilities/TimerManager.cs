using UnityEngine;

namespace FirpyTime {
    public class TimerManager : MonoBehaviour
    {
        public delegate void FrameUpdate(float deltaTime);
        public static event FrameUpdate OnFrameUpdate;
        public delegate void Init();
        public static event System.Action Initialized;
        void Update() => OnFrameUpdate?.Invoke(Time.deltaTime);

        void Start() => Initialized?.Invoke();
    }
}