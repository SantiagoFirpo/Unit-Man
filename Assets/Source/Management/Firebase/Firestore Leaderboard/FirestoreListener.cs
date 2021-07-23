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

		private string _scoreboardTextBuffer;

		private IEnumerable<LeaderData> _localLeaders;

		// Start is called before the first frame update
		private void Start()
		{
			FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
			firestore.Collection("leaders").Listen(ListenCallback);
			_scoreboardTextBuffer = "";
		}

		private void ListenCallback(QuerySnapshot dataSnapshot)
		{
			_scoreboardTextBuffer = "";
			_localLeaders = dataSnapshot.Documents.Select(DocumentToLeaderData).OrderByDescending(ScoreSorter);
			//TODO: add win based sorting
			foreach (LeaderData leader in _localLeaders)
			{
				Debug.Log(leader.PlayerDisplayName);
				Debug.Log(leader.Score);
				Debug.Log(leader.PlayerWon);
				string leaderWon = leader.PlayerWon ? "Yes" : "No";
				_scoreboardTextBuffer =
					$"{_scoreboardTextBuffer}Name: {leader.PlayerDisplayName} \n Score: {leader.Score} \n Player won? {leaderWon} \n \n";
			}

			scoreboardText.text = _scoreboardTextBuffer;


		}

		private static int ScoreSorter(LeaderData leader)
		{
			return leader.Score;
		}

		private static LeaderData DocumentToLeaderData(DocumentSnapshot snapshot) =>
			snapshot.ConvertTo<LeaderData>();
		
		public void GoToMainMenu()
		{
			SceneManager.LoadScene("Scenes/Main Menu");
		}
	}
}
