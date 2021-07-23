using TMPro;
using UnityEngine;

namespace UnitMan.Source.Management.Session
{
    public class SessionDataModel : MonoBehaviour
    {
        //TODO: add lives icons

        // Start is called before the first frame update
        public int pelletsEaten;
        public int lives = 3;
        public int score = 0;
        public bool won = false;

        [SerializeField]
        private TMP_Text livesLabel;
        
        [SerializeField]
        private TMP_Text scoreLabel;
        public static SessionDataModel Instance
        {
            get;
            private set;
        }

        private void Awake() {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

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
            scoreLabel.text = $"Score: \n {score}";
        }

        private void UpdateLivesLabel() {
            livesLabel.text = $"Lives: \n {lives}";
            // _scoreLabel.text = $"Score: {_score}";
        }
    }
}
