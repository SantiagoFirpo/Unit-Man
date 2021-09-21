using System.Threading.Tasks;
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
            _returnTimer.OnEnd += CheckScores;
        }

        private static void CheckScores()
        {
            if (FirebaseAuthManager.Instance.auth is null)
            {
                SceneManager.LoadScene("Scenes/Scoreboard");
            }
            else
            {
                Debug.Log("Starting check...");
                CollectionReference remoteLeaderboard = FirebaseFirestore.DefaultInstance.Collection(
                    $"leaderboards/{CrossSceneLevelContainer.Instance.level.id}/leaders");
                if (remoteLeaderboard is null)
                {
                    Debug.Log("Leaderboard is null!");
                    GoToScoreQuery();
                }
                else
                {
                    Debug.Log("Leaderboard is not null! Proceeding...");
                    remoteLeaderboard.Document(FirebaseAuthManager.Instance.auth.CurrentUser.UserId).GetSnapshotAsync()
                        .ContinueWithOnMainThread(
                            LeaderboardCallback);    
                }
            }
            

            
        }

        private static void LeaderboardCallback(Task<DocumentSnapshot> task)
        {
            if (task.Exception is { }) return;
            DocumentSnapshot leaderSnapshot = task.Result;
            if (leaderSnapshot.Exists)
            {
                FirestoreLeaderData firestoreLeader = leaderSnapshot.ConvertTo<FirestoreLeaderData>();
                Debug.Log("Will now switch scenes");
                SceneManager.LoadScene(firestoreLeader.Score >= SessionViewModel.Instance.score
                    ? "Scenes/Scoreboard"
                    : "Scenes/Score Query");
            }
            else
            {
                Debug.Log("Result is null");
                GoToScoreQuery();
            }
        }

        private static void GoToScoreQuery()
        {
            SceneManager.LoadScene("Scenes/Score Query");
        }
    }
}
