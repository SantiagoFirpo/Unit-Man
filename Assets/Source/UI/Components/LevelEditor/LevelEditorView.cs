using System;
using System.Collections.Generic;
using TMPro;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.EventSystems;

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
        
        [SerializeField]
        private GameObject pelletMarkerPrefab;
        
        [SerializeField]
        private GameObject powerMarkerPrefab;
        
        [SerializeField]
        private GameObject blinkyMarkerPrefab;
        
        [SerializeField]
        private GameObject pinkyMarkerPrefab;
        
        [SerializeField]
        private GameObject inkyMarkerPrefab;
        
        [SerializeField]
        private GameObject clydeMarkerPrefab;
        
        private readonly Dictionary<Vector3, GameObject> _localObjects = new Dictionary<Vector3, GameObject>();
        
        [SerializeField]
        private Transform pacManTransform;

        [SerializeField]
        private Transform ghostHouseTransform;
        
        private EventSystem _eventSystem;
        
        private SpriteRenderer _brushPreviewSprite;
        [SerializeField]
        private Sprite wallIcon;
        [SerializeField]
        private Sprite pelletIcon;
        [SerializeField]
        private Sprite powerPelletIcon;
        [SerializeField]
        private Sprite blinkyIcon;
        [SerializeField]
        private Sprite pinkyIcon;
        [SerializeField]
        private Sprite inkyIcon;
        [SerializeField]
        private Sprite clydeIcon;
        [SerializeField]
        private Sprite pacManIcon;
        [SerializeField]
        private Sprite houseIcon;

        private Quaternion _identity;
        [SerializeField]
        private Transform ghostDoor;

        [SerializeField]
        private Sprite doorIcon;

        [SerializeField]
        private GameObject clipboardPingText;

        private bool _isUIActive = true;

        private Timer _clipboardPingTimer;
        private Timer _uploadPingTimer;

        
        [SerializeField]
        private GameObject uiCanvas;

        [SerializeField]
        private TMP_InputField levelIdField;

        [SerializeField]
        private GameObject uploadPingText;
        
        [SerializeField]
        private OneWayBinding uploadBinding;

        [SerializeField]
        private OneWayBinding saveBinding;

        [SerializeField]
        private OneWayBinding loadBinding;

        [SerializeField]
        private OneWayBinding mainMenuBinding;

        [SerializeField]
        private OneWayBinding copyIdBinding;

        [SerializeField]
        private OneWayBinding toggleHudBinding;

        [SerializeField]
        private OneWayBinding<BrushType> brushTypeBinding;

        public void OnLevelIdChanged(string newLevelId)
        {
            levelIdBinding.SetValue(newLevelId);
        }

        private void Start()
        {
            brushPreviewSprite = brushPreviewTransform.GetComponent<SpriteRenderer>();
        }

        public void OnUploadPressed()
        {
            uploadBinding.Call();
        }

        public void OnSavePressed()
        {
            saveBinding.Call();
        }

        public void OnLoadPressed()
        {
            loadBinding.Call();
        }

        public void OnMainMenuPressed()
        {
            mainMenuBinding.Call();
        }

        public void OnCopyIdPressed()
        {
            copyIdBinding.Call();
        }

        public void OnToggleHudPressed()
        {
            toggleHudBinding.Call();
        }

        public void OnWallPressed()
        {
            brushTypeBinding.SetValue(BrushType.Wall);
        }

        public void OnPelletPressed()
        {
            brushTypeBinding.SetValue(BrushType.Pellet);
        }

        public void OnPowerPelletPressed()
        {
            brushTypeBinding.SetValue(BrushType.PowerPellet);
        }

        public void OnBlinkyPressed()
        {
            brushTypeBinding.SetValue(BrushType.Blinky);
        }

        public void OnPinkyPressed()
        {
            brushTypeBinding.SetValue(BrushType.Pinky);
        }

        public void OnInkyPressed()
        {
            brushTypeBinding.SetValue(BrushType.Inky);
        }

        public void OnClydePressed()
        {
            brushTypeBinding.SetValue(BrushType.Clyde);
        }

        public void OnPacManPressed()
        {
            brushTypeBinding.SetValue(BrushType.PacMan);
        }

        public void OnGhostHousePressed()
        {
            brushTypeBinding.SetValue(BrushType.GhostHouse);
        }

        public void OnGhostDoorPressed()
        {
            brushTypeBinding.SetValue(BrushType.GhostDoor);
        }
    }
}