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
        private Vector2Int _mouseWorldPositionV2Int;
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

        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.started += OnLeftClicked;
            _inputMap.UI.Click.canceled += OnLeftUnclicked;
            _inputMap.UI.RightClick.started += OnRightClicked;
            _inputMap.UI.RightClick.canceled += OnRightUnclicked;
            _currentWorkingMaze = new Maze();

        }

        private void Start()
        {
            _wallOrigin = wallTilemap.origin;
            _mainCamera = Camera.main;
        }

        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.started -= OnLeftClicked;
            _inputMap.UI.Click.canceled -= OnLeftUnclicked;
            _inputMap.UI.RightClick.started -= OnRightClicked;
            _inputMap.UI.RightClick.canceled -= OnRightUnclicked;
            _currentWorkingMaze = new Maze();
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
        //
        // private void OnLeftClick(InputAction.CallbackContext context)
        // {
        //     if (!context.started) return;
        //     Debug.Log("Should place");
        //     Vector2Int mouseWorldPosition = LevelGridController.VectorToVector2Int(_mouseWorldPosition);
        //     if (_currentWorkingMaze.levelObjects.ContainsKey(mouseWorldPosition)) return;
        //     switch (_selectedObjectType)
        //     {
        //         case MazeObjectType.Wall:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Wall);
        //             break;
        //         case MazeObjectType.Pellet:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Pellet);
        //             break;
        //         case MazeObjectType.PowerPellet:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.PowerPellet);
        //             break;
        //         case MazeObjectType.Player:
        //             _currentWorkingMaze.playerPosition = mouseWorldPosition;
        //             break;
        //         case MazeObjectType.Blinky:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Blinky);
        //
        //             break;
        //         case MazeObjectType.Pinky:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Pinky);
        //
        //             break;
        //         case MazeObjectType.Inky:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Inky);
        //
        //             break;
        //         case MazeObjectType.Clyde:
        //             _currentWorkingMaze.levelObjects.Add(mouseWorldPosition, MazeObjectType.Clyde);
        //
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        // }

        private void PlaceLevelObject(GameObject prefab)
        {
            if (prefab is null) return;
            Instantiate(prefab, _mouseWorldPosition, Quaternion.identity);
        }

        private void OnRightClicked(InputAction.CallbackContext context)
        {
            _isRightClicking = true;
        }
        
        private void OnRightUnclicked(InputAction.CallbackContext context)
        {
            _isRightClicking = false;
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
                _mouseWorldPositionV2Int = LevelGridController.VectorToVector2Int(_mouseWorldPosition);
                _currentWorkingMaze.levelObjects.Remove(_mouseWorldPositionV2Int);
                wallTilemap.SetTile(_mouseTilesetPosition, null);
            }

            else if (_isLeftClicking && !_currentWorkingMaze.levelObjects.ContainsKey(_mouseWorldPositionV2Int))
                switch (_selectedObjectType)
                {
                    case MazeObjectType.Wall when wallTilemap.GetTile(_mouseTilesetPosition) != wallRuleTile:
                    {
                        wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
                        break;
                    }
                    case MazeObjectType.Pellet:
                    {
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.Pellet);
                        PlaceLevelObject(pelletMarkerPrefab);
                        break;
                    }
                    case MazeObjectType.PowerPellet:
                    {
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.PowerPellet);
                        PlaceLevelObject(powerMarkerPrefab);
                        break;
                    }
                    case MazeObjectType.Player:
                    {
                        _currentWorkingMaze.playerPosition = _mouseWorldPositionV2Int;
                        // PlaceLevelObject(playerMare);
                        break;
                    }
                    case MazeObjectType.Blinky:
                    {
                        PlaceLevelObject(blinkyMarkerPrefab);
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.Blinky);
                    
                        break;
                    }
                    case MazeObjectType.Pinky:
                    {
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.Pinky);
                        PlaceLevelObject(pinkyMarkerPrefab);
                        break;
                    }
                    case MazeObjectType.Inky:
                    {
                        PlaceLevelObject(inkyMarkerPrefab);
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.Inky);
                    
                        break;
                    }
                    case MazeObjectType.Clyde:
                    {
                        PlaceLevelObject(clydeMarkerPrefab);
                        _currentWorkingMaze.levelObjects.Add(_mouseWorldPositionV2Int, MazeObjectType.Clyde);
                        break;
                    }
                }
        }

        private void OnLeftClicked(InputAction.CallbackContext context)
        {
            _isLeftClicking = true;
        }
        
        private void OnLeftUnclicked(InputAction.CallbackContext context)
        {
            _isLeftClicking = false;
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
