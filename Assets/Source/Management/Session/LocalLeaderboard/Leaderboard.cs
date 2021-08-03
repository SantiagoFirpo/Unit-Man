using System;
using System.Collections.Generic;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;

namespace UnitMan.Source.Management.Session.LocalLeaderboard
{
    [Serializable]
    public class Leaderboard
    {
        public List<LocalLeaderData> values;

        public Leaderboard(FirestoreLeaderData[] firestoreLeaders)
        {
            int firestoreLeadersLength = firestoreLeaders.Length;
            values = new List<LocalLeaderData>(firestoreLeadersLength);
            foreach (FirestoreLeaderData firestoreLeader in firestoreLeaders)
            {
                values.Add(new LocalLeaderData(firestoreLeader.PlayerDisplayName,
                    firestoreLeader.Score,
                    firestoreLeader.PlayerWon));
            }
            {
                
            }
        }
    }
}