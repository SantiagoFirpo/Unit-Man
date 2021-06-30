using System;
using UnitMan.Source.Entities.Actors;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class SessionManagerSingle : MonoBehaviour
    {

        
        private const float FREEZE_SECONDS = 1f;
        private Timer _freezeTimer;
        public static SessionManagerSingle Instance { get; private set; }
        private const float STARTUP_TIME_SECONDS = 4f;
        private Timer _startupTimer;
        
        [HideInInspector]
        public PlayerController playerController;

        public Transform leftWrap;
        public Transform rightWrap;
        
        public static event Action OnReset;

        private const int TOTAL_PELLETS = 284;
        
        public GameObject player;

        [SerializeField]
        private GameObject readyText;

        // Start is called before the first frame update
        private void Awake()
        {
            _startupTimer = new Timer(STARTUP_TIME_SECONDS, true, true);
            _freezeTimer = new Timer(FREEZE_SECONDS, false, true);
            if (Instance != null) {Destroy(gameObject);}
            Instance = this;
            playerController = player.GetComponent<PlayerController>();
            _startupTimer.OnEnd += StartLevel;
            _freezeTimer.OnEnd += UnpauseFreeze;
        }

        private void Start()
        {
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
            OnFreeze?.Invoke();
        }

        private void StartLevel() {
            Debug.Log("StartingLevel!");
            OnUnfreeze?.Invoke();
            OnReset?.Invoke();
            readyText.SetActive(false);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        public void CheckIfGameIsWon() {
            if (SessionDataModel.Instance.pelletsEaten < TOTAL_PELLETS) return;
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
            OnFreeze?.Invoke();
            _freezeTimer.Start();
        }

        private void UnpauseFreeze() {
            OnUnfreeze?.Invoke();
        }
        private void Reset() {
            OnReset?.Invoke();
            OnFreeze?.Invoke();
            _startupTimer.Start();
            readyText.SetActive(true);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
        }

        private static void GameOver() {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("Game Over");
        }

        public static event Action OnFreeze;
        public static event Action OnUnfreeze;
    }
}
