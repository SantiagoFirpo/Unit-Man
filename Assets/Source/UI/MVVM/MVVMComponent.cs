namespace UnitMan.Source.UI.MVVM
{
    public class MVVMComponent<TState, TView, TViewModel, TModel> where TView : View<TState, TViewModel> where TModel : Model where TViewModel : ViewModel<TState>, new()
    {
        private TView _view;
        private TViewModel _viewModel;
        private TModel _model;

        public MVVMComponent(TView view, TModel model)
        {
            _view = view;
            _viewModel = new TViewModel();
            _view.Initialize(_viewModel);
            _model = model;
        }
    }
}