using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.Auth
{
    public class AuthView : View
    {
        [Header("Auth Bindings")]
        public Reactive<string> emailBinding = new Reactive<string>();
        public Reactive<string> passwordBinding = new Reactive<string>();
        

        [SerializeField]
        private ReactiveEvent loginBinding = new ReactiveEvent();
        [SerializeField]
        private ReactiveEvent registerBinding = new ReactiveEvent();
        [SerializeField]
        private ReactiveEvent signOutBinding = new ReactiveEvent();

        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private ReactiveEvent quitEvent;

        public void OnEmailChanged(string email) => emailBinding.SetValue(email);

        public void OnPasswordChanged(string password) => passwordBinding.SetValue(password);

        public void OnLoginButtonPressed() => loginBinding.Call();

        public void OnRegisterButtonPressed() => registerBinding.Call();

        public void OnSignOutButtonPressed() => signOutBinding.Call();

        public void QuitPressed() => quitEvent.Call();


        public void ClearForms()
        {
            emailField.SetTextWithoutNotify("");
            passwordField.SetTextWithoutNotify("");
        }
    }
}
