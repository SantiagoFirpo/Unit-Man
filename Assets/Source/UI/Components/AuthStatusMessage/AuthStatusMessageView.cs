using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.AuthStatusMessage
{
    [RequireComponent(typeof(TMP_Text))]
    public class AuthStatusMessageView : View<FirebaseAuthManager.AuthStatus, AuthStatusMessageViewModel>
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        protected override void Render(FirebaseAuthManager.AuthStatus state)
        {
            _text.SetText(MainMenuController.AuthStatusToMessage(state));
        }
    }
}