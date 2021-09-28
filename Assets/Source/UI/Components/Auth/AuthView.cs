using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.Auth
{
    public class AuthView : View
    {
        [Header("Auth Bindings")]
        public OneWayBinding<string> emailBinding = new OneWayBinding<string>();
        public OneWayBinding<string> passwordBinding = new OneWayBinding<string>();
        

        [SerializeField]
        private OneWayBinding loginBinding = new OneWayBinding();
        [SerializeField]
        private OneWayBinding registerBinding = new OneWayBinding();
        [SerializeField]
        private OneWayBinding signOutBinding = new OneWayBinding();

        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        public void OnEmailChanged(string email) => emailBinding.SetValue(email);

        public void OnPasswordChanged(string password) => passwordBinding.SetValue(password);

        public void OnLoginButtonPressed() => loginBinding.Call();

        public void OnRegisterButtonPressed() => registerBinding.Call();

        public void OnSignOutButtonPressed() => signOutBinding.Call();


        public void ClearForms()
        {
            emailField.SetTextWithoutNotify("");
            passwordField.SetTextWithoutNotify("");
        }
    }
}
