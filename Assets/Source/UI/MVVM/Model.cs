using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class Model<TState, TViewModel> : MonoBehaviour
        where TViewModel : ViewModel<TState>
        where TState : struct
    {
        protected TViewModel viewModel;

        public void ProvideViewModel(TViewModel targetViewModel)
        {
            viewModel = targetViewModel;
        }
    }
}