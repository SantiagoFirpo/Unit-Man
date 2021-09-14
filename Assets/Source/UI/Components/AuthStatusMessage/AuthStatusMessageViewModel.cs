using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.AuthStatusMessage
{
    public class AuthStatusMessageViewModel : ViewModel<FirebaseAuthManager.AuthStatus>
    {
        protected override void OnStateChangeFromView(FirebaseAuthManager.AuthStatus newState)
        {
        }
    }
}