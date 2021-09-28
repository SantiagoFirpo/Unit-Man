using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.SaveAs
{
    public class SaveAsView : MonoBehaviour
    {
        [SerializeField]
        private OneWayBinding saveButtonBinding = new OneWayBinding();

        [SerializeField]
        private OneWayBinding<string> levelNameBinding = new OneWayBinding<string>();
        public void OnSaveButton() => saveButtonBinding.Call();

        public void OnLevelNameChanged(string newValue) => levelNameBinding.SetValue(newValue);
    }
}
