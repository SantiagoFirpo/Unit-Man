using UnitMan.Source.Management.Firebase.Auth;

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

        protected override void Start()
        {
            base.Start();
            if (FirebaseAuthManager.Instance.auth?.CurrentUser == null) return;
            SetState(MainMenuRoute.Home);
        }

        public enum MainMenuRoute
        {
            Undefined, Auth, Home, LocalLevelExplorer
        }
    }
}