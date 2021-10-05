using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.ScorePrompt
{
    public class ScorePromptView : View

    {
        [SerializeField]
        private TMP_Text scoreLabel;

        [SerializeField]
        private ReactiveProperty<string> nameBinding;

        [SerializeField]
        private Event yesButtonBinding = new Event();

        [SerializeField]
        private Event noButtonBinding = new Event();

        public void OnScoreChanged(int newScore)
        {
            scoreLabel.SetText($"SCORE: {newScore}");
        }

        public void OnNameChanged(string newName)
        {
            nameBinding.SetValue(newName);
        }

        public void OnYesPressed()
        {
            yesButtonBinding.Call();
        }

        public void OnNoPressed()
        {
            noButtonBinding.Call();
        }
    }
}