using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class InkyController : GhostController
    {
        
        // [SerializeField]
        private GhostController _blinkyController;

        protected override void ResolveDependencies() {
            base.ResolveDependencies();
            _blinkyController = FindObjectOfType<BlinkyController>() == null
                ? (GhostController)this
                : FindObjectOfType<BlinkyController>();
            standardMoveSpeed = PINKY_MOVE_SPEED * 0.9f;
            pelletThreshold = 30;
            scatterTargetPosition = LevelGridController.Instance.level.bottomRightPosition;

        }

        protected override void ResetActor()
        {
            base.ResetActor();
            SetState(GhostState.Resting);
        }


        protected override void PollChaseTarget()
        {
            currentTargetPosition = _blinkyController.gridPosition +
                                    (playerController.gridPosition - _blinkyController.gridPosition) * 2;
        }
    }
}