using UnityEngine;

namespace UnitMan.Source.LevelEditing
{
    public class WrapController : MonoBehaviour
    {
        private WrapController _destination;

        public void SetDestination(WrapController newDestination)
        {
            if (_destination != null) return;
            _destination = newDestination;
        }

        public void SyncWithDestination()
        {
            _destination._destination = this;
        }
    }
}