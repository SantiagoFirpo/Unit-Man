using System;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan
{
    public class JoystickViewModel : ViewModel
    {
        [SerializeField]
        private ReactiveProperty<bool> isMobile;

        public override void OnRendered()
        {
        }

        private void Start()
        {
            isMobile.SetValue(Application.isMobilePlatform);
        }
    }
}
