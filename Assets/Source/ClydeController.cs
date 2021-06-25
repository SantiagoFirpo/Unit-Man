using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source
{
    public class ClydeController : GhostController
    {
        public override void Initialize() {
            standardMoveSpeed = CLYDE_MOVE_SPEED;
            pathfindingIntervalSeconds = 2f;
            base.Initialize();
            
        }
        
        protected override void PollPlayerPosition()
        {
            currentTargetPosition =
                PathGrid.TaxiCabDistance(gridPosition, playerController.gridPosition) > 8
                    ? playerController.gridPosition //is further than 8 tiles away from pacman, target him
                    : PathGrid.VectorToVector2Int(_bottomLeftMapBound); //is closer than 8 tiles away from pacman,
                                                                        //go to bottom left corner
        }
    }
}