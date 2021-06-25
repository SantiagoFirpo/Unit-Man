using UnityEngine;

namespace UnitMan.Source
{
    public class InkyController : GhostController
    {
        [SerializeField]
        private GhostController blinkyController;
        public override void Initialize() {
            standardMoveSpeed = CLYDE_MOVE_SPEED;
            pathfindingIntervalSeconds = 3f;
            base.Initialize();
            
        }
        
        protected override void PollChasePosition()
        {
            currentTargetPosition = blinkyController.gridPosition +
                                    (playerController.gridPosition - blinkyController.gridPosition) * 2;
        }
    }
}