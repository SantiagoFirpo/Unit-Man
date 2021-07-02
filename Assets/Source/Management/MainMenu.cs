using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan.Source.Management
{
    public class MainMenu : MonoBehaviour
    {
        //TODO: restart doesnt allow for starting
        [SerializeField]
        private Button startButton;
        
        private static void OnPressStart() {
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        private void Start() {
            startButton.onClick.AddListener(OnPressStart);
        }
    }
}
