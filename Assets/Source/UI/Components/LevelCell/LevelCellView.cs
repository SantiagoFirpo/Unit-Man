using UnitMan.Source.UI.MVVM;
using UnityEngine;
using Event = UnitMan.Source.UI.MVVM.Event;

namespace UnitMan.Source.UI.Components.LevelCell
{
    public class LevelCellView : View
    {
        [SerializeField]
        private ReactiveProperty<string> levelIdBinding;

        [SerializeField]
        private ReactiveProperty<string> authorNameBinding;

        [SerializeField]
        private ReactiveProperty<string> levelNameBinding;

        [SerializeField]
        private Event playBinding;

        [SerializeField]
        private Event editBinding;
        
        [SerializeField]
        private Event uploadBinding;

        [SerializeField]
        private Event leaderboardBinding;

        [SerializeField]
        private Event deleteBinding;


        public void OnLevelIdChanged(string newValue) => levelIdBinding.SetValue($"LEVEL ID: {newValue}");

        public void OnAuthorNameChanged(string newValue) => authorNameBinding.SetValue($"BY {newValue}");

        public void OnLevelNameChanged(string newValue) => levelNameBinding.SetValue(newValue);

        public void OnPlayButtonPressed() => playBinding.Call();
        
        public void OnEditButtonBinding() => editBinding.Call();
        
        public void OnUploadButtonPressed() => uploadBinding.Call();

        public void LeaderboardPressed() => leaderboardBinding.Call();

        public void DeletePressed() => deleteBinding.Call();
    }
}
