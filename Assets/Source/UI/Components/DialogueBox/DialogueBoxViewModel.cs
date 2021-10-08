using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.DialogueBox
{
    public class DialogueBoxViewModel : ViewModel
    {
        [SerializeField]
        protected Reactive<string> message;
        public override void OnRendered()
        {
        }
    }
}