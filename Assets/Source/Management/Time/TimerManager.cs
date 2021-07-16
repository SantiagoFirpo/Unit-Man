using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Management.Time {
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }
        public readonly Emitter<float> timeEmitter = new Emitter<float>();
        private void Update() => timeEmitter.EmitNotification(UnityEngine.Time.deltaTime);


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
            
        }
    }
}