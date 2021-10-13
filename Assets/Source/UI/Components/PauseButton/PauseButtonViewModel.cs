using UnitMan.Source.Management.Session;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;

namespace UnitMan.Source.UI.Components.PauseButton
{
    public class PauseButtonViewModel : ViewModel
    {
        public void Pause()
        {
            SessionManagerSingle.Instance.ToggleUserPause();
            GameplayUIRouter.Instance.SetState(GameplayUIRoute.PauseScreen);
        }

        public override void OnRendered()
        {
            
        }
    }
}
