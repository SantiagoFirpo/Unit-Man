using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.Text;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel<MainMenuState>
    {
        [SerializeField]
        private TextViewModel authMessage;
        protected override void OnStateChangeFromView(MainMenuState newState)
        {
            SetState(newState);
            authMessage.SetState(MainMenuController.AuthStatusToMessage(newState._authStatus));
            switch (newState._authStatus)
            {
                case FirebaseAuthManager.AuthStatus.LoggingIn:
                    FirebaseAuthManager.Instance.TryLoginUser(newState._email, newState._password);
                    break;
                case FirebaseAuthManager.AuthStatus.Registering:
                    FirebaseAuthManager.Instance.TryRegisterUser(newState._email, newState._password);
                    break;
                case FirebaseAuthManager.AuthStatus.WaitingForUser:
                    break;
                case FirebaseAuthManager.AuthStatus.RegisterCanceled:
                    break;
                case FirebaseAuthManager.AuthStatus.RegisterError:
                    break;
                case FirebaseAuthManager.AuthStatus.RegisterSuccessful:
                    break;
                case FirebaseAuthManager.AuthStatus.LoginCanceled:
                    break;
                case FirebaseAuthManager.AuthStatus.LoginError:
                    break;
                case FirebaseAuthManager.AuthStatus.LoginSuccessful:
                    break;
                case FirebaseAuthManager.AuthStatus.SignedOut:
                    break;
                case FirebaseAuthManager.AuthStatus.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}