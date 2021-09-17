using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [SerializeField]
        private string email;
        [SerializeField]
        private string password;

        [SerializeField]
        private Binding<FirebaseAuthManager.AuthStatus> authStatusBinding;

        private Observer<FirebaseAuthManager.AuthStatus> _authObserver;

        private void Awake()
        {
            _authObserver = new Observer<FirebaseAuthManager.AuthStatus>(OnAuthChanged);
        }

        private void OnAuthChanged(FirebaseAuthManager.AuthStatus authStatus)
        {
            authStatusBinding.SetValue(authStatus);
        }

        public void OnEmailChanged(string newEmail)
        {
            this.email = newEmail;
        }

        public void OnPasswordChanged(string newPassword)
        {
            this.password = newPassword;
        }

        public void Login()
        {
            FirebaseAuthManager.Instance.TryLoginUser(email, password);
        }
        
        public void Register()
        {
            FirebaseAuthManager.Instance.TryRegisterUser(email, password);
        }

        public void SignOut()
        {
            FirebaseAuthManager.Instance.SignOutUser();
        }


        private void Start()
        {
            FirebaseAuthManager.Instance.authStateChangedEmitter.Attach(_authObserver);
        }
    }
}