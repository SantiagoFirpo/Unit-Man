using Firebase.Auth;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.FirestoreLeaderboard
{
    public class FirestoreWriter : MonoBehaviour
    {
        private FirestoreLeaderData _firestoreLeaderData;

        public void SubmitScore(string playerName)
        {
            FirebaseUser currentUser = FirebaseAuthManager.Instance.auth.CurrentUser;
            _firestoreLeaderData = new FirestoreLeaderData(playerName,
                                        SessionViewModel.Instance.score,
                                        SessionViewModel.Instance.won, currentUser.UserId);
            Debug.Log("Should write to db");
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaderboards/{CrossSceneLevelContainer.Instance.level.id}/leaders/{currentUser.UserId}").SetAsync(_firestoreLeaderData);
            GoToScoreboard();
            
        }

        public static void GoToScoreboard()
        {
            SceneManager.LoadScene("Scenes/Scoreboard");
        }
    }
}
