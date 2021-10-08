using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.UI.Components.Auth;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [SerializeField]
        private string levelId;
        
        private Observer<FirebaseAuthManager.AuthStatus> _authObserver;

        [SerializeField]
        private Reactive<string> authStatus = new Reactive<string>();

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

        private void Start()
        {
            FirebaseAuthManager.Instance.authStateChangedObservable.Attach(_authObserver);
        }

        private void OnAuthChanged(FirebaseAuthManager.AuthStatus newAuthStatus)
        {
            this.authStatus.SetValue(AuthViewModel.AuthStatusToMessage(newAuthStatus));
            if (newAuthStatus == FirebaseAuthManager.AuthStatus.SignedOut)
            {
                MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Auth);
            }
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
        
        public void OnSignOutPressed()
        {
            FirebaseAuthManager.Instance.SignOutUser();
            OnAuthChanged(FirebaseAuthManager.AuthStatus.SignedOut);
        }

        public void OnPlayButtonPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.OnlineLevelExplorer);
            // try
            // {
            //     LoadLocalLevel(levelId);
            //     SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogException(e);
            //     Debug.Log("Failed to fetch locally, Downloading level from firestore");
            //     //Display "loading..."
            //     DownloadFirestoreLevelWithId(levelId);
            // }
        }

        public void OnMyLevelsButtonPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.LocalLevelExplorer);
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

        public override void OnRendered()
        {
        }
    }
}