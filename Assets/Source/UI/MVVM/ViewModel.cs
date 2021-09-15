using System;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class ViewModel<TState> : MonoBehaviour
    {
        [SerializeField]
        private TState state;
        
        public readonly Emitter<TState> emitter = new Emitter<TState>();
        public readonly Observer<TState> observer;

        protected ViewModel()
        {
            this.observer = new Observer<TState>(OnStateChangeFromView);
        }

        protected abstract void OnStateChangeFromView(TState newState);

        public void SetState(TState targetState)
        {
            this.state = targetState;
            emitter.EmitNotification(state);
        }
        
        public void SetState(Action<TState> stateAction)
        {
            stateAction.Invoke(state);
            emitter.EmitNotification(state);
        }

        public TState GetState()
        {
            return state;
        }
    }
}