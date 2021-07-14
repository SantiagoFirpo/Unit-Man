using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.Management
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button classicalMapButton;
        [SerializeField]
        private Button testMapButton;
        
        public void OnPressStart() {
            // classicalMapButton.gameObject.SetActive(true);
            testMapButton.gameObject.SetActive(true);
        }

        public void OnSelectClassicMap()
        {
            SceneManager.LoadScene("Classic Map", LoadSceneMode.Single);
        }

        public void OnSelectTestMap()
        {
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        public void Start() {
            // startButton.onClick.AddListener(OnPressStart);
        }
    }
}
