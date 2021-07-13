using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors
{
    public class InkyController : GhostController
    {
        
        [SerializeField]
        private GhostController blinkyController;
        public override void Initialize() {
            standardMoveSpeed = INKY_BLINKY_PINKY_MOVE_SPEED + 0.1f;
            base.Initialize();
            pelletThreshold = 30;
            scatterTargetPosition = LevelGridController.Instance.mazeData.bottomLeftMapPosition;

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