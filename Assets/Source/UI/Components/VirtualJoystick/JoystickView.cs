using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.VirtualJoystick
{
    public class JoystickView : View
    {
        public void UpdateVisibility(bool isMobile)
        {
            gameObject.SetActive(isMobile);
        }
    }
}
