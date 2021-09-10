using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVC
{
    public abstract class View<TState> : MonoBehaviour
    {
        public abstract void RenderView(TState props);
        private ViewModel<TState> _viewModel;
        private Observer<TState> _observer;
        private Emitter<TState> _emitter;
        
        public void Initialize(ViewModel<TState> viewModel)
        {
            _viewModel = viewModel;
            _observer = new Observer<TState>(OnViewModelStateChange);
            _viewModel.emitter.Attach(_observer);
        }

        private void OnViewModelStateChange(Emitter<TState> source, TState newState)
        {
            RenderView(newState);
        }
    }
}