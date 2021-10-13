using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class ViewModel : MonoBehaviour
    {
        public abstract void OnRendered();

        [SerializeField]
        protected ReactiveEvent renderEvent;
    }
}