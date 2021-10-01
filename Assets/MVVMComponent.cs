using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan
{
    public class MVVMComponent : MonoBehaviour
    {
        [HideInInspector]
        public ViewModel viewModel;
        
        [HideInInspector]
        public View view;

        public void Render()
        {
            if (view is null) return;
            view.GameObject.SetActive(true);
            if (viewModel is null) return;
            viewModel.OnRendered();
        }
    }
}
