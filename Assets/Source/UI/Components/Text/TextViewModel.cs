using UnitMan.Source.UI.MVVM;

namespace UnitMan.Source.UI.Components.Text
{
    // [Serializable]
    public class TextViewModel : ViewModel
    {
        public OneWayBinding<string> textBinding;

        public void Set(string value)
        {
            textBinding.SetValue(value);
        }
    }
}