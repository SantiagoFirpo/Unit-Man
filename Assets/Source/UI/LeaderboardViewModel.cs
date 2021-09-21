using System;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Session.LocalLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnitMan.Source.UI
{
    public class LeaderboardViewModel : ViewModel
    {
        [FormerlySerializedAs("leaderCells")] [SerializeField]
        private LeaderCellView[] leaderUICells;

        [SerializeField]
        private GameObject threeDotsUp;
        
        [SerializeField]
        private GameObject threeDotsDown;
        
        private LeaderCellView _lastLeader;

        private bool _isSignedIn;


        public void InjectLeaderboard(Leaderboard leaderboard)
        {
            LocalLeaderData[] leaders = leaderboard.values;
            int leadersLength = leaders.Length;
            for (int i = 0; i < leadersLength; i++)
            {
                if (leaderUICells.Length - 1 < i) break;
                leaderUICells[i]?.InjectLeaderData(leaders[i]);
            }

            if (leadersLength < 5) return;
            threeDotsUp.SetActive(true);  
            _lastLeader = leaderUICells[leaderUICells.Length - 1];
            _isSignedIn = FirebaseAuthManager.Instance != null && FirebaseAuthManager.Instance.auth != null;
            if (!_isSignedIn)
            {
                _lastLeader.InjectLeaderData(leaders[leadersLength - 1]);
                _lastLeader.MarkAsLast();    
            }
            else
            {
                _lastLeader.InjectLeaderData(Array.Find(leaders, FindUserEntry));
                _lastLeader.MarkAsUserScore();
                threeDotsDown.SetActive(true);
            }
            
        }

        private bool FindUserEntry(LocalLeaderData obj)
        {
            if (!_isSignedIn) return false;
            return obj.playerId == FirebaseAuthManager.Instance.auth.CurrentUser.UserId;
        }
    }
}