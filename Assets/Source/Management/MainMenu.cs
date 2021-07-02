using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.Management
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        
        private static void OnPressStart() {
            SceneManager.LoadScene("Gameplay");
        }

        private void Start() {
            startButton.onClick.AddListener(OnPressStart);
        }
    }
}
