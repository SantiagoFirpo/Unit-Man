using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.UI
{
    
    public class MainMenuController : MonoBehaviour
    {
        
        [SerializeField]
        private TextMeshProUGUI loginStatusLabel;
        
        [SerializeField]
        private TMP_InputField emailField;
        
        [SerializeField]
        private TMP_InputField passwordField;

        private Timer _authFetchTimer;
        [SerializeField]
        private TMP_InputField levelIdField;


        private void Awake()
        {
            #if UNITY_EDITOR
                        Debug.unityLogger.logEnabled = true;
            #else
                         Debug.unityLogger.logEnabled=false;
            #endif
            Debug.Log("Main Menu should initialize");
        }

        private void AuthFetchTimerOnOnEnd()
        {
            Debug.Log("Timer finished");
            loginStatusLabel.SetText(FirebaseAuthManager.Instance.AuthStatusMessage);
        }

        private void Start()
        {
            
            // if (FirebaseAuthManager.Instance.auth == null) return;
            _authFetchTimer = new Timer(1f, false, true);
            _authFetchTimer.OnEnd += AuthFetchTimerOnOnEnd;
            loginStatusLabel.SetText(FirebaseAuthManager.Instance.AuthStatusMessage);
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
            CrossSceneLevelContainer.Instance.SetLevel(levelFromJson);
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        private static Level LevelJsonWrapper(FirestoreLevel firestoreLevel)
        {
            string levelJson = JsonUtility.ToJson(Level.FromFirestoreLevel(firestoreLevel));
            Debug.Log(levelJson);
            Level levelFromJson = JsonUtility.FromJson<Level>(levelJson);
            return levelFromJson;
        }

        public void LoadLocalLevel(string levelId)
        {
            CrossSceneLevelContainer.Instance.SetLevel(JsonUtility.FromJson<Level>(FirestoreListener.LoadStringFromJson(levelId)));
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
            loginStatusLabel.SetText("FETCHING...");
            FirebaseAuthManager.Instance.TryRegisterUser(emailField.text, passwordField.text);
            _authFetchTimer.Start();
        }

        public void LoginUser()
        {
            loginStatusLabel.SetText("FETCHING...");
            FirebaseAuthManager.Instance.TryLoginUser(emailField.text, passwordField.text);
            _authFetchTimer.Start();

        }

        public void SignOut()
        {
            loginStatusLabel.SetText("FETCHING...");
            FirebaseAuthManager.Instance.SignOutUser();
            _authFetchTimer.Start();
        }
    }
}
