using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.Text
{
    // [Serializable]
    public class TextViewModel : ViewModel<string>
    {
        protected override void InitializeState()
        {
            OverwriteState("");
        }
    }
}