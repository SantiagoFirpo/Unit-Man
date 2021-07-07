using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Utilities.TimeTracking {
    public class TimerManager : MonoBehaviour, IEmitter<float>
    {
        public static TimerManager Instance { get; private set; }
        private void Update() => EmitNotification(_listeningObservers, Time.deltaTime);

        private readonly List<ObserverSystem.IObserver<float>> _listeningObservers =
                    new List<ObserverSystem.IObserver<float>>(); 
        
        public void Attach(ObserverSystem.IObserver<float> observer) 
        {
            _listeningObservers.Add(observer);
        }

        public void EmitNotification(IEnumerable<ObserverSystem.IObserver<float>> observers, float deltaTime)
        {
            foreach (ObserverSystem.IObserver<float> observer in observers)
            {
                observer.OnNotified(this, Time.deltaTime);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public void Detach(ObserverSystem.IObserver<float> observer)
        {
            _listeningObservers.Remove(observer);
        }
    }
}