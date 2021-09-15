using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View<MainMenuState>
    {
        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        // private TextMeshPro authMessage;

        public void OnLoginButtonPressed()
        {
            viewModel.SetState(SendLoginRequestData);
        }

        private void SendLoginRequestData(MainMenuState state)
        {
            state._email = emailField.text;
            state._password = passwordField.text;
            state._authStatus = FirebaseAuthManager.AuthStatus.LoggingIn;
        }
        
        private void SendRegisterRequestData(MainMenuState state)
        {
            state._email = emailField.text;
            state._password = passwordField.text;
            state._authStatus = FirebaseAuthManager.AuthStatus.Registering;
        }

        public void OnRegisterButtonPressed()
        {
            viewModel.SetState(SendRegisterRequestData);
        }

        public void OnSignOutButtonPressed()
        {
            
        }
        protected override void Render(MainMenuState state)
        {
        }
    }
}