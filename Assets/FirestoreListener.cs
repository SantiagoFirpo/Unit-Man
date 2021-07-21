using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;
using UnityEngine;

namespace UnitMan
{
    public class FirestoreListener : MonoBehaviour
    {
	    private IEnumerable<LeaderData> _localLeaders;
        // Start is called before the first frame update
        private void Start()
        {
	        FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
	        firestore.Collection("leaders").Listen(ListenCallback);
        }

        private void ListenCallback(QuerySnapshot dataSnapshot)
        {
	        
	        _localLeaders = dataSnapshot.Documents.Select(DocumentToLeaderData);
	        foreach (LeaderData leader in _localLeaders)
	        {
		        Debug.Log(leader.PlayerDisplayName);
		        Debug.Log(leader.Score);
		        Debug.Log(leader.PlayerWon);
	        }
        }

        private static LeaderData DocumentToLeaderData(DocumentSnapshot snapshot) => snapshot.ConvertTo<LeaderData>();
    }
	}
