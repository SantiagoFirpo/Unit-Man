using System;
using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.Management.Session
{
    public class SessionViewModel : MonoBehaviour
    {
        //TODO: add lives icons
        
        public int pelletsEaten;
        public int lives = 3;
        public int score = 0;
        public bool won = false;

        [SerializeField]
        private Reactive<string> livesBinding;

        [SerializeField]
        private Reactive<string> scoreBinding;
        
        [SerializeField]
        private TMP_Text scoreLabel;

        [SerializeField]
        private Reactive<bool> virtualStickVisibility;

        public static SessionViewModel Instance
        {
            get;
            private set;
        }

        private void Awake() {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // private void Start()
        // {
        //     virtualStickVisibility.SetValue(Application.isMobilePlatform);
        // }

        public void LoseLife() {
            lives--;
            UpdateLivesLabel();
        }
        

        public void IncrementScore(int delta)
        {
            score += delta;
            UpdateScoreLabel();
        }

        private void UpdateScoreLabel()
        {
            scoreBinding.SetValue($"Score: \n {score}"); 
        }

        private void UpdateLivesLabel() {
            livesBinding.SetValue($"Lives: {lives}"); 
            // _scoreLabel.text = $"Score: {_score}";
        }
    }
}
