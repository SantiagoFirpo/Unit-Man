using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.AuthStatusMessage
{
    public class AuthStatusMessageModel : Model<FirebaseAuthManager.AuthStatus, AuthStatusMessageViewModel>
    {
        public void SetMessage(FirebaseAuthManager.AuthStatus authStatus)
        {
            viewModel.SetState(authStatus);
        }
    }
}