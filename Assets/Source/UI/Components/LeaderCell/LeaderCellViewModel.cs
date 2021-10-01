using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LeaderCell
{
    public class LeaderCellViewModel : ViewModel
    {
        [SerializeField]
        private OneWayBinding<uint> positionBinding;
        [SerializeField]
        private OneWayBinding<string> playerDisplayNameBinding;
        
        [SerializeField]
        private OneWayBinding<int> scoreBinding;

        [SerializeField]
        private OneWayBinding isLastBinding = new OneWayBinding();

        [SerializeField]
        private OneWayBinding isUserScoreBinding = new OneWayBinding();

        public void SetLocalLeaderState(LocalLeaderData leaderData)
        {
            positionBinding.SetValue(leaderData.position);
            playerDisplayNameBinding.SetValue(leaderData.playerDisplayName);
            scoreBinding.SetValue(leaderData.score);
        }

        public void MarkAsLast()
        {
            isLastBinding.Call();
        }

        public void MarkAsUserScore()
        {
            isUserScoreBinding.Call();
        }

        public override void OnRendered()
        {
        }
    }
}