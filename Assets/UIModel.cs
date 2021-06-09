using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnitMan.Source;
using UnityEngine;
using UnityEngine.UI;

namespace UnitMan
{
    public class UIModel : MonoBehaviour
    {
        public static UIModel Instance
        {
            get;
            private set;
        }

        private void Awake() {
            if (Instance != null) return;
            Instance = this;
        }

        public void LoseLife() {
            _lives--;
            UpdateLivesLabel();
        }

        public void UpdateInvincibleTime(float newValue) {
            float normalizedInvincibleTimer = 1f - newValue/PlayerController.INVINCIBLE_TIME_SECONDS;
            UpdateInvincibleUI(normalizedInvincibleTimer);
        }

        private void UpdateLivesLabel() {
            _livesLabel.text = $"Lives: {_lives}";
            // _scoreLabel.text = $"Score: {_score}";
        }

        private void UpdateInvincibleUI(float newValue) {
            _invincibleTimer.fillAmount = newValue;
        }

        // Start is called before the first frame update
        private int _lives = 3;
        private int _score;
        private float _normalizedInvincibleTimer = 0f;
        
        [SerializeField]
        private TMP_Text _livesLabel;
        
        [SerializeField]
        private TMP_Text _scoreLabel;
        
        [SerializeField]
        private Image _invincibleTimer;
    }
}
