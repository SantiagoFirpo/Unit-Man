using UnitMan.Source.Entities.Actors;

namespace UnitMan.Source
{
    public class PinkyController : GhostController
    {
        public override void Initialize()
        {
            standardMoveSpeed = INKY_BLINKY_PINKY_MOVE_SPEED;
            base.Initialize();
        }

        protected override void PollChasePosition()
        {
            currentTargetPosition = playerController.gridPosition + (playerController.currentDirection * 3);
        }
    }
}