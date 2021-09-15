using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextView : View<string>
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        protected override void Render(string state)
        {
            _text.SetText(state);
        }
    }
}