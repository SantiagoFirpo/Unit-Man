using UnityEngine;

namespace UnitMan.Source
{
    public class Inky : Enemy
    {
        [SerializeField]
        private Enemy blinkyController;
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