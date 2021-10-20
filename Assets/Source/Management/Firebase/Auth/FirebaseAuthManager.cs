using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Management.Firebase.Auth
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }

        private FirebaseApp _app;

        private FirebaseAuth _auth;

        [SerializeField]
        private Reactive<FirebaseUser> reactiveUser;
        
        public FirebaseUser User {get; private set;}

        public enum AuthStatus
        {
            WaitingForUser,
            RegisterCanceled, RegisterError, RegisterSuccessful, LoginCanceled, LoginError, LoginSuccessful, SignedOut,
            Empty,
            LoggingIn,
            Registering,
            RegisterRequested,
            LoginRequested,
            SignOutRequested
        }

        public Observable<AuthStatus> authStateChangedObservable;

        // Start is called before the first frame update
        
        
        private void Awake()
        {
            if (Instance != null)
            {
                // Debug.LogError("Instance is not null");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                authStateChangedObservable = new Observable<AuthStatus>();

            }



        }

        public static string GetDisplayName()
        {
            return Instance._auth.CurrentUser.Email.Split(char.Parse("@"))[0];
        }

        private void InitializeFirebase()
        {
            
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = FirebaseApp.DefaultInstance;
                    InitializeAuth();

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        private void Start()
        {
            InitializeFirebase();
        }

        private void InitializeAuth()
        {
            
            _auth = FirebaseAuth.GetAuth(_app);
            _auth.StateChanged += OnAuthStateChanged;
            OnAuthStateChanged(this, null);
            _app.Options.DatabaseUrl = new Uri("https://unit-man-default-rtdb.firebaseio.com/");
        }
        
        //BUG: user can't logout after gameplay because the AuthManager is flagged DontDestroyOnLoad
        public void SignOutUser()
        {
            _auth.SignOut();
            SetAuthStatus(AuthStatus.SignedOut);
        }
        
        private void OnAuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (_auth.CurrentUser == User) return;
            bool signedIn = User != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && User != null) {
                Debug.Log($"Signed out {User.UserId}");
                MainMenuRouter.Instance.OnUserSignedOut();
            }
            User = _auth.CurrentUser;
            reactiveUser.SetValue(User);
            if (!signedIn) return;
            Debug.Log($"Signed in {User.UserId}");
            MainMenuRouter.Instance.OnUserLoggedIn();
        }

        public void TryRegisterUser(string email, string password)
        {
            SetAuthStatus(AuthStatus.Registering);
            void RegisterTask(Task<FirebaseUser> task)
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Asynchronous User Register was cancelled");
                    SetAuthStatus(AuthStatus.RegisterCanceled);

                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Asynchronous User Register encountered an error: {task.Exception}");
                    SetAuthStatus(AuthStatus.RegisterError);

                    return;
                }

                // Firebase user has been created.
                FirebaseUser newUser = task.Result;
                Debug.Log($"Firebase user created successfully: {GetDisplayName()} ({newUser.UserId})");
                SetAuthStatus(AuthStatus.RegisterSuccessful);
                TryLoginUser(email, password);
            }

            _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(RegisterTask);
            SetAuthStatus(AuthStatus.Registering);
        }

        public void TryLoginUser(string email, string password)
        {
            SetAuthStatus(AuthStatus.LoggingIn);
            _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(LoginTask);
            
        }

        private void LoginTask(Task<FirebaseUser> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login was canceled.");
                SetAuthStatus(AuthStatus.LoginCanceled);
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"Login encountered an error: {task.Exception}");
                SetAuthStatus(AuthStatus.LoginError);
                return;
            }

            FirebaseUser loggedUser = task.Result;
            Debug.Log($"User logged in successfully: {loggedUser.DisplayName} ({loggedUser.UserId})");
            // logStatus.gameObject.SetActive(false);
            SetAuthStatus(AuthStatus.LoginSuccessful);
        }

        public void SetAuthStatus(AuthStatus authStatus)
        {
            authStateChangedObservable.EmitNotification(authStatus);
        }
        
        private void OnDisable()
        {
            _auth.StateChanged -= OnAuthStateChanged;
            _auth = null;
        }

        public void DeleteUserAccount()
        {
            User?.DeleteAsync().ContinueWithOnMainThread(task => {
                if (task.IsCanceled) {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError($"DeleteAsync encountered an error: {task.Exception}");
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }
    }
}
