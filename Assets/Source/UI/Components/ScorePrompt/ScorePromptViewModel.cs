using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.Management.Session;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.ScorePrompt
{
    public class ScorePromptViewModel : ViewModel
    {
        [SerializeField]
        private OneWayBinding<int> score;
        private string _playerName;

        [SerializeField]
        private FirestoreWriter firestoreWriter;

        public void OnNameChanged(string newName)
        {
            _playerName = newName;
        }

        public void OnNo()
        {
            FirestoreWriter.GoToScoreboard();
        }

        public void OnYes()
        {
            firestoreWriter.SubmitScore(_playerName);
        }

        private void Start()
        {
            score.SetValue(SessionViewModel.Instance.score);
        }

        public override void OnRendered()
        {
        }
    }
}