using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class Model<TState, TViewModel> : MonoBehaviour where TViewModel : ViewModel<TState>
    {
        private TViewModel _viewModel;
    }
}