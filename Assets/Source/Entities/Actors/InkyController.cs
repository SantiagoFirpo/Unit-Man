using UnitMan.Source.Utilities.Pathfinding;
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
            base.pelletThreshold = 30;
            _scatterTargetPosition = LevelGridController.Instance.mazeData.bottomLeftMapPosition;

        }

        protected override void ResetActor()
        {
            base.ResetActor();
            SetState(State.Resting);
        }

        protected override void PollChaseTarget()
        {
            currentTargetPosition = blinkyController.gridPosition +
                                    (playerController.gridPosition - blinkyController.gridPosition) * 2;
        }
    }
}