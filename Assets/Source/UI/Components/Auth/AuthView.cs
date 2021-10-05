using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.Auth
{
    public class AuthView : View
    {
        [Header("Auth Bindings")]
        public ReactiveProperty<string> emailBinding = new ReactiveProperty<string>();
        public ReactiveProperty<string> passwordBinding = new ReactiveProperty<string>();
        

        [SerializeField]
        private Event loginBinding = new Event();
        [SerializeField]
        private Event registerBinding = new Event();
        [SerializeField]
        private Event signOutBinding = new Event();

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
