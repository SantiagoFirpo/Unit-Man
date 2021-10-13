using UnitMan.Source.LevelEditing;
using UnityEngine;

namespace UnitMan.Source.Utilities
{
    public class LevelIdToDeleteContainer : MonoBehaviour
    {
        public static LevelIdToDeleteContainer Instance { get; private set; }
        public Level level;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;
        }
    }
}