using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using UnityEngine.Events;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View
    {
        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TMP_Text authMessage;

        [SerializeField]
        private AuthEvent loginButtonEvent;
        
        [SerializeField]
        private AuthEvent registerButtonEvent;

        [SerializeField]
        private UnityEvent signOutEvent;

        public void OnLoginButtonPressed()
        {
            loginButtonEvent.Invoke(new AuthFormData(emailField.text, passwordField.text));
        }

        public void OnRegisterButtonPressed()
        {
            registerButtonEvent.Invoke(new AuthFormData(emailField.text, passwordField.text));
        }

        public void OnSignOutButtonPressed()
        {
            signOutEvent.Invoke();
        }

        public void SetAuthToSignedOut(MainMenuState state)
        {
            state._email = "";
            state._password = "";
            state._authStatus = FirebaseAuthManager.AuthStatus.SignOutRequested;
        }

        protected override void Render()
        {
        }
    }
}