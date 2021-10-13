using System;
using UnityEngine;

namespace UnitMan.Source.LevelEditing.Online
{
    public class CrossSceneLevelContainer : MonoBehaviour
    {
        public static CrossSceneLevelContainer Instance { get; private set; }

        [NonSerialized]
        public Level level;

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
    }
}
