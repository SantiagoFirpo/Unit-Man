using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button testMapButton;
        
        [SerializeField]
        private TextMeshProUGUI loginStatusLabel;
        
        [SerializeField]
        private TMP_InputField emailField;
        
        [SerializeField]
        private TMP_InputField passwordField;

        public void OnPressStart() {
            // classicalMapButton.gameObject.SetActive(true);
            testMapButton.gameObject.SetActive(true);
        }

        public void OnSelectClassicMap()
        {
            // SceneManager.LoadScene("Classic Map", LoadSceneMode.Single);
        }

        public void OnSelectTestMap()
        {
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        private void Start()
        {
            if (FirebaseAuthManager.Instance.auth == null) return;
            FirebaseAuthManager.Instance.UpdateLoginStatusLabel(FirebaseAuthManager.Instance.auth.CurrentUser.Email);
        }

        public void RegisterUser()
        {
            FirebaseAuthManager.Instance.RegisterUserWithTextFields();
        }
    }
}
