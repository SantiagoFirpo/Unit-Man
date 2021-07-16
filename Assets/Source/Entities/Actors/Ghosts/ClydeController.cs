using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public class ClydeController : GhostController
    {
        public override void Initialize() {
            standardMoveSpeed = INKY_BLINKY_PINKY_MOVE_SPEED;
            base.Initialize();
            pelletThreshold = 60;
            scatterTargetPosition = LevelGridController.Instance.mazeData.bottomLeftMapPosition;
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