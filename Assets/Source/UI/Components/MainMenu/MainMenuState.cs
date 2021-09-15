using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    [Serializable]
    public class MainMenuState
    {
        public string _email;
        [HideInInspector]
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

        public MainMenuState()
        {
            
        }
    }
}