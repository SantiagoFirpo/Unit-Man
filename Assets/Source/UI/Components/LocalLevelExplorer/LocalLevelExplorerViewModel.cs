using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routers;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LocalLevelExplorerViewModel : ViewModel
    {
        public void OnMainMenuPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }
    }
}
