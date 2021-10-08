using UnityEngine;

namespace UnitMan.Source.Utilities
{
    public class LevelIdToDeleteContainer : MonoBehaviour
    {
        public static LevelIdToDeleteContainer Instance { get; private set; }

        public string levelId;

        public string levelName;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;
        }
    }
}