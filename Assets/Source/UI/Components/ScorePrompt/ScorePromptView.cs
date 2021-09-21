using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.ScorePrompt
{
    public class ScorePromptView : View

    {
        [SerializeField]
        private TMP_Text scoreLabel;

        [SerializeField]
        private OneWayBinding<string> nameBinding = new OneWayBinding<string>();

        [SerializeField]
        private OneWayBinding yesButtonBinding = new OneWayBinding();

        [SerializeField]
        private OneWayBinding noButtonBinding = new OneWayBinding();

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