using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace UnitMan.Source.Management.Firebase.Auth
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }

        private FirebaseApp _app;

        public FirebaseAuth auth;

        private enum LoginStatus
        {
            Fetching,
            RegisterCanceled, RegisterError, RegisterSuccessful, LoginCanceled, LoginError, LoginSuccessful, SignedOut
        }

        private LoginStatus _authStatus;

        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
            }

            InitializeFirebase();


        }

        private void AuthFetchTimerOnOnEnd()
        {
            
        }

        public string AuthStatusMessage { get {return _authStatus switch
        {
            LoginStatus.LoginCanceled => "LOGIN CANCELED",
            LoginStatus.LoginError => "LOGIN ERROR",
            LoginStatus.RegisterCanceled => "REGISTERED CANCELED",
            LoginStatus.RegisterError => "REGISTER ERROR",
            LoginStatus.RegisterSuccessful => $"REGISTER SUCCESSFUL, REGISTERED AS {auth.CurrentUser.Email}",
            LoginStatus.LoginSuccessful => $"LOGIN SUCCESSFUL, LOGGED AS {auth.CurrentUser.Email}",
            LoginStatus.SignedOut => "SIGNED OUT",
            LoginStatus.Fetching => "FETCHING",
            _ => throw new ArgumentOutOfRangeException()
        };}}

        private void InitializeFirebase()
        {
            
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = FirebaseApp.Create();
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

        private void InitializeAuth()
        {
            auth = FirebaseAuth.GetAuth(_app);
            _app.Options.DatabaseUrl = new Uri("https://unit-man-default-rtdb.firebaseio.com/");
        }
        
        //TODO: move all dependent methods and fields to MainMenu

        //BUG: user can't logout after gameplay because the AuthManager is flagged DontDestroyOnLoad
        public void SignOutUser()
        {
            auth.SignOut();
            _authStatus = LoginStatus.SignedOut;
        }

        public void TryRegisterUser(string email, string password)
        {
            _authStatus = LoginStatus.Fetching;
            void RegisterTask(Task<FirebaseUser> task)
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Asynchronous User Register was cancelled");
                    _authStatus = LoginStatus.RegisterCanceled;

                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Asynchronous User Register encountered an error: {task.Exception}");
                    _authStatus = LoginStatus.RegisterError;

                    return;
                }

                // Firebase user has been created.
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                _authStatus = LoginStatus.RegisterSuccessful;
                TryLoginUser(email, password);
            }

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(RegisterTask);
            _authStatus = LoginStatus.Fetching;
        }

        public void TryLoginUser(string email, string password)
        {
            _authStatus = LoginStatus.Fetching;
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(LoginTask);
            
        }

        private void LoginTask(Task<FirebaseUser> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login was canceled.");
                _authStatus = LoginStatus.LoginCanceled;

                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"Login encountered an error: {task.Exception}");
                _authStatus = LoginStatus.LoginError;

                return;
            }

            FirebaseUser loggedUser = task.Result;
            Debug.LogFormat("User logged in successfully: {0} ({1})", loggedUser.DisplayName, loggedUser.UserId);
            // logStatus.gameObject.SetActive(false);
            _authStatus = LoginStatus.LoginSuccessful;
        }
    }
}
