using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Routing.Routers
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

        private void OnEnable()
        {
            if (FirebaseAuthManager.Instance.auth == null) return;
            SetState(MainMenuRoute.Home);
        }

        public enum MainMenuRoute
        {
            Undefined, Auth, Home, LocalLevelExplorer
        }
    }
}