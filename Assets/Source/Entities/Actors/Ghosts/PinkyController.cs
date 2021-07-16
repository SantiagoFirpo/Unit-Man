using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class PinkyController : GhostController
    {
        public override void Initialize()
        {
            base.Initialize();
            standardMoveSpeed = PINKY_MOVE_SPEED;
            scatterTargetPosition = LevelGridController.Instance.mazeData.topLeftMapPosition;
        }

        protected override void ResetActor()
        {
            base.ResetActor();
            SetState(State.Resting);
        }

        protected override void PollChaseTarget()
        {
            currentTargetPosition = playerController.gridPosition + (playerController.currentDirection * 3);
        }
    }
}