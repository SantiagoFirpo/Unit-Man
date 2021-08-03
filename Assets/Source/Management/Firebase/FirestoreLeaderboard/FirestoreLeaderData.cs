using Firebase.Firestore;

namespace UnitMan.Source.Management.Firebase.FirestoreLeaderboard
{
	[FirestoreData]
	public struct FirestoreLeaderData
	{
		[FirestoreProperty]
		public string PlayerDisplayName { get; set; }
		
		[FirestoreProperty]
		public int Score { get; set; }
		
		[FirestoreProperty]
		public bool PlayerWon { get; set; }

		

		public FirestoreLeaderData(string playerDisplayName, int score, bool playerWon)
		{
			PlayerDisplayName = playerDisplayName;
			Score = score;
			PlayerWon = playerWon;
		}
	}
}