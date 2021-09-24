using System;
using UnityEngine;

namespace UnitMan.Source.UI.Routers
{
    [Serializable]
    public class Route<TEnumType>
    {
        [SerializeField]
        private GameObject view;
        [SerializeField]
        private TEnumType value;


        public TEnumType GetValue() => value;
        public Route(TEnumType value)
        {
            this.value = value;
        }

        public void Render()
        {
            view.SetActive(true);
        }

        public void Hide()
        {
            view.SetActive(false);
        }
    }
}