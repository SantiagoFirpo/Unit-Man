using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.Management.Firebase.Firestore_Leaderboard
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
				Debug.Log(leader.PlayerDisplayName);
				Debug.Log(leader.Score);
				Debug.Log(leader.PlayerWon);
				string leaderWon = leader.PlayerWon ? "Yes" : "No";
				_scoreboardTextBuffer = $"{_scoreboardTextBuffer} \n {leader.Score}		{leader.PlayerDisplayName}";

				// $"{_scoreboardTextBuffer}Name: {leader.PlayerDisplayName} \n Score: {leader.Score} \n Player won? {leaderWon} \n \n";
			}

			scoreboardText.SetText(_scoreboardTextBuffer);


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
