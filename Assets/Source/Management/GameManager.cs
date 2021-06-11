using System;
using TMPro;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameManager : MonoBehaviour
    {
        private const float FREEZE_SECONDS = 1f;
        private readonly Timer _pauseTimer = new Timer(FREEZE_SECONDS);
        public static GameManager Instance { get; private set; }
        private readonly Timer _startupTimer = new Timer(4f, 0f, true);
        public PlayerController playerController;
        
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
            playerController = player.GetComponent<PlayerController>();
            _startupTimer.OnEnd += StartLevel;
            _pauseTimer.OnEnd += UnpauseFreeze;

            // foreach (GameObject sceneObject in sceneObjects) {
            //     if (sceneObject.activeSelf) {
            //         sceneObject.SetActive(false);
            //     }
            //     sceneObject.SetActive(true);
            // }
        }

        private void StartLevel() {
            SetPause(false);
            readyText.SetActive(false);
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Siren, 1, true);
        }

        public void SetPause(bool paused) {
            foreach (Actor actor in actors) {
                actor.rigidBody.simulated = !paused;
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
        
        
        public void Freeze() {
            playerController.invincibleTimer.paused = true;
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.EatGhost, 1, false);
            SetPause(true);
            _pauseTimer.Begin();
        }

        private void UnpauseFreeze() {
            playerController.invincibleTimer.paused = false;
            Instance.SetPause(false);
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Retreating, 1, true);
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
