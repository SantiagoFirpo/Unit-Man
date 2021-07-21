using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

namespace UnitMan
{
    public class FirestoreListener : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
	        FirebaseFirestore firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
	        firestore.Document("leaders/hhhhh").Listen(dataSnapshot =>
	        {
		        LeaderData leaderData = dataSnapshot.ConvertTo<LeaderData>();
		        Debug.Log(leaderData.PlayerDisplayName);
		        Debug.Log(leaderData.PlayerID);
		        Debug.Log(leaderData.Score);
		        Debug.Log(leaderData.PlayerWon);
	        });
        }
    }
	}
