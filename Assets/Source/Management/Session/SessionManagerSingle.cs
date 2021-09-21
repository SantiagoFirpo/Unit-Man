using UnitMan.Source.Entities.Actors;
using UnitMan.Source.Management.Audio;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Session
{
    public class SessionManagerSingle : MonoBehaviour
    {

        
        private const float FREEZE_SECONDS = 1f;
        public Timer freezeTimer;
        public static SessionManagerSingle Instance { get; private set; }
        private const float STARTUP_TIME_SECONDS = 4.5f;
        private const float DEATH_ANIMATION_SECONDS = 1.5f;
        private Timer _startupTimer;
        private Timer _deathAnimationTimer;
        
        private const int POINT_PER_LIFE_REMAINING = 500;
        
        [HideInInspector]
        public PlayerController playerController;

        public Transform leftWrap;
        public Transform rightWrap;

        public GameObject player;
        
        private GameObject _readyText;

        private bool _frozen = true;

        public Observable freezeObservable;

        public Observable unfreezeObservable;

        public Observable resetObservable;

        public Observable onPelletEatenObservable;
        
        public Observable powerPelletObservable;
        
        public int eatenGhostsTotal;
        public int fleeingGhostsTotal;


        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            freezeObservable = new Observable();
            unfreezeObservable = new Observable();
            resetObservable = new Observable();
            
            onPelletEatenObservable = new Observable();
            powerPelletObservable = new Observable();
        }
        
        

        private void DeathAnimationTimerOnOnEnd()
        {
            SessionViewModel.Instance.LoseLife();
            playerController.ResetAnimation();
            if (SessionViewModel.Instance.lives < 0) {
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
            
            _deathAnimationTimer = new Timer(DEATH_ANIMATION_SECONDS, false, true);
            
            playerController = player.GetComponent<PlayerController>();
            _startupTimer.OnEnd += StartLevel;
            freezeTimer.OnEnd += Unfreeze;
            _deathAnimationTimer.OnEnd += DeathAnimationTimerOnOnEnd;
            _readyText = GameObject.Find("Ready!");
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
            Freeze();
        }

        private void StartLevel() {
            // Debug.Log("StartingLevel!");
            Unfreeze();
            resetObservable.EmitNotification();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        public static void CheckIfGameIsWon() {
            if (SessionViewModel.Instance.pelletsEaten < LevelGridController.Instance.level.pelletCount) return;
            // Debug.Log("You won!");
            SessionViewModel.Instance.won = true;
            
            SessionViewModel.Instance.score += SessionViewModel.Instance.lives * POINT_PER_LIFE_REMAINING;
            SceneManager.LoadScene("You Won", LoadSceneMode.Single);
        }

        public void Die() {
            Debug.Log("Died!");
            
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Death, 1, false);
            Freeze();
            playerController.Die();
            _deathAnimationTimer.Start();
        }
        
        
        public void Freeze()
        {
            if (_frozen) return;
            freezeObservable.EmitNotification();
            _frozen = true;
        }

        private void Unfreeze()
        {
            if (!_frozen) return;
            unfreezeObservable.EmitNotification();
            _frozen = false;
        }
        private void ResetSession() {
            resetObservable.EmitNotification();
            Freeze();
            _startupTimer.Start();
            _readyText.SetActive(true);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.IntroMusic, 0, false);
        }

        private static void GameOver() {
            // Debug.Log("Game Over!");
            SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
        }

    }

    public enum FreezeType {EatGhost, Death, GameStart}
}
