namespace UnitMan.Source
{
    public class Pinky : Enemy
    {
        public override void Initialize() {
            base.moveSpeed = 4.5f;
            base.pathfindingIntervalSeconds = 10f;
            base.Initialize();
            
        }
    }
}