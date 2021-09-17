using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnitMan.Source.UI.MVVM
{
    [Serializable]
    public class Binding<T> : UnityEvent<T>
    {
        [SerializeField]
        private T _value;

        public void SetValue(T value)
        {
            _value = value;
            this.Invoke(value);
        }

        public T GetValue()
        {
            return _value;
        }
    }
    [Serializable]
    public class Binding : UnityEvent
    {
        public void Call()
        {
            this.Invoke();
        }
    }
}