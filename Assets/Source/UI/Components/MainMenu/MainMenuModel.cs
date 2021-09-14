using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuModel<TState, TViewModel> : Model<TState, TViewModel>
        where TViewModel : ViewModel<TState>
        where TState : struct
    {
    }
}