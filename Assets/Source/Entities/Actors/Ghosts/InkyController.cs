using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class InkyController : GhostController
    {
        
        [SerializeField]
        private GhostController blinkyController;

        protected override void Initialize() {
            base.Initialize();
            standardMoveSpeed = PINKY_MOVE_SPEED * 0.9f;
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