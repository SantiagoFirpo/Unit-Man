using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.AuthStatusMessage;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public class MVVMController : MonoBehaviour
    {
        private MVVMComponent<FirebaseAuthManager.AuthStatus,
                            AuthStatusMessageView, 
                            AuthStatusMessageViewModel, 
                            AuthStatusMessageModel> authStatusMessage;

        [Header("Auth Status Message")]
        [SerializeField]
        private AuthStatusMessageView _authStatusMessageView;
        
        [SerializeField]
        private AuthStatusMessageModel _authStatusMessageModel;
        private void Start()
        {
            authStatusMessage =
                new MVVMComponent<FirebaseAuthManager.AuthStatus, AuthStatusMessageView, AuthStatusMessageViewModel,
                    AuthStatusMessageModel>(_authStatusMessageView, _authStatusMessageModel, FirebaseAuthManager.AuthStatus.Empty);
        }
    }
}
