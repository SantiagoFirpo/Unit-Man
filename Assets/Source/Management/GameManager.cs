using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int pelletsEaten = 0;
        // Start is called before the first frame update
        private void Awake() {
            if (Instance != null) {GameObject.Destroy(gameObject);}
            Instance = this;
        }

        public void CheckIfGameIsWon() {
            if (pelletsEaten >= 89) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                foreach (Actor actor in FindObjectsOfType<Actor>()) {
                    actor.Initialize();
                }
                
                
            }
        }
    }
}
