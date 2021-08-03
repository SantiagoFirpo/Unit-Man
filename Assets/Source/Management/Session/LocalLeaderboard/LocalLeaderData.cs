using System;

namespace UnitMan.Source.Management.Session.LocalLeaderboard
{
    [Serializable]
    public class LocalLeaderData
    {
        public string playerDisplayName;
        public int score;
        public bool playerWon;

        public LocalLeaderData(string playerDisplayName, int score, bool playerWon)
        {
            this.playerDisplayName = playerDisplayName;
            this.score = score;
            this.playerWon = playerWon;
        }

        public override string ToString()
        {
            return $"{this.score}		{this.playerDisplayName}";
        }
    }
}