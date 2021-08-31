using UnityEngine;

namespace UnitMan.Source.LevelEditing.Online
{
    public class CrossSceneLevelContainer : MonoBehaviour
    {
        public static CrossSceneLevelContainer Instance { get; private set; }

        private Level _level;

        // public string id;
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

        public Level GetLevel()
        {
            return this._level;

        }
    }
}
