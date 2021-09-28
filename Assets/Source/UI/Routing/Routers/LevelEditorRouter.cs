using System;
using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Routing.Routers
{
    public class LevelEditorRouter : Router<LevelEditorRoute>
    {
        public static LevelEditorRouter Instance { get; private set; }

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
    }

    public enum LevelEditorRoute
    {
        Undefined, Editor, SaveAs
    }
}
