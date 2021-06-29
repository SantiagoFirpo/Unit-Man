using TMPro;
using UnitMan.Source;
using UnityEngine;
using UnityEngine.UI;

namespace UnitMan
{
    public class SessionDataModel : MonoBehaviour
    {
        //TODO: add score label
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

        public void UpdateInvincibleTime(float newValue) {
            float normalizedInvincibleTimer = 1f - newValue/PlayerController.INVINCIBLE_TIME_SECONDS;
            UpdateInvincibleUI(normalizedInvincibleTimer);
        }

        public void IncrementScore(int delta)
        {
            _score += delta;
        }

        private void UpdateLivesLabel() {
            _livesLabel.text = $"Lives: {lives}";
            // _scoreLabel.text = $"Score: {_score}";
        }

        private void UpdateInvincibleUI(float newValue) {
            _invincibleTimer.fillAmount = newValue;
        }

        // Start is called before the first frame update
        public int lives = 3;
        private int _score;

        [SerializeField]
        private TMP_Text _livesLabel;
        
        [SerializeField]
        private TMP_Text _scoreLabel;
        
        [SerializeField]
        private Image _invincibleTimer;
    }
}
