using UnitMan.Source.Management.Firebase.Auth;

namespace UnitMan.Source.UI.MVVM
{
    public class MVVMComponent<TState, TView, TViewModel, TModel> 
        where TView : View<TState, TViewModel> 
        where TState : struct 
        where TModel : Model<TState, TViewModel> 
        where TViewModel : ViewModel<TState>, new()
    {
        private TView _view;
        private TViewModel _viewModel;
        private TModel _model;

        public MVVMComponent(TView view, TModel model, TState initialState)
        {
            _view = view;
            _viewModel = new TViewModel();
            _view.Initialize(_viewModel, initialState);
            _model = model;
            _model.ProvideViewModel(_viewModel);
        }
    }
}