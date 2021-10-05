using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnityEngine;

namespace UnitMan.Source.UI.Components.SaveAs
{
    public class SaveAsViewModel : ViewModel
    {
        private string _levelName;
        [SerializeField]
        private ReactiveProperty<string> saveBinding;

        public void OnSaveButtonPressed()
        {
            saveBinding.SetValue(_levelName);
            LevelEditorRouter.Instance.SetState(LevelEditorRoute.Editor);
        }

        public void OnLevelNameChanged(string newValue)
        {
            _levelName = newValue;
        }

        public override void OnRendered()
        {
        }
    }
}