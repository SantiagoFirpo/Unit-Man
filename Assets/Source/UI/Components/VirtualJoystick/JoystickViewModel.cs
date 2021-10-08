using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.VirtualJoystick
{
    public class JoystickViewModel : ViewModel
    {
        [SerializeField]
        private Reactive<bool> isMobile;

        public override void OnRendered()
        {
        }

        private void Start()
        {
            isMobile.SetValue(Application.isMobilePlatform);
        }
    }
}
