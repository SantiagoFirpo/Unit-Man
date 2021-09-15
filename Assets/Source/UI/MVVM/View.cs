using System;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM

{
    [Serializable]
    public abstract class View<TState> : MonoBehaviour
    {
        private Observer<TState> _observer;
        
        [SerializeField]
        protected Emitter<TState> _emitter;
        // [SerializeField]
        // protected TextViewModel textViewModel;
        public ViewModel<TState> viewModel;

        protected abstract void Render(TState state);

        private void Awake()
        {
            _emitter = new Emitter<TState>();
            _observer = new Observer<TState>(Render);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            viewModel.emitter.Attach(_observer);
            _emitter.Attach(viewModel.observer);
            Debug.Log($"{gameObject.name} is now coupled to its ViewModel {viewModel.gameObject.name}");
            // Render(initialState);
        }
    }
}