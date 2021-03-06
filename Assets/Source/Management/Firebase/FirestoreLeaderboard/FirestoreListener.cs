using System.Collections.Generic;
using System.IO;
using System.Linq;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.Components.Leaderboard;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UnitMan.Source.Management.Firebase.FirestoreLeaderboard
{
	public class FirestoreListener : MonoBehaviour
	{
		[FormerlySerializedAs("_leaderboardUIController")] [SerializeField]
		private LeaderboardViewModel leaderboardViewModel;

		private string _leaderboardJson;

		private IEnumerable<FirestoreLeaderData> _firestoreLeaders;

		private Leaderboard _localLeaderboard;

		// Start is called before the first frame update
		private void Start()
		{
			FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
			firestore.Collection($"leaderboards/{CrossSceneLevelContainer.Instance.level.id}/leaders").Listen(ListenCallback);
		}

		private void ListenCallback(QuerySnapshot dataSnapshot)
		{
			_firestoreLeaders = dataSnapshot.Documents.Select(DocumentToLeaderData).OrderByDescending(ScoreSorter);
			_leaderboardJson = JsonUtility.ToJson(new Leaderboard(_firestoreLeaders), true);

			_localLeaderboard = JsonUtility.FromJson<Leaderboard>(_leaderboardJson);
			
			leaderboardViewModel.InjectLeaderboard(_localLeaderboard);
			Debug.Log(_leaderboardJson);
			

			SaveStringIntoJson(_leaderboardJson, "leaderboard");
			

		}

		public static void SaveStringIntoJson(string json, string fileName){
			File.WriteAllText($"{FilePaths.LevelsPath}/{fileName}.json", json);
			Debug.Log($"Saved {fileName}.json to {Application.persistentDataPath}/levels");
		}

		public static string LoadStringFromJson(string fileName)
		{
			return File.ReadAllText($"{FilePaths.LevelsPath}/{fileName}.json");
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
