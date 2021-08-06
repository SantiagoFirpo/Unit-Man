using System;

namespace UnitMan.Source.Management.Session.LocalLeaderboard
{
    [Serializable]
    public class LocalLeaderData
    {
        public string playerDisplayName;
        public string playerId;
        public int score;
        public bool playerWon;
        public uint position;

        public LocalLeaderData(string playerDisplayName, int score, bool playerWon, uint position, string playerId)
        {
            this.playerDisplayName = playerDisplayName;
            this.playerId = playerId;
            this.score = score;
            this.playerWon = playerWon;
            this.position = position;
        }

        public override string ToString()
        {
            return $"{this.score}		{this.playerDisplayName}";
        }
    }
}