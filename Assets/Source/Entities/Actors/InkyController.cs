using UnityEngine;

namespace UnitMan.Source.Entities.Actors
{
    public class InkyController : GhostController
    {
        //TODO: set target to initialPosition first and then chase position when pellet threshold is achieved
        
        [SerializeField]
        private GhostController blinkyController;
        public override void Initialize() {
            standardMoveSpeed = CLYDE_MOVE_SPEED;
            base.Initialize();
            
        }
        
        protected override void PollChasePosition()
        {
            currentTargetPosition = blinkyController.gridPosition +
                                    (playerController.gridPosition - blinkyController.gridPosition) * 2;
        }
    }
}