using UnityEngine;

namespace UnitMan.Source.Management
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int score = 0;
        // Start is called before the first frame update
        private void Awake() {
            if (Instance != null) {GameObject.Destroy(gameObject);}
            Instance = this;
        }
    }
}
