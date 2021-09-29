using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LevelCell
{
    public class LevelCellView : View
    {
        [SerializeField]
        private OneWayBinding<string> levelIdBinding;

        [SerializeField]
        private OneWayBinding<string> authorNameBinding;

        [SerializeField]
        private OneWayBinding<string> levelNameBinding;

        [SerializeField]
        private OneWayBinding playBinding;

        [SerializeField]
        private OneWayBinding editBinding;
        
        [SerializeField]
        private OneWayBinding uploadBinding;


        public void OnLevelIdChanged(string newValue) => levelIdBinding.SetValue($"LEVEL ID: {newValue}");

        public void OnAuthorNameChanged(string newValue) => authorNameBinding.SetValue($"BY {newValue}");

        public void OnLevelNameChanged(string newValue) => levelNameBinding.SetValue(newValue);

        public void OnPlayButtonPressed() => playBinding.Call();
        
        public void OnEditButtonBinding() => editBinding.Call();
        
        public void OnUploadButtonPressed() => uploadBinding.Call();
    }
}
