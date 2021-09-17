using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.ObserverSystem;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {

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
            // FirebaseAuthManager.Instance.authStateChangedEmitter.Attach(_authObserver);
        }
    }
}