using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnitMan.Source.Management.FirebaseManagement
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        public static FirebaseAuthManager Instance { get; private set; }
        
        public FirebaseApp app;

        public FirebaseAuth auth;

        public TMPro.TMP_InputField _emailField;

        public TMPro.TMP_InputField _passwordField;

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
                    app = FirebaseApp.DefaultInstance;
                    InitializeAuth();

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    Debug.LogError(string.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        private void InitializeAuth()
        {
            auth = FirebaseAuth.DefaultInstance;
            app.Options.DatabaseUrl = new Uri("https://unit-man-default-rtdb.firebaseio.com/");
        }

        public void RegisterUserWithTextFields()
        {
            TryRegisterUser(_emailField.text, _passwordField.text);
        }

        public void LoginUserWithTextFields()
        {
            TryLoginUser(_emailField.text, _passwordField.text);
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
