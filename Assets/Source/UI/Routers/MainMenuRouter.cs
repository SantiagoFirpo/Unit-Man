using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Routers
{
    public class MainMenuRouter : Router<MainMenuRouter.MainMenuRoute>
    {
        public static MainMenuRouter Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public enum MainMenuRoute
        {
            Undefined, Auth, Home, LocalLevelExplorer
        }
    }
}