namespace UnitMan.Source
{
    public class Pinky : Enemy
    {
        public override void Initialize() {
            base.moveSpeed = 5f;
            base.pathfindingIntervalSeconds = 2f;
            base.Initialize();
            
        }
    }
}