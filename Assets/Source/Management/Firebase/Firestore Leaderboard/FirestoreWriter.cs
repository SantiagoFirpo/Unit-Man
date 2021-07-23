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
        private TMP_InputField _nameField;

        private LeaderData _leaderData;
        
        

        private void Awake()
        {
            _leaderData = new LeaderData("hhhhh", "Firpy", 981, true);
        }

        public void SubmitScore()
        {
            Debug.Log("Should write to db");
            FirebaseFirestore firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
            firestore.Document($"leaders/{_leaderData.PlayerID}").SetAsync(_leaderData);
        }
    }
}
