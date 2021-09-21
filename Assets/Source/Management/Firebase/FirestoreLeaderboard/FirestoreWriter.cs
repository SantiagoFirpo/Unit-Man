using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.FirestoreLeaderboard
{
    public class FirestoreWriter : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField nameField;
        
        [SerializeField]
        private TMP_Text scoreField;

        private FirestoreLeaderData _firestoreLeaderData;


        private void Start()
        {
            scoreField.SetText($"Score: {SessionViewModel.Instance.score}");
        }

        public void SubmitScore()
        {
            FirebaseUser currentUser = FirebaseAuthManager.Instance.auth.CurrentUser;
            _firestoreLeaderData = new FirestoreLeaderData(nameField.text.ToUpper(),
                                        SessionViewModel.Instance.score,
                                        SessionViewModel.Instance.won, currentUser.UserId);
            Debug.Log("Should write to db");
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaderboards/{CrossSceneLevelContainer.Instance.level.id}/leaders/{currentUser.UserId}").SetAsync(_firestoreLeaderData);
            GoToScoreboard();
            
        }

        public void GoToScoreboard()
        {
            SceneManager.LoadScene("Scenes/Scoreboard");
        }
    }
}
