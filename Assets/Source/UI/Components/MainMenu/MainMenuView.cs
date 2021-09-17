using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using UnityEngine.Events;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        public Binding<string> emailBinding;
        public Binding<string> passwordBinding;

        [SerializeField]
        private Binding loginBinding;
        [SerializeField]
        private Binding registerBinding;
        [SerializeField]
        private Binding signOutBinding;
        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TMP_Text authMessage;

        public void OnEmailChanged(string email)
        {
            emailBinding.SetValue(email);
        }

        public void OnAuthStatusChanged(FirebaseAuthManager.AuthStatus authStatus)
        {
            authMessage.SetText(MainMenuController.AuthStatusToMessage(authStatus));
        }
        
        public void OnPasswordChanged(string password)
        {
            passwordBinding.SetValue(password);
        }

        public void OnLoginButtonPressed()
        {
            loginBinding.Call();
        }

        public void OnRegisterButtonPressed()
        {
            registerBinding.Call();
        }

        public void OnSignOutButtonPressed()
        {
            signOutBinding.Call();
        }

        protected override void Render()
        {
        }
    }
}