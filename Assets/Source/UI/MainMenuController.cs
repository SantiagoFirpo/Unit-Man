using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI
{
    
    public class MainMenuController : MonoBehaviour
    {
        
        [SerializeField]
        private TextMeshProUGUI authStatusLabel;
        
        [SerializeField]
        private TMP_InputField emailField;
        
        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TMP_InputField levelIdField;

        public Observer<FirebaseAuthManager.AuthStatus> onAuthStatusChangedObserver;


        private void Awake()
        {
            #if UNITY_EDITOR
                        Debug.unityLogger.logEnabled = true;
            #else
                         Debug.unityLogger.logEnabled=false;
            #endif
            Debug.Log("Main Menu should initialize");
        }

        private void Start()
        {
            onAuthStatusChangedObserver = new Observer<FirebaseAuthManager.AuthStatus>(SetAuthStatusLabel);
            FirebaseAuthManager.Instance.authStateChangedEmitter.Attach(onAuthStatusChangedObserver);
        }


        private void SetAuthStatusLabel(Emitter<FirebaseAuthManager.AuthStatus> source, FirebaseAuthManager.AuthStatus authStatus)
        {
            
            authStatusLabel.SetText(authStatus switch
            {
                FirebaseAuthManager.AuthStatus.Fetching => "FETCHING",
                FirebaseAuthManager.AuthStatus.WaitingForUser => "PLEASE REGISTER/LOGIN BELOW:",
                FirebaseAuthManager.AuthStatus.RegisterCanceled => "REGISTER WAS CANCELED",
                FirebaseAuthManager.AuthStatus.RegisterError => "REGISTER ERROR",
                FirebaseAuthManager.AuthStatus.RegisterSuccessful => $"REGISTER SUCCESSFUL! REGISTERED AS {FirebaseAuthManager.Instance.auth.CurrentUser.Email}",
                FirebaseAuthManager.AuthStatus.LoginCanceled => "LOGIN WAS CANCELED",
                FirebaseAuthManager.AuthStatus.LoginError => "LOGIN ERROR",
                FirebaseAuthManager.AuthStatus.LoginSuccessful => $"LOGIN SUCCESSFUL! LOGGED IN AS {FirebaseAuthManager.Instance.auth.CurrentUser.Email}",
                FirebaseAuthManager.AuthStatus.SignedOut => "SIGNED OUT",
                _ => throw new ArgumentOutOfRangeException(nameof(authStatus), authStatus, null)
            });
        }

        public void GoToLeaderboard()
        {
            SceneManager.LoadScene("Scoreboard");
        }
        
        public void OnPressStart() {
            // classicalMapButton.gameObject.SetActive(true);
            // testMapButton.gameObject.SetActive(true);
            string levelId = levelIdField.text;
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

        private static void StoreLevelAndGoToGameplay(Task<DocumentSnapshot> task)
        {
            Level levelFromJson = LevelJsonWrapper(task.Result.ConvertTo<FirestoreLevel>());
            CrossSceneLevelContainer.Instance.level = levelFromJson;
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        private static Level LevelJsonWrapper(FirestoreLevel firestoreLevel)
        {
            string levelJson = JsonUtility.ToJson(Level.FromFirestoreLevel(firestoreLevel));
            Debug.Log(levelJson);
            return JsonUtility.FromJson<Level>(levelJson);
        }

        private static void LoadLocalLevel(string levelId)
        {
            CrossSceneLevelContainer.Instance.level = JsonUtility.FromJson<Level>(FirestoreListener.LoadStringFromJson(levelId));
        }

        public void OnSelectTestMap()
        {
        }

        public void OnLevelEditorSelected()
        {
            SceneManager.LoadScene("Level Editor", LoadSceneMode.Single);
        }

        public void RegisterUser()
        {
            // SetAuthStatusMessageToFetching();
            FirebaseAuthManager.Instance.TryRegisterUser(emailField.text, passwordField.text);
        }

        public void LoginUser()
        {
            // SetAuthStatusMessageToFetching();
            FirebaseAuthManager.Instance.TryLoginUser(emailField.text, passwordField.text);
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void SetAuthStatusMessageToFetching()
        {
            authStatusLabel.SetText("FETCHING...");
        }

        public void SignOut()
        {
            // SetAuthStatusMessageToFetching();
            FirebaseAuthManager.Instance.SignOutUser();
        }
    }
}
