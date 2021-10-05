using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.SaveAs
{
    public class SaveAsView : MonoBehaviour
    {
        [SerializeField]
        private Event saveButtonBinding = new Event();

        [SerializeField]
        private ReactiveProperty<string> levelNameBinding = new ReactiveProperty<string>();
        public void OnSaveButton() => saveButtonBinding.Call();

        public void OnLevelNameChanged(string newValue) => levelNameBinding.SetValue(newValue);
    }
}
