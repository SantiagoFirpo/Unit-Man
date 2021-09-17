using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;  

namespace UnitMan.Source.UI.Components.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextView : View
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void OnTextChanged(string newText)
        {
            Render(newText);
        }

        private void Render(string newText)
        {
            _text.SetText(newText);
        }
    }
}