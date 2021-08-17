using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.Management.Session;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI
{
    public class WinAndLoseController : MonoBehaviour
    {
        // Start is called before the first frame update
        private Timer _returnTimer;

        private void Start()
        {
            Debug.Log("Starting timer!");

            _returnTimer = new Timer(1f, false, true);
            _returnTimer.OnEnd += ReturnToMainScreen;
            _returnTimer.Start();
        }

        private void ReturnToMainScreen() {
            // Debug.Log("Timer ended!");
            _returnTimer.Stop();
            
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaders/{FirebaseAuthManager.Instance.auth.CurrentUser.UserId}").GetSnapshotAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        // Assert.IsNull(task.Exception);
                        FirestoreLeaderData userData = task.Result.ConvertTo<FirestoreLeaderData>();
                        SceneManager.LoadScene(userData.Score < SessionDataModel.Instance.score
                            ? "Score Query"
                            : "Scoreboard");
                    });
        }
    }
}
