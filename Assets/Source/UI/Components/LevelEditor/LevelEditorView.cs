using System;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.UI.Components.LevelEditor
{
    public class LevelEditorView : View
    {
        [Header("Bindings")]

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
        
        [SerializeField]
        private OneWayBinding<string> levelIdBinding;
        
        [SerializeField]
        private OneWayBinding<bool> leftClickBinding;

        [SerializeField]
        private OneWayBinding<bool> pointerOverUIBinding;

        [SerializeField]
        private OneWayBinding<bool> rightClickBinding;

        [SerializeField]
        private OneWayBinding<Vector3Int> mouseTilesetPosition;

        [SerializeField]
        private OneWayBinding<Vector3> mouseWorldPosition;
        
        [Header("Icons")]

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

        [SerializeField]
        private Sprite doorIcon;
        
        [SerializeField]
        private Transform brushPreviewTransform;

        private EventSystem _eventSystem;
        
        [SerializeField]
        private SpriteRenderer brushPreviewSprite;

        [Header("Dependencies")]
        
        [SerializeField]
        private GameObject uiCanvas;
        private Camera _mainCamera;
        private Vector3 _mouseScreenPosition;
        
        [SerializeField]
        private Tilemap wallTilemap;
        
        private Gameplay _inputMap;

        [SerializeField]
        private ToastViewModel toast;

        private bool _wasPointerOverUI;
        private void Start()
        {
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

        public void PingLevelUpload()
        {
            toast.Notify("LEVEL UPLOADED SUCCESSFULLY!");
        }
        
        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mouseScreenPosition = VectorUtil.ToVector3(context.ReadValue<Vector2>());
            mouseTilesetPosition.SetValue(wallTilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition)));

            Vector3 mouseWorldPositionBuffer = VectorUtil.Round(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition));
            mouseWorldPositionBuffer.z = 0f;
            mouseWorldPosition.SetValue(mouseWorldPositionBuffer);
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

        public void PingClipboard()
        {
            toast.Notify("LEVEL ID COPIED TO CLIPBOARD!");
        }
        

        public void OnBrushChanged(BrushType newValue)
        {
            brushPreviewSprite.sprite = BrushTypeToIcon(newValue);
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