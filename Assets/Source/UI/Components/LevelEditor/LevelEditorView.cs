using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LevelEditor
{
    public class LevelEditorView : View
    {
        [SerializeField]
        private OneWayBinding<string> levelIdBinding;

        [SerializeField]
        private Transform brushPreviewTransform;

        [SerializeField]
        private SpriteRenderer brushPreviewSprite;

        public void OnLevelIdChanged(string newLevelId)
        {
            levelIdBinding.SetValue(newLevelId);
        }

        public void OnUploadPressed()
        {
            
        }

        public void OnSavePressed()
        {
            
        }

        public void OnLoadPressed()
        {
            
        }

        public void OnMainMenuPressed()
        {
            
        }

        public void OnCopyIdPressed()
        {
            
        }

        public void OnToggleHudPressed()
        {
            
        }

        public void OnWallPressed()
        {
            
        }

        public void OnPelletPressed()
        {
            
        }

        public void OnPowerPelletPressed()
        {
            
        }

        public void OnBlinkyPressed()
        {
            
        }

        public void OnPinkyPressed()
        {
            
        }

        public void OnInkyPressed()
        {
            
        }

        public void OnClydePressed()
        {
            
        }

        public void OnPacManPressed()
        {
            
        }

        public void OnGhostHousePressed()
        {
            
        }

        public void OnGhostDoorPressed()
        {
            
        }
    }
}