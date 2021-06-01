namespace UnitMan.Source
{
    public class Inky : Enemy
    {
        protected override void Awake() {
            base.Awake();
            moveSpeed = 4f;
            pathfindingIntervalSeconds = 3f;
        }
    }
}