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
        private void Awake()
        {
            _pauseTimer = new Timer(FREEZE_SECONDS, true, true);
            _startupTimer = new Timer(4f, true, true);
            actors = FindObjectsOfType<Actor>();
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
            Debug.Log("StartingLevel!");
            SetPause(false);
            readyText.SetActive(false);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        public void SetPause(bool paused) {
            foreach (Actor actor in actors)
            {
                actor.Rigidbody.simulated = !paused;
                actor.animator.enabled = !paused;
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
            SessionDataModel.Instance.LoseLife();
            if (SessionDataModel.Instance.lives < 0) {
                GameOver();
            }
            else {
                Reset();
            }
        }
        
        
        public void Freeze() {
            playerController.invincibleTimer.Stop();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.EatGhost, 1, false);
            SetPause(true);
            _pauseTimer.Start();
        }

        private void UnpauseFreeze() {
            playerController.invincibleTimer.Start();
            Instance.SetPause(false);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Retreating, 1, true);
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
