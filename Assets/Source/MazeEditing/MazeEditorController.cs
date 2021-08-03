using System;
using UnitMan.Source.Config;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.UI
{
    public class MazeEditorController : MonoBehaviour
    {
        private MazeObjectType _selectedObjectType = MazeObjectType.Wall;
        private Maze _currentWorkingMaze;
        private Gameplay _inputMap;
        private Vector2 _mousePosition;

        [SerializeField]
        private Tilemap _wallTilemap;

        [SerializeField]
        private TileBase wallRuleTile;
        
        private Vector3Int _wallOrigin;
        private Camera _mainCamera;
        private bool _isLeftClicking = false;
        private bool _isRightClicking;

        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.performed += OnClickUpdated;
            _inputMap.UI.RightClick.performed += OnRightClickUpdated;

        }

        private void Start()
        {
            _wallOrigin = _wallTilemap.origin;
            _mainCamera = Camera.main;
            // wallRuleTile = _wallTilemap.GetTile(Vector3Int.zero);
        }

        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.performed -= OnClickUpdated;
            _inputMap.UI.RightClick.performed -= OnRightClickUpdated;
        }

        public void OnWallButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Wall;
        }
        
        public void OnPelletButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Pellet;
        }
        
        public void OnBlinkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Blinky;
        }
        
        public void OnPinkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Pinky;
        }
        
        public void OnInkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Inky;
        }
        
        public void OnClydeButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Clyde;
        }
        
        public void OnPowerPelletButtonSelected()
        {
            _selectedObjectType = MazeObjectType.PowerPellet;
        }

        private void OnRightClickUpdated(InputAction.CallbackContext obj)
        {
            _isRightClicking = !_isRightClicking;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            Vector3Int mousePositionOnWallTileset = LevelGridController.Vector2ToVector3Int
                                                                        (_mainCamera.ScreenToWorldPoint
                                                                        (_mousePosition))  - _wallOrigin;
            if (_isRightClicking)
            {
                _wallTilemap.SetTile(mousePositionOnWallTileset, null);
            }

            else if (_isLeftClicking)
                switch (_selectedObjectType)
                {
                    case MazeObjectType.Wall when _wallTilemap.GetTile(mousePositionOnWallTileset) != wallRuleTile:
                        _wallTilemap.SetTile(mousePositionOnWallTileset, wallRuleTile);
                        break;
                    case MazeObjectType.Pellet:
                        //place pellet
                        break;
                    case MazeObjectType.PowerPellet:
                        break;
                    case MazeObjectType.Player:
                        break;
                }
        }

        private void OnClickUpdated(InputAction.CallbackContext context)
        {
            _isLeftClicking = !_isLeftClicking;
        }
    }

    public enum MazeObjectType
    {
        Wall, Pellet, PowerPellet, Player,
        Blinky,
        Pinky,
        Inky,
        Clyde
    }
}
