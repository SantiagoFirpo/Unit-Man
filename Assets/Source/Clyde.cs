namespace UnitMan.Source
{
    public class Clyde : Enemy
    {
        public override void Initialize() {
            standardMoveSpeed = 2f;
            pathfindingIntervalSeconds = 2f;
            base.Initialize();
            
        }
    }
}