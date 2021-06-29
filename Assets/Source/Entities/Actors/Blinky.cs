using UnitMan.Source.Entities.Actors;

namespace UnitMan.Source
{
    public class Blinky : GhostController
    {
        public override void Initialize() {
            standardMoveSpeed = 3f;
            base.Initialize();
        }
    }
}