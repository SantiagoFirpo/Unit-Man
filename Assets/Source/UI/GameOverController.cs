using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI
{
    public class GameOverController : MonoBehaviour
    {
        // Start is called before the first frame update
        private Timer _returnTimer;

        private void Awake() {
            _returnTimer = new Timer(1f, false, true);
            Debug.Log("Starting timer!");
            _returnTimer.Start();
            _returnTimer.OnEnd += ReturnToMainScreen;
        }

        private void ReturnToMainScreen() {
            Debug.Log("Timer ended!");
            _returnTimer.Stop();
            SceneManager.LoadScene("Main Menu");
        }
    }
}
