using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class PinkyController : GhostController
    {
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            standardMoveSpeed = PINKY_MOVE_SPEED;
            pelletThreshold = 3;
            scatterTargetPosition = LevelGridController.Instance.level.topLeftPosition;
        }

        protected override void ResetActor()
        {
            base.ResetActor();
            SetState(GhostState.Resting);
        }

        protected override void PollChaseTarget()
        {
            currentTargetPosition = playerController.gridPosition + (playerController.currentDirection * 3);
        }
    }
}