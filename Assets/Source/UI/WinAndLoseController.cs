using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.Firestore_Leaderboard;
using UnitMan.Source.Management.Session;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI
{
    public class WinAndLoseController : MonoBehaviour
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
            // Debug.Log("Timer ended!");
            _returnTimer.Stop();
            
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaders/{FirebaseAuthManager.Instance.auth.CurrentUser.UserId}").GetSnapshotAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        Assert.IsNull(task.Exception);
                        LeaderData userData = task.Result.ConvertTo<LeaderData>();
                        if (userData.Score < SessionDataModel.Instance.score)
                        {
                            SceneManager.LoadScene("Score Query");
                        }
                        else
                        {
                            SceneManager.LoadScene("Scoreboard");
                        }
                    });
        }
    }
}
