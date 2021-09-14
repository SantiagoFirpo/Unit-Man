using System;
using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public class AuthStatusMessageView : View<FirebaseAuthManager.AuthStatus, AuthStatusMessageViewModel>
    {
        [SerializeField]
        private TMP_Text text;

        protected override void Render(FirebaseAuthManager.AuthStatus state)
        {
            text.SetText(MainMenuController.AuthStatusToMessage(state));
        }
    }
}