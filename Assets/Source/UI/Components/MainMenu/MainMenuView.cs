using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        [Header("App Navigation Button Bindings")]

        [SerializeField]
        private Event quitButtonBinding;

        // [SerializeField]
        // private Binding leaderboardButtonBinding;

        [SerializeField]
        private ReactiveProperty<string> authStatusMessage;
        
        [SerializeField]
        private Event playButtonBinding;

        [SerializeField]
        private Event levelEditorButtonBinding;

        [Header("Custom Level Input Bindings")]
        [SerializeField]
        private ReactiveProperty<string> levelIdBinding;

        [SerializeField]
        private Event signOutBinding;
        
        [SerializeField]
        private Event myLevelsBinding = new Event();

        public void OnLevelIdChanged(string newValue) => levelIdBinding.SetValue(newValue);

        public void OnAuthMessageChanged(string newValue) => authStatusMessage.SetValue(newValue);

        public void OnLevelEditorButtonPressed() => levelEditorButtonBinding.Call();

        public void OnQuitButtonPressed() => quitButtonBinding.Call();

        public void OnPlayButtonPressed() => playButtonBinding.Call();
        
        public void OnSignOutButtonPressed() => signOutBinding.Call();
        
        public void OnMyLevelsButtonPressed() => myLevelsBinding.Call();


        // public void OnLeaderboardButtonPressed() => leaderboardButtonBinding.Call();
    }
}