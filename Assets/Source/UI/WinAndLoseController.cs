using System;
using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing.Online;
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

            _returnTimer = new Timer(1f, true, true);
            _returnTimer.OnEnd += ReturnToMainScreen;
        }

        private static void ReturnToMainScreen() {
            // Debug.Log("Timer ended!");
            // _returnTimer.Stop();
            Debug.Log("Will now attempt to get player score");
            try
            {
                FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
                firestore.Document($"leaderboards/{CrossSceneLevelContainer.Instance.GetLevel().id}/leaders/{FirebaseAuthManager.Instance.auth.CurrentUser.UserId}").GetSnapshotAsync()
                    .ContinueWithOnMainThread(
                        task =>
                        {
                            // Assert.IsNull(task.Exception);
                            AggregateException aggregateException = task.Exception;
                            if (aggregateException != null)
                            {
                                Debug.LogException(aggregateException);
                            }
                            Debug.Log("Success!");
                            FirestoreLeaderData userData = task.Result.ConvertTo<FirestoreLeaderData>();
                            SceneManager.LoadScene(userData.Score < SessionDataModel.Instance.score
                                ? "Score Query"
                                : "Scoreboard");
                            Debug.Log("WHAT?");
                        });
            }
            catch (Exception e)
            {
                Debug.Log("Error! Entry probably doesn't exist!");
                Debug.LogException(e);
                SceneManager.LoadScene("Score Query");
            }
        }
    }
}
