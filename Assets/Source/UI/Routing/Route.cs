using System;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Routing.Routers
{
    [Serializable]
    public class Route<TEnumType> where TEnumType : struct, Enum
    {
        [SerializeField]
        private GameObject view;

        // [SerializeField]
        // private MVVMComponent component;

        [SerializeField]
        private ViewModel viewModel;
        
        [SerializeField]
        private TEnumType value;


        public TEnumType GetValue() => value;
        public Route(TEnumType value)
        {
            this.value = value;
        }

        public void Render()
        {
            Debug.Log($"Rendering route {value}");
            view.SetActive(true);
            if (viewModel is null) return;
            viewModel.OnRendered();
            // if (viewModel == null) return;
            // viewModel.OnRendered();
        }

        public void Hide()
        {
            view.SetActive(false);
        }
    }
}