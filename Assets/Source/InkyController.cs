using UnityEngine;

namespace UnitMan.Source
{
    public class InkyController : GhostController
    {
        [SerializeField]
        private GhostController blinkyController;
        public override void Initialize() {
            standardMoveSpeed = 3.5f;
            pathfindingIntervalSeconds = 3f;
            base.Initialize();
            
        }
        
        protected override void PollPlayerPosition()
        {
            currentTargetPosition = blinkyController.gridPosition +
                                    (playerController.gridPosition - blinkyController.gridPosition) * 2;
        }
    }
}