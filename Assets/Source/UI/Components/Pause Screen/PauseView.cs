using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.Pause_Screen
{
    public class PauseView : View
    {
        [SerializeField]
        private ReactiveEvent resumeEvent;

        [SerializeField]
        private ReactiveEvent mainMenuEvent;

        public void ResumePressed() => resumeEvent.Call();

        public void MainMenuPressed() => mainMenuEvent.Call();
    }
}
