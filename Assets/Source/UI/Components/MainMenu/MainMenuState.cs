using UnitMan.Source.Management.Firebase.Auth;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public struct MainMenuState
    {
        public string _email;
        public string _password;
        public FirebaseAuthManager.AuthStatus _authStatus;
        public bool _isLoggedIn;

        public MainMenuState(string email, string password, FirebaseAuthManager.AuthStatus authStatus, bool isLoggedIn)
        {
            _email = email;
            _password = password;
            _authStatus = authStatus;
            _isLoggedIn = isLoggedIn;
        }
    }
}