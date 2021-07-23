using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine;

namespace UnitMan.Source.Management.Firebase.Auth
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }

        private FirebaseApp _app;

        public FirebaseAuth auth;
        
        [SerializeField]
        private TMP_InputField emailField;
        
        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TextMeshProUGUI loginStatusLabel;

        private enum LoginStatus
        {
            RegisterCanceled, RegisterError, RegisterSuccessful, LoginCanceled, LoginError, LoginSuccessful, SignedOut
        }

        private LoginStatus _loginStatus;

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

        private void InitializeAuth()
        {
            auth = FirebaseAuth.DefaultInstance;
            _app.Options.DatabaseUrl = new Uri("https://unit-man-default-rtdb.firebaseio.com/");
        }

        public void RegisterUserWithTextFields()
        {
            TryRegisterUser(emailField.text, passwordField.text);
        }

        public void LoginUserWithTextFields()
        {
            TryLoginUser(emailField.text, passwordField.text);
        }

        public void SignOutUser()
        {
            auth.SignOut();
            _loginStatus = LoginStatus.SignedOut;
            loginStatusLabel.SetText("SIGNED OUT");

        }

        private void TryRegisterUser(string email, string password)
        {
            void RegisterTask(Task<FirebaseUser> task)
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Asynchronous User Register was cancelled");
                    _loginStatus = LoginStatus.RegisterCanceled;

                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Asynchronous User Register encountered an error: {task.Exception}");
                    _loginStatus = LoginStatus.RegisterError;

                    return;
                }

                // Firebase user has been created.
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                _loginStatus = LoginStatus.RegisterSuccessful;
                TryLoginUser(email, password);
            }

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(RegisterTask);
            UpdateLoginStatusLabel(email);
        }

        private void UpdateLoginStatusLabel(string email)
        {
            loginStatusLabel.SetText(_loginStatus switch
            {
                LoginStatus.LoginCanceled => "LOGIN CANCELED",
                LoginStatus.LoginError => "LOGIN ERROR",
                LoginStatus.RegisterCanceled => "REGISTERED CANCELED",
                LoginStatus.RegisterError => "REGISTER ERROR",
                LoginStatus.RegisterSuccessful => $"REGISTER SUCCESSFUL, REGISTERED AS {email}",
                LoginStatus.LoginSuccessful => $"LOGIN SUCCESSFUL, LOGGED AS {auth.CurrentUser.Email}",
                LoginStatus.SignedOut => "SIGNED OUT",
                _ => throw new ArgumentOutOfRangeException()
            });
        }

        private void TryLoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(LoginTask);
            UpdateLoginStatusLabel(email);
            
        }

        private void LoginTask(Task<FirebaseUser> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login was canceled.");
                _loginStatus = LoginStatus.LoginCanceled;

                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"Login encountered an error: {task.Exception}");
                _loginStatus = LoginStatus.LoginError;

                return;
            }

            FirebaseUser loggedUser = task.Result;
            Debug.LogFormat("User logged in successfully: {0} ({1})", loggedUser.DisplayName, loggedUser.UserId);
            // logStatus.gameObject.SetActive(false);
            _loginStatus = LoginStatus.LoginSuccessful;
        }
    }
}
