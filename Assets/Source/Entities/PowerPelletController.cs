using System;
using UnitMan.Source.Entities.Actors;
using UnitMan.Source.Management;

namespace UnitMan.Source.Entities
{
    public class PowerPelletController : PelletController
    {
        public static event Action OnPowerPelletCollected;
        private PlayerController _playerController;
        protected override void Awake() {
            base.Awake();
            scoreValue = 50;
            _playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
        }


        protected override void UpdateSessionState()
        {
            OnPowerPelletCollected?.Invoke();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Fleeing, 1, true);
        }

        
    }
}
