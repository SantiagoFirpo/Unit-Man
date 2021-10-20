using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Management.Time {
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }
        public readonly Observable<float> timeObservable = new Observable<float>();
        private void Update() => timeObservable.EmitNotification(UnityEngine.Time.deltaTime);


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
    }
}