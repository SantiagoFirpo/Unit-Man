using TMPro;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnityEngine;

namespace UnitMan
{
    public class LeaderCellController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text positionText;
        
        [SerializeField]
        private TMP_Text initialsText;
        
        [SerializeField]
        private TMP_Text scoreText;

        public void InjectLeaderData(LocalLeaderData leader)
        {
            positionText.SetText(leader.position.ToString());
            initialsText.SetText(leader.playerDisplayName);
            scoreText.SetText(leader.score.ToString());
        }

        public void MarkAsLast()
        {
            positionText.SetText($"{positionText.text} (LAST)");
        }

        public void MarkAsUserScore()
        {
            positionText.SetText($"{positionText.text} (YOU)");

        }
    }
}
