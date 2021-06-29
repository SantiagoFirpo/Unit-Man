using UnitMan.Source.Utilities.Pathfinding;

namespace UnitMan.Source
{
    public class ClydeController : GhostController
    {
        //TODO: set target to initialPosition first and then chase position when pellet threshold is achieved
        public override void Initialize() {
            standardMoveSpeed = CLYDE_MOVE_SPEED;
            base.Initialize();
            currentTargetPosition = PathGrid.VectorToVector2Int(StartPosition);
        }
        
        protected override void PollChasePosition()
        {
            currentTargetPosition =
                PathGrid.TaxiCabDistance(gridPosition, playerController.gridPosition) > 8
                    ? playerController.gridPosition //is further than 8 tiles away from pacman, target him
                    : PathGrid.VectorToVector2Int(bottomLeftMapBound); //is closer than 8 tiles away from pacman,
                                                                        //go to bottom left corner
        }
    }
}