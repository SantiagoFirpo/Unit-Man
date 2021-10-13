using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        [Header("App Navigation Button Bindings")]

        [SerializeField]
        private ReactiveEvent quitButtonBinding;

        // [SerializeField]
        // private Binding leaderboardButtonBinding;

        [SerializeField]
        private Reactive<string> authStatusMessage;
        
        [SerializeField]
        private ReactiveEvent playButtonBinding;

        [SerializeField]
        private ReactiveEvent levelEditorButtonBinding;

        [Header("Custom Level Input Bindings")]
        [SerializeField]
        private Reactive<string> levelIdBinding;

        [SerializeField]
        private ReactiveEvent signOutBinding;
        
        [SerializeField]
        private ReactiveEvent myLevelsBinding = new ReactiveEvent();

        [SerializeField]
        private ReactiveEvent deleteAccountEvent;

        public void OnLevelIdChanged(string newValue) => levelIdBinding.SetValue(newValue);

        public void OnAuthMessageChanged(string newValue) => authStatusMessage.SetValue(newValue);

        public void OnLevelEditorButtonPressed() => levelEditorButtonBinding.Call();

        public void OnQuitButtonPressed() => quitButtonBinding.Call();

        public void OnPlayButtonPressed() => playButtonBinding.Call();
        
        public void OnSignOutButtonPressed() => signOutBinding.Call();
        
        public void OnMyLevelsButtonPressed() => myLevelsBinding.Call();

        public void DeleteButtonPressed() => deleteAccountEvent.Call();


        // public void OnLeaderboardButtonPressed() => leaderboardButtonBinding.Call();
    }
}