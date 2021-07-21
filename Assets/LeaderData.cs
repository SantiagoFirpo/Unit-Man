using Firebase.Firestore;

namespace UnitMan
{
	[FirestoreData]
	public struct LeaderData
	{
		[FirestoreProperty]
		public string PlayerID { get; set; }
		
		[FirestoreProperty]
		public string PlayerDisplayName { get; set; }
		
		[FirestoreProperty]
		public int Score { get; set; }
		
		[FirestoreProperty]
		public bool PlayerWon { get; set; }

		public LeaderData(string playerID, string playerDisplayName, int score, bool playerWon)
		{
			PlayerID = playerID;
			PlayerDisplayName = playerDisplayName;
			Score = score;
			PlayerWon = playerWon;
		}
	}
}