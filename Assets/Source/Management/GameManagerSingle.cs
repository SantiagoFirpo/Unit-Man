using System;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameManagerSingle : MonoBehaviour
    {
        //TODO: remake level reset
        private const float FREEZE_SECONDS = 1f;
        private Timer _pauseTimer;
        public static GameManagerSingle Instance { get; private set; }
        private Timer _startupTimer;
        
        [HideInInspector]
        public PlayerController playerController;
        
        public static event Action OnReset;

        private const int TOTAL_PELLETS = 284;
        
        public GameObject player;

        [SerializeField]
        private Actor[] actors;

        public int pelletsEaten;


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
            if (pelletsEaten < TOTAL_PELLETS) return;
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
            playerController.invincibleTimer.Stop();
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.EatGhost, 1, false);
            SetPause(true);
            _pauseTimer.Start();
        }

        private void UnpauseFreeze() {
            playerController.invincibleTimer.Start();
            Instance.SetPause(false);
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Retreating, 1, true);
        }
        private void Reset() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        private static void GameOver() {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("Game Over");
        }
    }
}
