namespace UnitMan.Source
{
    public class Inky : Enemy
    {
        public override void Initialize() {
            standardMoveSpeed = 4f;
            pathfindingIntervalSeconds = 3f;
            base.Initialize();
            
        }
    }
}