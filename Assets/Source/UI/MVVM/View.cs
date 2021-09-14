using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class View<TState, TViewModel> : MonoBehaviour where TViewModel : ViewModel<TState>
    {
        private Observer<TState> _observer;
        private readonly Emitter<TState> _emitter = new Emitter<TState>();
        private TViewModel _viewModel;

        protected abstract void Render(TState state);

        public void Initialize(TViewModel viewModel)
        {
            _observer = new Observer<TState>(Render);
            _viewModel = viewModel;
            _viewModel.emitter.Attach(_observer);
            _emitter.Attach(_viewModel.observer);
        }
    }
}