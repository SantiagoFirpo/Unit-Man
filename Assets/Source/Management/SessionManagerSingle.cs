using System;
using UnitMan.Source.Entities.Actors;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class SessionManagerSingle : MonoBehaviour
    {

        
        private const float FREEZE_SECONDS = 1f;
        public Timer freezeTimer;
        public static SessionManagerSingle Instance { get; private set; }
        private const float STARTUP_TIME_SECONDS = 4.5f;
        private Timer _startupTimer;
        private Timer _deathAnimationTimer;
        
        [HideInInspector]
        public PlayerController playerController;

        public Transform leftWrap;
        public Transform rightWrap;
        
        public static event Action OnReset;

        public GameObject player;
        
        private GameObject _readyText;

        private static readonly int OnDeath = Animator.StringToHash("OnDeath");
        private bool _frozen = true;


        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance != null) {Destroy(gameObject);}
            Instance = this;

        }
        
        

        private void DeathAnimationTimerOnOnEnd()
        {
            SessionDataModel.Instance.LoseLife();
            if (SessionDataModel.Instance.lives < 0) {
                GameOver();
            }
            else {
                ResetSession();
            }
        }

        private void Start()
        {
            _startupTimer = new Timer(STARTUP_TIME_SECONDS, true, true);
            freezeTimer = new Timer(FREEZE_SECONDS, false, true);
            _deathAnimationTimer = new Timer(1.5f, false, true);
            
            playerController = player.GetComponent<PlayerController>();
            _startupTimer.OnEnd += StartLevel;
            freezeTimer.OnEnd += Unfreeze;
            _deathAnimationTimer.OnEnd += DeathAnimationTimerOnOnEnd;
            _readyText = GameObject.Find("Ready!");
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
            Freeze(FreezeType.GameStart);
        }

        private void StartLevel() {
            Debug.Log("StartingLevel!");
            Unfreeze();
            OnReset?.Invoke();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        public static void CheckIfGameIsWon() {
            if (SessionDataModel.Instance.pelletsEaten < LevelGridController.Instance.mazeData.pelletCount) return;
            Debug.Log("You won!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            foreach (Actor actor in FindObjectsOfType<Actor>()) {
                actor.Initialize();
            }
        }

        public void Die() {
            Debug.Log("Died!");
            
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Death, 1, false);
            Freeze(FreezeType.Death);
            playerController.animator.SetTrigger(OnDeath);
            _deathAnimationTimer.Start();
        }
        
        
        public void Freeze(FreezeType freezeType)
        {
            if (_frozen) return;
            OnFreeze?.Invoke(freezeType);
            _frozen = true;
        }
        
        public void Unfreeze()
        {
            if (!_frozen) return;
            OnUnfreeze?.Invoke();
            _frozen = false;
        }
        private void ResetSession() {
            OnReset?.Invoke();
            Freeze(FreezeType.GameStart);
            _startupTimer.Start();
            _readyText.SetActive(true);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
        }

        private static void GameOver() {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
        }

        public static event Action<FreezeType> OnFreeze;
        public static event Action OnUnfreeze;
    }

    public enum FreezeType {EatGhost, Death, GameStart}
}
