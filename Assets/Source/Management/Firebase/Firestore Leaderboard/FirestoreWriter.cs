using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.Firestore_Leaderboard
{
    public class FirestoreWriter : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField nameField;

        private LeaderData _leaderData;
        
        

        // private void Awake()
        // {
        //     _leaderData = new LeaderData(FirebaseAuthManager.Instance.auth.CurrentUser.UserId, "Firpy", 0, false);
        // }

        public void SubmitScore()
        {
            FirebaseUser currentUser = FirebaseAuthManager.Instance.auth.CurrentUser;
            _leaderData = new LeaderData(currentUser.UserId,
                                        nameField.text,
                                        SessionDataModel.Instance.score,
                                        SessionDataModel.Instance.won);
            Debug.Log("Should write to db");
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaders/{_leaderData.PlayerID}").SetAsync(_leaderData);
            GoToScoreboard();
            
        }

        public void GoToScoreboard()
        {
            SceneManager.LoadScene("Scenes/Scoreboard");
        }
    }
}
