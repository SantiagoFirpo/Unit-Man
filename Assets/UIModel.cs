using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            UpdateUI();
        }

        private void UpdateUI() {
            _livesLabel.text = $"Lives: {_lives}";
            // _scoreLabel.text = $"Score: {_score}";
        }

        // Start is called before the first frame update
        private int _lives = 3;
        private int _score;
        
        [SerializeField]
        private TMP_Text _livesLabel;
        
        [SerializeField]
        private TMP_Text _scoreLabel;
    }
}
