using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class ClydeController : GhostController
    {
        protected override void ResolveDependencies() {
            standardMoveSpeed = PINKY_MOVE_SPEED * 0.8f;
            base.ResolveDependencies();
            pelletThreshold = 60;
            scatterTargetPosition = LevelGridController.Instance.level.bottomLeftPosition;
        }
        
        protected override void ResetActor()
        {
            base.ResetActor();
            SetState(State.Resting);
        }
        
        protected override void PollChaseTarget()
        {
            currentTargetPosition =
                LevelGridController.TaxiCabDistance(gridPosition, playerController.gridPosition) > 8
                    ? playerController.gridPosition //is further than 8 tiles away from pacman, target him
                    : bottomLeftMapBound; //is closer than 8 tiles away from pacman,
                                                                        //go to bottom left corner
        }
    }
}