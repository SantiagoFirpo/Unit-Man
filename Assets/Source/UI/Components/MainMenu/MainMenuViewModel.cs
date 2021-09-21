using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.UI.Components.Text;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [SerializeField]
        private string email;
        [SerializeField]
        private string password;

        [SerializeField]
        private string levelId;

        [SerializeField]
        private OneWayBinding<string> authStatusMessageBinding;

        private Observer<FirebaseAuthManager.AuthStatus> _authObserver;

        private void Awake()
        {
            #if UNITY_EDITOR
                        Debug.unityLogger.logEnabled = true;
            #else
                                     Debug.unityLogger.logEnabled=false;
            #endif
                        Debug.Log("Main Menu should initialize");
            _authObserver = new Observer<FirebaseAuthManager.AuthStatus>(OnAuthChanged);
        }

        private void OnAuthChanged(FirebaseAuthManager.AuthStatus authStatus)
        {
            authStatusMessageBinding.SetValue(AuthStatusToMessage(authStatus));
        }

        public void OnEmailChanged(string newEmail)
        {
            this.email = newEmail;
        }

        public void OnPasswordChanged(string newPassword)
        {
            this.password = newPassword;
        }

        public void OnLoginPressed()
        {
            FirebaseAuthManager.Instance.TryLoginUser(email, password);
        }
        
        public void OnRegisterPressed()
        {
            FirebaseAuthManager.Instance.TryRegisterUser(email, password);
        }

        public void OnSignOutPressed()
        {
            FirebaseAuthManager.Instance.SignOutUser();
        }

        public void OnLevelIdChanged(string newLevelId)
        {
            levelId = newLevelId;
        }

        public void OnLevelEditorPressed()
        {
            SceneManager.LoadScene("Level Editor", LoadSceneMode.Single);
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        public void OnPlayButtonPressed()
        {
            try
            {
                LoadLocalLevel(levelId);
                SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Failed to fetch locally, Downloading level from firestore");
                //Display "loading..."
                DownloadFirestoreLevelWithId(levelId);
            }
        }
        
        private static void LoadLocalLevel(string levelId)
        {
            CrossSceneLevelContainer.Instance.level = JsonUtility.FromJson<Level>(FirestoreListener.LoadStringFromJson(levelId));
        }
        
        private static Level LevelJsonWrapper(FirestoreLevel firestoreLevel)
        {
            string levelJson = JsonUtility.ToJson(Level.FromFirestoreLevel(firestoreLevel));
            Debug.Log(levelJson);
            return JsonUtility.FromJson<Level>(levelJson);
        }
        private static void StoreLevelAndGoToGameplay(Task<DocumentSnapshot> task)
        {
            Level levelFromJson = LevelJsonWrapper(task.Result.ConvertTo<FirestoreLevel>());
            CrossSceneLevelContainer.Instance.level = levelFromJson;
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }
        
        private static void DownloadFirestoreLevelWithId(string levelId)
        {
            FirebaseFirestore.DefaultInstance.Document($"levels/{levelId}")
                .GetSnapshotAsync()
                .ContinueWithOnMainThread(task =>
                {
                    AggregateException aggregateException = task.Exception;
                    if (aggregateException == null)
                    {
                        StoreLevelAndGoToGameplay(task);
                    }
                    else
                    {
                        Debug.LogException(aggregateException);
                    }
                });
        }

        private void Start()
        {
            FirebaseAuthManager.Instance.authStateChangedObservable.Attach(_authObserver);
        }

        public static string AuthStatusToMessage(FirebaseAuthManager.AuthStatus authStatus)
        {
            return authStatus switch
            {
                FirebaseAuthManager.AuthStatus.LoggingIn => "LOGGING IN",
                FirebaseAuthManager.AuthStatus.Registering => "REGISTERING",
                FirebaseAuthManager.AuthStatus.WaitingForUser => "PLEASE REGISTER/LOGIN BELOW:",
                FirebaseAuthManager.AuthStatus.RegisterCanceled => "REGISTER WAS CANCELED",
                FirebaseAuthManager.AuthStatus.RegisterError => "REGISTER ERROR",
                FirebaseAuthManager.AuthStatus.RegisterSuccessful => $"REGISTER SUCCESSFUL! REGISTERED AS {FirebaseAuthManager.Instance.auth.CurrentUser.Email}",
                FirebaseAuthManager.AuthStatus.LoginCanceled => "LOGIN WAS CANCELED",
                FirebaseAuthManager.AuthStatus.LoginError => "LOGIN ERROR",
                FirebaseAuthManager.AuthStatus.LoginSuccessful => $"LOGIN SUCCESSFUL! LOGGED IN AS {FirebaseAuthManager.Instance.auth.CurrentUser.Email}",
                FirebaseAuthManager.AuthStatus.SignOut => "SIGNED OUT",
                FirebaseAuthManager.AuthStatus.Empty => "",
                FirebaseAuthManager.AuthStatus.RegisterRequested => "REGISTERING",
                FirebaseAuthManager.AuthStatus.LoginRequested => "LOGGING IN",
                FirebaseAuthManager.AuthStatus.SignOutRequested => "SIGNING OUT",
                _ => throw new ArgumentOutOfRangeException(nameof(authStatus), authStatus, null)
            };
        }
    }
}