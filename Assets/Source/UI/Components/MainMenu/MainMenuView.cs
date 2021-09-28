using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        [Header("App Navigation Button Bindings")]

        [SerializeField]
        private OneWayBinding quitButtonBinding;

        // [SerializeField]
        // private Binding leaderboardButtonBinding;

        [SerializeField]
        private OneWayBinding<string> authStatusMessage;
        
        [SerializeField]
        private OneWayBinding playButtonBinding;

        [SerializeField]
        private OneWayBinding levelEditorButtonBinding;

        [Header("Custom Level Input Bindings")]
        [SerializeField]
        private OneWayBinding<string> levelIdBinding;

        [SerializeField]
        private OneWayBinding signOutBinding;
        
        [SerializeField]
        private OneWayBinding myLevelsBinding = new OneWayBinding();

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