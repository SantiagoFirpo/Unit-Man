using UnitMan.Source.UI.MVVM;

namespace UnitMan
{
    public class JoystickView : View
    {
        public void UpdateVisibility(bool isMobile)
        {
            gameObject.SetActive(isMobile);
        }
    }
}
