namespace UnitMan.Source
{
    public class Blinky : Enemy
    {
        public override void Initialize() {
            standardMoveSpeed = 3f;
            pathfindingIntervalSeconds = 5f;
            base.Initialize();
        }
    }
}