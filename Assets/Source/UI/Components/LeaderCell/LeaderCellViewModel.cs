using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.LeaderCell
{
    public class LeaderCellViewModel : ViewModel
    {
        [SerializeField]
        private ReactiveProperty<uint> positionBinding;
        [SerializeField]
        private ReactiveProperty<string> playerDisplayNameBinding;
        
        [SerializeField]
        private ReactiveProperty<int> scoreBinding;

        [SerializeField]
        private Event isLastBinding = new Event();

        [SerializeField]
        private Event isUserScoreBinding = new Event();

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