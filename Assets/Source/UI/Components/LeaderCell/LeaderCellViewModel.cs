﻿using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LeaderCell
{
    public class LeaderCellViewModel : ViewModel
    {
        [SerializeField]
        private Reactive<uint> positionBinding;
        [SerializeField]
        private Reactive<string> playerDisplayNameBinding;
        
        [SerializeField]
        private Reactive<int> scoreBinding;

        [SerializeField]
        private ReactiveEvent isLastBinding = new ReactiveEvent();

        [SerializeField]
        private ReactiveEvent isUserScoreBinding = new ReactiveEvent();

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