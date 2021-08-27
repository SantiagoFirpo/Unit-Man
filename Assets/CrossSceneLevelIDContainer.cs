using UnityEngine;

namespace UnitMan
{
    public class CrossSceneLevelIDContainer : MonoBehaviour
    {
        public static CrossSceneLevelIDContainer Instance { get; private set; }
        private string _levelID;
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
                Destroy(gameObject);
            }
        }

        public void SetLevelID(string id)
        {
            _levelID = id;
        }

        public void GetLevelIdAndDispose(out string id)
        {
            id = _levelID;
            Destroy(this);
        }
    }
}
