using System;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.Management.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.Firestore_Leaderboard
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
            scoreField.SetText($"Score: {SessionDataModel.Instance.score}");
        }

        public void SubmitScore()
        {
            FirebaseUser currentUser = FirebaseAuthManager.Instance.auth.CurrentUser;
            _firestoreLeaderData = new FirestoreLeaderData(nameField.text.ToUpper(),
                                        SessionDataModel.Instance.score,
                                        SessionDataModel.Instance.won);
            Debug.Log("Should write to db");
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaders/{currentUser.UserId}").SetAsync(_firestoreLeaderData);
            GoToScoreboard();
            
        }

        public void GoToScoreboard()
        {
            SceneManager.LoadScene("Scenes/Scoreboard");
        }
    }
}
