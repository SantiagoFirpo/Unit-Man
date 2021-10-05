using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan
{
    public class PauseButtonView : MonoBehaviour
    {
        [SerializeField]
        private ReactiveEvent _pauseEvent = new ReactiveEvent();
        public void PausePressed()
        {
            _pauseEvent.Call();
        }
    }
}
