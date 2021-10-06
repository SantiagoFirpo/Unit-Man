using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan.Source.UI.Components.Auth
{
    public class AuthViewModel : MonoBehaviour
    {
        [SerializeField]
        private string email;
        [SerializeField]
        private string password;

        [SerializeField]
        private ReactiveProperty<string> authStatusMessageBinding = new ReactiveProperty<string>();

        private Observer<FirebaseAuthManager.AuthStatus> _authObserver;
        private Timer _redirectDelayTimer;
        
        [SerializeField]
        private ReactiveEvent clearFormsBinding = new ReactiveEvent();

        private void Awake()
        {
            _authObserver = new Observer<FirebaseAuthManager.AuthStatus>(OnAuthChanged);
        }
        
        private void Start()
        {
            FirebaseAuthManager.Instance.authStateChangedObservable.Attach(_authObserver);
            _redirectDelayTimer = new Timer(2f, false, true);
            _redirectDelayTimer.OnEnd += RedirectDelayTimerOnOnEnd;
        }

        public void Quit()
        {
            Application.Quit();
        }

        private static void RedirectDelayTimerOnOnEnd()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }

        private void OnAuthChanged(FirebaseAuthManager.AuthStatus authStatus)
        {
            authStatusMessageBinding.SetValue(AuthStatusToMessage(authStatus));
            switch (authStatus)
            {
                case FirebaseAuthManager.AuthStatus.LoginSuccessful:
                    _redirectDelayTimer.Start();
                    break;
                case FirebaseAuthManager.AuthStatus.SignedOut:
                    clearFormsBinding.Call();
                    break;
            }
        }
        
        public void OnEmailChanged(string newEmail)
        {
            this.email = newEmail;
        }

        public void OnPasswordChanged(string newPassword)
        {
            this.password = newPassword;
        }
        
        public void OnLoginPressed()
        {
            FirebaseAuthManager.Instance.TryLoginUser(email, password);
        }
        
        public void OnRegisterPressed()
        {
            FirebaseAuthManager.Instance.TryRegisterUser(email, password);
        }

        public void OnSignOutPressed()
        {
            FirebaseAuthManager.Instance.SignOutUser();
        }
        
        public static string AuthStatusToMessage(FirebaseAuthManager.AuthStatus authStatus)
        {
            return authStatus switch
            {
                FirebaseAuthManager.AuthStatus.LoggingIn => "LOGGING IN",
                FirebaseAuthManager.AuthStatus.Registering => "REGISTERING",
                FirebaseAuthManager.AuthStatus.WaitingForUser => "PLEASE REGISTER/LOGIN BELOW:",
                FirebaseAuthManager.AuthStatus.RegisterCanceled => "REGISTER WAS CANCELED",
                FirebaseAuthManager.AuthStatus.RegisterError => "REGISTER ERROR",
                FirebaseAuthManager.AuthStatus.RegisterSuccessful => $"REGISTER SUCCESSFUL! REGISTERED AS {FirebaseAuthManager.GetDisplayName()}",
                FirebaseAuthManager.AuthStatus.LoginCanceled => "LOGIN WAS CANCELED",
                FirebaseAuthManager.AuthStatus.LoginError => "LOGIN ERROR",
                FirebaseAuthManager.AuthStatus.LoginSuccessful => $"LOGIN SUCCESSFUL! LOGGED IN AS {FirebaseAuthManager.GetDisplayName()}",
                FirebaseAuthManager.AuthStatus.SignedOut => "SIGNED OUT",
                FirebaseAuthManager.AuthStatus.Empty => "",
                FirebaseAuthManager.AuthStatus.RegisterRequested => "REGISTERING",
                FirebaseAuthManager.AuthStatus.LoginRequested => "LOGGING IN",
                FirebaseAuthManager.AuthStatus.SignOutRequested => "SIGNING OUT",
                _ => throw new ArgumentOutOfRangeException(nameof(authStatus), authStatus, null)
            };
        }
    }
}
