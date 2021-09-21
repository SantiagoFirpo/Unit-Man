using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Management.Firebase.Auth
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }

        private FirebaseApp _app;

        public FirebaseAuth auth;

        public enum AuthStatus
        {
            WaitingForUser,
            RegisterCanceled, RegisterError, RegisterSuccessful, LoginCanceled, LoginError, LoginSuccessful, SignOut,
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
            auth = FirebaseAuth.GetAuth(_app);
            _app.Options.DatabaseUrl = new Uri("https://unit-man-default-rtdb.firebaseio.com/");
        }
        
        //BUG: user can't logout after gameplay because the AuthManager is flagged DontDestroyOnLoad
        public void SignOutUser()
        {
            auth.SignOut();
            SetAuthStatus(AuthStatus.SignOut);
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
                Debug.Log($"Firebase user created successfully: {newUser.DisplayName} ({newUser.UserId})");
                SetAuthStatus(AuthStatus.RegisterSuccessful);
                TryLoginUser(email, password);
            }

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(RegisterTask);
            SetAuthStatus(AuthStatus.Registering);
        }

        public void TryLoginUser(string email, string password)
        {
            SetAuthStatus(AuthStatus.LoggingIn);
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(LoginTask);
            
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
    }
}
