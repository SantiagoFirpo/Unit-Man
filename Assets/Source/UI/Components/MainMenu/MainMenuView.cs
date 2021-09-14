using TMPro;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class MainMenuView : View<MainMenuState, MainMenuViewModel>
    {
        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private TMP_InputField passwordField;
        public void OnLoginButtonPressed()
        {
            
        }

        public void OnRegisterButtonPressed()
        {
            
        }

        public void OnSignOutButtonPressed()
        {
            
        }
        protected override void Render(MainMenuState state)
        {
            // throw new System.NotImplementedException();
        }
    }
}