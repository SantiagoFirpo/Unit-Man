using UnityEngine;

namespace UnitMan.Source.UI.Components.MainMenu
{
    public class AuthFormData
    {
        public string email;
        [HideInInspector]
        public string password;

        public AuthFormData(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}