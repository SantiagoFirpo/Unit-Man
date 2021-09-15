using System;
using UnitMan.Source.UI.Components.Text;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM

{
    [Serializable]
    public abstract class View<TState> : MonoBehaviour
    {
        private Observer<TState> _observer;
        private readonly Emitter<TState> _emitter = new Emitter<TState>();
        // [SerializeField]
        // protected TextViewModel textViewModel;
        public ViewModel<TState> viewModel;

        protected abstract void Render(TState state);

        private void Awake()
        {
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
            // Render(initialState);
        }
    }
}