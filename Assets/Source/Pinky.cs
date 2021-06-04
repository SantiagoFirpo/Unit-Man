namespace UnitMan.Source
{
    public class Pinky : Enemy
    {
        public override void Initialize() {
            standardMoveSpeed = 4.5f;
            pathfindingIntervalSeconds = 10f;
            base.Initialize();
            
        }
    }
}