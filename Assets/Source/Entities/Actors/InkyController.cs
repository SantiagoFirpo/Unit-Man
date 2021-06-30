using UnityEngine;

namespace UnitMan.Source.Entities.Actors
{
    public class InkyController : GhostController
    {
        //TODO: set target to initialPosition first and then chase position when pellet threshold is achieved
        
        [SerializeField]
        private GhostController blinkyController;
        public override void Initialize() {
            standardMoveSpeed = INKY_BLINKY_PINKY_MOVE_SPEED;
            base.Initialize();
            pelletThreshold = 30;

        }
        
        protected override void PollChasePosition()
        {
            currentTargetPosition = blinkyController.gridPosition +
                                    (playerController.gridPosition - blinkyController.gridPosition) * 2;
        }
    }
}