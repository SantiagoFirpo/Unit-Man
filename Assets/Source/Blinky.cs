namespace UnitMan.Source
{
    public class Blinky : Enemy
    {
        public override void Initialize() {
            base.moveSpeed = 3f;
            base.pathfindingIntervalSeconds = 5f;
            base.Initialize();
        }
    }
}