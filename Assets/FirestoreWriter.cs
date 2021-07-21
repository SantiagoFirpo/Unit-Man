using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using TMPro;
using UnityEngine;

namespace UnitMan
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
