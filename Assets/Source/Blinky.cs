namespace UnitMan.Source
{
    public class Blinky : GhostController
    {
        public override void Initialize() {
            standardMoveSpeed = 3f;
            pathfindingIntervalSeconds = 5f;
            base.Initialize();
        }
    }
}