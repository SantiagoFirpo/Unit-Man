using System;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM

{
    [Serializable]
    public abstract class View : MonoBehaviour
    {
        public GameObject GameObject {get; private set;}

        protected virtual void Awake()
        {
            GameObject = gameObject;
        }

        public void Hide()
        {
            GameObject.SetActive(false);
        }
    }
}