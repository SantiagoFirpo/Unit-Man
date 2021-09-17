using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        [Header("UI Components")]
        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TMP_InputField levelIdField;
        [Header("Auth Bindings")]
        public OneWayBinding<string> emailBinding;
        public OneWayBinding<string> passwordBinding;
        

        [SerializeField]
        private OneWayBinding loginBinding;
        [SerializeField]
        private OneWayBinding registerBinding;
        [SerializeField]
        private OneWayBinding signOutBinding;

        [SerializeField]
        private TMP_Text authMessage;

        [Header("App Navigation Button Bindings")]

        [SerializeField]
        private OneWayBinding quitButtonBinding;

        // [SerializeField]
        // private Binding leaderboardButtonBinding;
        
        [SerializeField]
        private OneWayBinding playButtonBinding;

        [SerializeField]
        private OneWayBinding levelEditorButtonBinding;

        [Header("Custom Level Input Bindings")]
        [SerializeField]
        private OneWayBinding<string> levelIdBinding;
        public void OnEmailChanged(string email)
        {
            emailBinding.SetValue(email);
        }

        public void OnAuthStatusChanged(FirebaseAuthManager.AuthStatus authStatus)
        {
            authMessage.SetText(MainMenuViewModel.AuthStatusToMessage(authStatus));
        }
        
        public void OnPasswordChanged(string password) => passwordBinding.SetValue(password);

        public void OnLoginButtonPressed() => loginBinding.Call();

        public void OnRegisterButtonPressed() => registerBinding.Call();

        public void OnSignOutButtonPressed() => signOutBinding.Call();

        public void OnLevelIdChanged() => levelIdBinding.SetValue(levelIdField.text);

        public void OnLevelEditorButtonPressed() => levelEditorButtonBinding.Call();

        public void OnQuitButtonPressed() => quitButtonBinding.Call();

        public void OnPlayButtonPressed() => playButtonBinding.Call();

        // public void OnLeaderboardButtonPressed() => leaderboardButtonBinding.Call();
    }
}