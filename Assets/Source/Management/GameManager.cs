using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action OnReset;

        public GameObject player;

        public GameObject[] sceneObjects;

        public int pelletsEaten = 0;

        public int lives = 3; 
        // Start is called before the first frame update
        private void Awake() {
            if (Instance != null) {GameObject.Destroy(gameObject);}
            Instance = this;

            foreach (GameObject sceneObject in sceneObjects) {
                sceneObject.SetActive(true);
            }
        }

        public void CheckIfGameIsWon() {
            if (pelletsEaten < 282) return;
            Debug.Log("You won!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            foreach (Actor actor in FindObjectsOfType<Actor>()) {
                actor.Initialize();
            }
        }

        public void Die() {
            lives--;
            if (lives < 0) {
                GameOver();
            }
            else {
                Reset();
            }
        }

        private void Reset() {
            OnReset?.Invoke();
        }

        private static void GameOver() {
            Debug.Log("Game Over!");
        }
    }
}
