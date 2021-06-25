namespace UnitMan.Source
{
    public class Pinky : Enemy
    {
        public override void Initialize() {
            
            standardMoveSpeed = CLYDE_MOVE_SPEED;
            pathfindingIntervalSeconds = 2f;
            base.Initialize();
            
        }
        
        protected override void PollPlayerPosition()
        {
            currentTargetPosition = playerController.gridPosition + (playerController.currentDirection * 3);
        }
    }
}