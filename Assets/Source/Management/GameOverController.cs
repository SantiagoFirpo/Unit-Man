using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management
{
    public class GameOverController : MonoBehaviour
    {
        // Start is called before the first frame update
        private Timer _returnTimer;

        private void Awake() {
            _returnTimer = new Timer(1f, true, true);
            _returnTimer.OnEnd += ReturnToMainScreen;
        }

        private void ReturnToMainScreen() {
            _returnTimer.Stop();
            SceneManager.LoadScene("Main Menu");
        }
    }
}
