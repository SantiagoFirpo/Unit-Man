using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LocalLevelExplorerView : View
    {
        [SerializeField]
        private OneWayBinding mainMenuBinding;

        public void OnMainMenuPressed() => mainMenuBinding.Call();
    }
}
