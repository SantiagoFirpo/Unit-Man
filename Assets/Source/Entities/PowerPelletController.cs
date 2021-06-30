using UnitMan.Source.Entities.Actors;
using UnitMan.Source.Management;

namespace UnitMan.Source.Entities
{
    public class PowerPelletController : PelletController
    {
        private PlayerController _playerController;
        protected override void Awake() {
            base.Awake();
            scoreValue = 50;
            _playerController = SessionManagerSingle.Instance.player.GetComponent<PlayerController>();
        }


        protected override void UpdateSessionState() { 
            _playerController.SetInvincible();
        }
    }
}
