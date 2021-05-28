using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan
{
    public class PelletController : MonoBehaviour
    {
        private GameObject _gameObject;
        protected int scoreValue = 1;

        protected virtual void Awake() {
            _gameObject = gameObject;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            _gameObject.SetActive(false);
            UpdatePlayerState();
            ScoreManager.Instance.score += scoreValue;
        }

        protected virtual void UpdatePlayerState() {
            
        }
    }
}
