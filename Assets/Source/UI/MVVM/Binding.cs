using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnitMan.Source.UI.MVVM
{
    [Serializable]
    public class Binding<T> : UnityEvent<T>
    {
        [SerializeField]
        private T value;

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
    public class Binding : UnityEvent
    {
        public void Call()
        {
            Invoke();
        }
    }
}