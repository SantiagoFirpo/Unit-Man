using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnityEngine;

namespace UnitMan
{
    public class LeaderboardUIController : MonoBehaviour
    {
        [SerializeField]
        private LeaderCellController[] leaderCells;

        private LeaderCellController _lastLeader;

        public void InjectLeaderboard(Leaderboard leaderboard)
        {
            _lastLeader = leaderCells[leaderCells.Length];
            for (int i = 0; i < leaderboard.values.Length; i++)
            {
                if (leaderCells[i] is null)
                {
                    //TODO: inject the user's scoreboard into last cell if it's on the scoreboard
                }
                else
                {
                    leaderCells[i].InjectLeaderData(leaderboard.values[i]);
                }
            }
        }
    }
}
