using System;

namespace UnitMan.Source.UI.Routing.Routers
{
    public class GameplayUIRouter : Router<GameplayUIRoute>
    {
        public static GameplayUIRouter Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;
        }
    }

    public enum GameplayUIRoute
    {
        Undefined, Gameplay, PauseScreen
    }
}