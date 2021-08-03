using System;
using UnitMan.Source.UI;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.MazeEditing
{
    public class MazeEditorController : MonoBehaviour
    {
        private MazeObjectType _selectedObjectType = MazeObjectType.Wall;
        private Maze _currentWorkingMaze;
        private Gameplay _inputMap;
        private Vector2 _mouseScreenPosition;
        private Vector3Int _mouseTilesetPosition;
        private Vector3 _mouseWorldPosition;

        [FormerlySerializedAs("_wallTilemap")] [SerializeField]
        private Tilemap wallTilemap;

        [SerializeField]
        private TileBase wallRuleTile;
        
        private Vector3Int _wallOrigin;
        private Camera _mainCamera;
        private bool _isLeftClicking;
        private bool _isRightClicking;
        
        [SerializeField]
        private GameObject pelletPrefab;
        
        [SerializeField]
        private GameObject powerPrefab;
        
        [SerializeField]
        private GameObject blinkyPrefab;
        
        [SerializeField]
        private GameObject pinkyPrefab;
        
        [SerializeField]
        private GameObject inkyPrefab;
        
        [SerializeField]
        private GameObject clydePrefab;

        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.performed += OnClickUpdated;
            _inputMap.UI.RightClick.performed += OnRightClickUpdated;
            _inputMap.UI.Click.performed += OnLeftClick;

            _currentWorkingMaze = new Maze();

        }

        private void Start()
        {
            _wallOrigin = wallTilemap.origin;
            _mainCamera = Camera.main;
            // wallRuleTile = _wallTilemap.GetTile(Vector3Int.zero);
        }

        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.performed -= OnClickUpdated;
            _inputMap.UI.RightClick.performed -= OnRightClickUpdated;
            _inputMap.UI.Click.started -= OnLeftClick;
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

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            Debug.Log("Should place");
            switch (_selectedObjectType)
            {
                case MazeObjectType.Wall:
                    break;
                case MazeObjectType.Pellet:
                    PlaceLevelObject(pelletPrefab);
                    break;
                case MazeObjectType.PowerPellet:
                    PlaceLevelObject(powerPrefab);
                    break;
                case MazeObjectType.Player:
                    break;
                case MazeObjectType.Blinky:
                    break;
                case MazeObjectType.Pinky:
                    break;
                case MazeObjectType.Inky:
                    break;
                case MazeObjectType.Clyde:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlaceLevelObject(GameObject prefab)
        {
            Instantiate(prefab, _mouseWorldPosition, Quaternion.identity);
        }

        private void OnRightClickUpdated(InputAction.CallbackContext obj)
        {
            _isRightClicking = !_isRightClicking;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mouseScreenPosition = context.ReadValue<Vector2>();
            _mouseTilesetPosition = LevelGridController.Vector2ToVector3Int
            (_mainCamera.ScreenToWorldPoint
                (_mouseScreenPosition))  - _wallOrigin;
            _mouseWorldPosition = (wallTilemap.CellToWorld(_mouseTilesetPosition));
        }

        private void Update()
        {
            
            if (_isRightClicking)
            {
                wallTilemap.SetTile(_mouseTilesetPosition, null);
            }

            else if (_isLeftClicking)
                switch (_selectedObjectType)
                {
                    case MazeObjectType.Wall when wallTilemap.GetTile(_mouseTilesetPosition) != wallRuleTile:
                        wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
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
