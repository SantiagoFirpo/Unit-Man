using UnitMan.Source.LevelEditing;
using UnityEngine;

namespace UnitMan
{
    public class CrossSceneLevelContainer : MonoBehaviour
    {
        public static CrossSceneLevelContainer Instance { get; private set; }

        private Level _level;
        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyGameObject();
            }
        }

        private void DestroyGameObject()
        {
            Destroy(gameObject);
        }

        public void SetLevel(Level levelIn)
        {
            this._level = levelIn;
        }

        public void GetLevelAndDispose(out Level levelOut)
        {
            levelOut = this._level;
            DestroyGameObject();
        }
    }
}
