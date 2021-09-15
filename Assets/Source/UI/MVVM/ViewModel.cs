using System;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class ViewModel<TState> : MonoBehaviour
    {
        [SerializeField]
        private TState state;
        
        [SerializeField]
        public Emitter<TState> emitter;
        public Observer<TState> observer;

        private void Awake()
        {
            emitter = new Emitter<TState>();
            InitializeState();
        }

        protected virtual void OnStateChange(TState stateFromView)
        {
            
        }

        protected abstract void InitializeState();

        public void OverwriteState(TState targetState)
        {
            state = targetState;
            emitter.EmitNotification(state);
        }
        
        public virtual void ChangeState(Action<TState> stateAction)
        {
            stateAction.Invoke(state);
            OnStateChange(state);
        }

        protected void EmitNewState()
        {
            emitter.EmitNotification(state);
        }

        public TState GetState()
        {
            return state;
        }
    }
}