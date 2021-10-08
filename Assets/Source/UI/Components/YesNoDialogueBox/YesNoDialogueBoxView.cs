using UnitMan.Source.UI.Components.DialogueBox;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.YesNoDialogueBox
{
    public class YesNoDialogueBoxView : DialogueBoxView
    {
        [SerializeField]
        private ReactiveEvent yesEvent;
        
        [SerializeField]
        private ReactiveEvent noEvent;

        public void OnYesPressed() => yesEvent.Call();
        
        public void OnNoPressed() => noEvent.Call();
    }
}