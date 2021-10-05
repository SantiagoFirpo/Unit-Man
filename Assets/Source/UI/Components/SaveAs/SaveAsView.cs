using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.SaveAs
{
    public class SaveAsView : MonoBehaviour
    {
        [SerializeField]
        private ReactiveEvent saveButtonBinding = new ReactiveEvent();

        [SerializeField]
        private ReactiveProperty<string> levelNameBinding = new ReactiveProperty<string>();
        public void OnSaveButton() => saveButtonBinding.Call();

        public void OnLevelNameChanged(string newValue) => levelNameBinding.SetValue(newValue);
    }
}
