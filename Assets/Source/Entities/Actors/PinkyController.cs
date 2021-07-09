using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors
{
    public class PinkyController : GhostController
    {
        public override void Initialize()
        {
            standardMoveSpeed = INKY_BLINKY_PINKY_MOVE_SPEED;
            base.Initialize();
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