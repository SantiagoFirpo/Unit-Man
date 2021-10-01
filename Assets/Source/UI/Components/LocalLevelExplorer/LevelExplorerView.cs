using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LevelExplorerView : View
    {
        [SerializeField]
        private OneWayBinding mainMenuBinding;

        [SerializeField]
        private OneWayBinding refreshBinding;

        public void OnMainMenuPressed() => mainMenuBinding.Call();
        
        public void OnRefreshPressed() => refreshBinding.Call();
    }
}
