using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnitMan.Source.UI.MVVM
{
    [Serializable]
    public class OneWayBinding<T> : UnityEvent<T>
    {
        [SerializeField]
        private T value;

        public OneWayBinding(T value)
        {
            this.value = value;
        }

        public void SetValue(T newValue)
        {
            value = newValue;
            Invoke(newValue);
        }

        public T GetValue()
        {
            return value;
        }
    }
    [Serializable]
    public class OneWayBinding : UnityEvent
    {
        public void Call()
        {
            Invoke();
        }
    }
}