using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LevelExplorerView : View
    {
        [SerializeField]
        private Event mainMenuBinding;

        [SerializeField]
        private Event refreshBinding;

        public void OnMainMenuPressed() => mainMenuBinding.Call();
        
        public void OnRefreshPressed() => refreshBinding.Call();
    }
}
