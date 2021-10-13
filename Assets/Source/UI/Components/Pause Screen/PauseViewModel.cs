using UnitMan.Source.Management.Session;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI.Components.Pause_Screen
{
    public class PauseViewModel : ViewModel
    {
        public void Resume()
        {
            SessionManagerSingle.Instance.ToggleUserPause();
            GameplayUIRouter.Instance.SetState(GameplayUIRoute.Gameplay);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public override void OnRendered()
        {
        }
    }
}
