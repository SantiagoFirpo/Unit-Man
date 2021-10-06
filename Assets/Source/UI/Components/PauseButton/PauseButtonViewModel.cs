using UnitMan.Source.Management.Audio;
using UnitMan.Source.Management.Session;
using UnitMan.Source.UI.MVVM;

namespace UnitMan
{
    public class PauseButtonViewModel : ViewModel
    {
        public void Pause()
        {
            SessionManagerSingle.Instance.ToggleUserPause();
        }

        public override void OnRendered()
        {
            
        }
    }
}
