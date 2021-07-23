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
        }

        private void TryRegisterUser(string email, string password)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("Asynchronous User Register was cancelled");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError($"Asynchronous User Register encountered an error: {task.Exception}");
                    return;
                }

                // Firebase user has been created.
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }

        private void TryLoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("Login was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError($"Login encountered an error: {task.Exception}");
                    return;
                }

                FirebaseUser loggedUser = task.Result;
                Debug.LogFormat("User logged in successfully: {0} ({1})",
                    loggedUser.DisplayName, loggedUser.UserId);
            });
        }
    }
}
