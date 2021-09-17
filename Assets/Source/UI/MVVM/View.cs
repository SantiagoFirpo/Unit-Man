using System;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM

{
    [Serializable]
    public abstract class View : MonoBehaviour
    {
        protected abstract void Render();
    }
}