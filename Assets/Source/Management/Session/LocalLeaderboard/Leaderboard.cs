using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;

namespace UnitMan.Source.Management.Session.LocalLeaderboard
{
    [Serializable]
    public class Leaderboard
    {
        public LocalLeaderData[] values;

        public Leaderboard(IEnumerable<FirestoreLeaderData> firestoreLeaders)
        {
            FirestoreLeaderData[] firestoreLeaderArray = firestoreLeaders.ToArray(); //duplicating
            values = new LocalLeaderData[firestoreLeaderArray.Length];
            for (uint i = 0; i < firestoreLeaderArray.Length; i++)
            {
                FirestoreLeaderData currentLeader = firestoreLeaderArray[i];
                values[i] = new LocalLeaderData(currentLeader.PlayerDisplayName, currentLeader.Score, currentLeader.PlayerWon, i + 1, currentLeader.PlayerId);
            }
        }
    }
}