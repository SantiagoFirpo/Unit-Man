using System;
using UnityEngine.Events;

namespace UnitMan.Source.UI.MVVM
{
    [Serializable]
    public class ReactiveEvent : UnityEvent
    {
        public void Call()
        {
            Invoke();
        }
    }
}