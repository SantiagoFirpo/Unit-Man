using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnitMan.Source.Management
{
    public class SessionDataModel : MonoBehaviour
    {
        //TODO: add lives icons
        public static SessionDataModel Instance
        {
            get;
            private set;
        }

        private void Awake() {
            if (Instance != null) return;
            Instance = this;
        }

        public void LoseLife() {
            lives--;
            UpdateLivesLabel();
        }
        

        public void IncrementScore(int delta)
        {
            _score += delta;
            UpdateScoreLabel();
        }

        private void UpdateScoreLabel()
        {
            scoreLabel.text = $"Score: \n {_score}";
        }

        private void UpdateLivesLabel() {
            livesLabel.text = $"Lives: \n {lives}";
            // _scoreLabel.text = $"Score: {_score}";
        }

        // Start is called before the first frame update
        public int pelletsEaten;
        public int lives = 3;
        private int _score;

        [SerializeField]
        private TMP_Text livesLabel;
        
        [SerializeField]
        private TMP_Text scoreLabel;
    }
}
