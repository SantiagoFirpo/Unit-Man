namespace UnitMan.Source
{
    public class Clyde : Enemy
    {
        public override void Initialize() {
            base.moveSpeed = 2f;
            base.pathfindingIntervalSeconds = 2f;
            base.Initialize();
            
        }
    }
}