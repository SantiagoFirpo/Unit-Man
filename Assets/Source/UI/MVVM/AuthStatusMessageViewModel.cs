using UnitMan.Source.Management.Firebase.Auth;

namespace UnitMan.Source.UI.MVVM
{
    public class AuthStatusMessageViewModel : ViewModel<FirebaseAuthManager.AuthStatus>
    {
        protected override void OnStateChangeFromView(FirebaseAuthManager.AuthStatus newState)
        {
        }
    }
}