using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.ObserverSystem;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel<MainMenuState>
    {
        private Observer<FirebaseAuthManager.AuthStatus> _authObserver;

        protected override void InitializeState()
        {
            _authObserver = new Observer<FirebaseAuthManager.AuthStatus>(OnAuthChange);
            OverwriteState(new MainMenuState());
        }

        private void OnAuthChange(FirebaseAuthManager.AuthStatus authStatus)
        {
            ChangeState(SetAuthStatus);
            
            void SetAuthStatus(MainMenuState state)
            {
                state._authStatus = authStatus;
            }
        }

        public override void ChangeState(Action<MainMenuState> stateAction)
        {
            base.ChangeState(stateAction);
            EmitNewState();
        }

        public void Login(AuthFormData formData)
        {
            FirebaseAuthManager.Instance.TryLoginUser(formData.email, formData.password);
        }
        
        public void Register(AuthFormData formData)
        {
            FirebaseAuthManager.Instance.TryRegisterUser(formData.email, formData.password);
        }

        public void SignOut()
        {
            FirebaseAuthManager.Instance.SignOutUser();
        }


        private void Start()
        {
            FirebaseAuthManager.Instance.authStateChangedEmitter.Attach(_authObserver);
        }

        protected override void OnStateChange(MainMenuState newState)
        {
            base.OnStateChange(newState);
        }
    }
}