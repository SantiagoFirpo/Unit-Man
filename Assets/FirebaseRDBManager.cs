using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnitMan.Source.Management.FirebaseManagement;

namespace UnitMan
{
    public class FirebaseRDBManager : MonoBehaviour
    {
        public DatabaseReference rdb;
        
        public FirebaseRDBManager Instance { get; private set; }

        public string userId;
        public string displayName;
        
        

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
                AddScoreboardEntry(50);
            }
            
        }

        private void InitializeDatabase()
        {
            rdb = FirebaseDatabase.DefaultInstance.RootReference;
            userId = FirebaseAuthManager.Instance.auth.CurrentUser.UserId;
            displayName = FirebaseAuthManager.Instance.auth.CurrentUser.DisplayName;
        }

        public void AddScoreboardEntry(int score)
        {
            rdb.Child("leaders").Child(userId).Child("score").SetValueAsync(score);
        }
    }
}
