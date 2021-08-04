using TMPro;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnityEngine;

namespace UnitMan
{
    public class LeaderboardUIController : MonoBehaviour
    {
        [SerializeField]
        private LeaderCellController[] leaderCells;

        [SerializeField]
        private TMP_Text threeDots;

        private LeaderCellController _lastLeader;

        private const string THREE_DOTS = "...";

        public void InjectLeaderboard(Leaderboard leaderboard)
        {
            LocalLeaderData[] leaders = leaderboard.values;
            int leadersLength = leaders.Length;
            if (leadersLength > 3)
            {
                threeDots.SetText(THREE_DOTS);
                _lastLeader = leaderCells[leaderCells.Length - 1];
                _lastLeader.InjectLeaderData(leaders[leadersLength - 1]);
            }
            
            for (int i = 0; i < leadersLength; i++)
            {
                if (leaderCells[i] is null)
                {
                    //TODO: inject the user's scoreboard into last cell if it's on the scoreboard
                }
                else
                {
                    leaderCells[i].InjectLeaderData(leaders[i]);
                }
            }
        }
    }
}
