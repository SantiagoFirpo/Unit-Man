using TMPro;

namespace UnitMan.Source.UI
{
    public interface IBindable<in T>
    {
        public void Update(T data);
    }

    public class LivesBinding : IBindable<int>
    {
        private readonly TMP_Text _text;

        public LivesBinding(TMP_Text text)
        {
            _text = text;
        }

        public void Update(int data)
        {
            _text.SetText($"Lives: {data}");
        }
    }

    public class PureBinding : IBindable<object>
    {
        private readonly TMP_Text _text;

        public PureBinding(TMP_Text text)
        {
            _text = text;
        }

        public void Update(object data)
        {
            _text.SetText(data.ToString());
        }
    }
}