namespace UnitMan.Source
{
    public class Inky : Enemy
    {
        public override void Initialize() {
            base.moveSpeed = 4f;
            base.pathfindingIntervalSeconds = 3f;
            base.Initialize();
            
        }
    }
}