using System;
using UnitMan.Source.Management;
using UnityEngine;

namespace UnitMan.Source.Entities
{
    public class PelletController : MonoBehaviour
    {
        private GameObject _gameObject;
        protected int scoreValue = 10;

        public static event Action OnPelletEaten;

        protected virtual void Awake() {
            _gameObject = gameObject;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            _gameObject.SetActive(false);
            UpdateSessionState();
            SessionDataModel.Instance.IncrementScore(scoreValue);
            // if (!AudioManager.Instance.IsTrackPlaying(0)) {
                AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Munch, 0, false);
            // }
            
            
        }

        protected virtual void UpdateSessionState() {
            SessionDataModel.Instance.pelletsEaten++;
            SessionManagerSingle.CheckIfGameIsWon();
            OnPelletEaten?.Invoke();
        }
    }
}
