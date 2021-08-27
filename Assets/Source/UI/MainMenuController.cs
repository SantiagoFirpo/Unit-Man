using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private Button testMapButton;
        
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
            LevelIDContainer.Instance.SetLevelID(levelIdField.text);
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);

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
