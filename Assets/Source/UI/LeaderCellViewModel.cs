﻿using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI
{
    public class LeaderCellViewModel : ViewModel
    {
        [SerializeField]
        private OneWayBinding<uint> positionBinding = new OneWayBinding<uint>();
        [SerializeField]
        private OneWayBinding<string> playerDisplayNameBinding = new OneWayBinding<string>();
        
        [SerializeField]
        private OneWayBinding<int> scoreBinding = new OneWayBinding<int>();

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
    }
}