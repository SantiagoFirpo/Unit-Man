using System.Collections.Generic;
using System.IO;
using System.Linq;
using Firebase.Firestore;
using TMPro;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.FirestoreLeaderboard
{
	public class FirestoreListener : MonoBehaviour
	{
		[SerializeField] private TMP_Text scoreboardText;

		private Leaderboard _leaderboard;

		private string _scoreboardTextBuffer;
		private string _leaderboardJson;

		private IEnumerable<FirestoreLeaderData> _localLeaders;

		// Start is called before the first frame update
		private void Start()
		{
			FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
			firestore.Collection("leaders").Listen(ListenCallback);
			_scoreboardTextBuffer = "";
		}

		private void ListenCallback(QuerySnapshot dataSnapshot)
		{
			_localLeaders = dataSnapshot.Documents.Select(DocumentToLeaderData).OrderByDescending(ScoreSorter);
			_leaderboard = new Leaderboard(_localLeaders.ToArray());
			_leaderboardJson = JsonUtility.ToJson(_leaderboard, true);
			//TODO: add win based sorting
			_scoreboardTextBuffer = "SCORE			NAME";
			Debug.Log(_leaderboardJson);
				
			foreach (LocalLeaderData leader in _leaderboard.values)
			{
				Debug.Log(leader.playerDisplayName);
				Debug.Log(leader.score);
				Debug.Log(leader.playerWon);
				// string leaderWon = leader.playerWon ? "Yes" : "No";
				_scoreboardTextBuffer = $"{_scoreboardTextBuffer} \n {leader.score}		{leader.playerDisplayName}";
			
			}
			SaveStringIntoJson(_leaderboardJson);

			scoreboardText.SetText(_scoreboardTextBuffer);


		}
		
		public static void SaveStringIntoJson(string json){
			File.WriteAllText($"{Application.persistentDataPath}/leaderboard.json", json);
			Debug.Log($"Saved leaderboard.json to {Application.persistentDataPath}");
		}

		private static int ScoreSorter(FirestoreLeaderData firestoreLeader)
		{
			return firestoreLeader.Score;
		}

		private static FirestoreLeaderData DocumentToLeaderData(DocumentSnapshot snapshot) =>
			snapshot.ConvertTo<FirestoreLeaderData>();
		
		public void GoToMainMenu()
		{
			SceneManager.LoadScene("Scenes/Main Menu");
		}
	}
}
