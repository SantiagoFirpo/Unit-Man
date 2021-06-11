using System;
using TMPro;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Timer _startupTimer = new Timer(4f, 0f, true);
        
        public static event Action OnReset;

        public GameObject player;

        public GameObject[] sceneObjects;
        public Actor[] actors;

        public int pelletsEaten;

        public int lives = 3;

        [SerializeField]
        private GameObject readyText;

        // Start is called before the first frame update
        private void Awake() {
            if (Instance != null) {Destroy(gameObject);}
            Instance = this;
            _startupTimer.OnEnd += StartLevel;

            foreach (GameObject sceneObject in sceneObjects) {
                if (sceneObject.activeSelf) {
                    sceneObject.SetActive(false);
                }
                sceneObject.SetActive(true);
            }
        }

        private void StartLevel() {
            readyText.SetActive(false);
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Siren, 1, true);
            foreach (Actor actor in actors) {
                actor.rigidBody.simulated = true;
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
            Debug.Log("Died!");
            lives--;
            UIModel.Instance.LoseLife();
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
            SceneManager.LoadScene("Game Over");
        }
    }
}
