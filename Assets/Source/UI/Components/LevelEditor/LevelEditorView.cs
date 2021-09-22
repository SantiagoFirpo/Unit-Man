using System;
using System.Collections.Generic;
using TMPro;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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

        private Camera _mainCamera;
        private Vector3 _mouseScreenPosition;
        [SerializeField]
        private Tilemap wallTilemap;
        
        private Gameplay _inputMap;
        [SerializeField]
        private OneWayBinding<bool> leftClickBinding;

        private bool _wasPointerOverUI;
        [SerializeField]
        private OneWayBinding<bool> pointerOverUIBinding;

        [SerializeField]
        private OneWayBinding<bool> rightClickBinding;

        [SerializeField]
        private OneWayBinding<Vector3Int> _mouseTilesetPosition;

        [SerializeField]
        private OneWayBinding<Vector3> _mouseWorldPosition;

        private void Start()
        {
            _clipboardPingTimer = new Timer(3f, false, true);
            _clipboardPingTimer.OnEnd += ClipboardPingTimerOnOnEnd;

            _uploadPingTimer = new Timer(3f, false, true);
            _uploadPingTimer.OnEnd += UploadPingTimerOnOnEnd;
            
            _mainCamera = Camera.main;
            _eventSystem = EventSystem.current;
            brushPreviewSprite = brushPreviewTransform.GetComponent<SpriteRenderer>();
        }
        
        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.started -= OnLeftClicked;
            _inputMap.UI.Click.canceled -= OnLeftUnclicked;
            _inputMap.UI.RightClick.started -= OnRightClicked;
            _inputMap.UI.RightClick.canceled -= OnRightUnclicked;
        }
        
        private void OnLeftClicked(InputAction.CallbackContext context)
        {
            leftClickBinding.SetValue(true);
        }
        
        private void OnLeftUnclicked(InputAction.CallbackContext context)
        {
            leftClickBinding.SetValue(false);
        }
        
        private void OnRightClicked(InputAction.CallbackContext context)
        {
            rightClickBinding.SetValue(true);
        }
        
        private void OnRightUnclicked(InputAction.CallbackContext context)
        {
            rightClickBinding.SetValue(false);
        }

        private void UploadPingTimerOnOnEnd()
        {
            uploadPingText.SetActive(false);
        }

        private void ClipboardPingTimerOnOnEnd()
        {
            clipboardPingText.SetActive(false);
        }

        public void OnLevelIdChanged(string newLevelId)
        {
            levelIdBinding.SetValue(newLevelId);
        }
        
        private Sprite BrushTypeToIcon(BrushType brushType)
        {
            return brushType switch
            {
                BrushType.Wall => wallIcon,
                BrushType.Pellet => pelletIcon,
                BrushType.PowerPellet => powerPelletIcon,
                BrushType.PacMan => pacManIcon,
                BrushType.Blinky => blinkyIcon,
                BrushType.Pinky => pinkyIcon,
                BrushType.Inky => inkyIcon,
                BrushType.Clyde => clydeIcon,
                BrushType.GhostHouse => houseIcon,
                BrushType.GhostDoor => doorIcon,
                _ => throw new ArgumentOutOfRangeException(nameof(brushType), brushType, null)
            };
        }
        
        private void PingUserClipboard()
        {
            clipboardPingText.SetActive(true);
            _clipboardPingTimer.Start();
        }

        private void PingLevelUpload()
        {
            uploadPingText.SetActive(true);
            _uploadPingTimer.Start();
        }
        
        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mouseScreenPosition = VectorUtil.ToVector3(context.ReadValue<Vector2>());
            _mouseTilesetPosition.SetValue(wallTilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition)));

            Vector3 mouseWorldPositionBuffer = VectorUtil.Round(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition));
            mouseWorldPositionBuffer.z = 0f;
            _mouseWorldPosition.SetValue(mouseWorldPositionBuffer);
            if (_eventSystem.IsPointerOverGameObject()) return;
            brushPreviewTransform.position = mouseWorldPositionBuffer;
        }

        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.started += OnLeftClicked;
            _inputMap.UI.Click.canceled += OnLeftUnclicked;
            _inputMap.UI.RightClick.started += OnRightClicked;
            _inputMap.UI.RightClick.canceled += OnRightUnclicked;
        }

        private void Update()
        {
            bool isPointerOverGameObject = _eventSystem.IsPointerOverGameObject();
            if (isPointerOverGameObject != _wasPointerOverUI)
            {
                pointerOverUIBinding.SetValue(isPointerOverGameObject);
            }

            _wasPointerOverUI = isPointerOverGameObject;

        }

        public void OnBrushChanged(BrushType newValue)
        {
            _brushPreviewSprite.sprite = BrushTypeToIcon(newValue);
        }

        public void ToggleUI(bool newValue)
        {
            uiCanvas.SetActive(newValue);
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