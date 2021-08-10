using System;
using System.Collections.Generic;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private Vector3 _mouseWorldPosition;

        [FormerlySerializedAs("_wallTilemap")] [SerializeField]
        private Tilemap wallTilemap;

        [SerializeField]
        private TileBase wallRuleTile;
        
        private Camera _mainCamera;
        private bool _isLeftClicking;
        private bool _isRightClicking;
        
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

        private Vector3Int _mouseTilesetPosition;

        private readonly Dictionary<Vector3, GameObject> _localObjects = new Dictionary<Vector3, GameObject>();
        
        [SerializeField]
        private Transform pacManTransform;

        private EventSystem _eventSystem;

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
            _mainCamera = Camera.main;
            _eventSystem = EventSystem.current;
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
        
        public void OnPacManButtonSelected()
        {
            _selectedObjectType = MazeObjectType.PacMan;
        }

        private void PlaceLevelObject(MazeObjectType objectType, Vector3 position)
        {
            Vector2Int positionV2Int = LevelGridController.VectorToVector2Int(position);
            if (_currentWorkingMaze.playerPosition == LevelGridController.VectorToVector2Int(position)) return;
            if (_currentWorkingMaze.levelObjects.ContainsKey(positionV2Int)) return;
            if (wallTilemap.GetTile(_mouseTilesetPosition) == wallRuleTile) return;
            switch (objectType)
            {
                case MazeObjectType.Wall:
                    wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
                    _currentWorkingMaze.levelObjects.Add(positionV2Int, objectType);
                    break;
                case MazeObjectType.PacMan:
                    _currentWorkingMaze.playerPosition = LevelGridController.VectorToVector2Int(position);
                    pacManTransform.position = position;
                    break;
                default:
                    _currentWorkingMaze.levelObjects.Add(positionV2Int, objectType);
                    _localObjects.Add(position, InstantiateLevelObject(objectType switch
                    {
                        MazeObjectType.Blinky => blinkyMarkerPrefab,
                        MazeObjectType.Pinky => pinkyMarkerPrefab,
                        MazeObjectType.Inky => inkyMarkerPrefab,
                        MazeObjectType.Clyde => clydeMarkerPrefab,
                        MazeObjectType.Pellet => pelletMarkerPrefab,
                        MazeObjectType.PowerPellet => powerMarkerPrefab,
                        _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null)
                    }, position));
                    break;
            }
            
            
        }

        private GameObject InstantiateLevelObject(GameObject prefab, Vector3 position)
        {
            return prefab is null ? null : Instantiate(prefab, position, Quaternion.identity);
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
            _mouseTilesetPosition = wallTilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition));
            _mouseWorldPosition = LevelGridController.Round(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition)); 
            _mouseWorldPosition.z = 0f;
        }
        
        private void Update()
        {
            
            if (_eventSystem.IsPointerOverGameObject()) return;
            if (_isLeftClicking)
            {
                PlaceLevelObject(_selectedObjectType, _mouseWorldPosition);
            }
            
            else if (_isRightClicking)
            {
                if (!_currentWorkingMaze.levelObjects.ContainsKey(
                    LevelGridController.VectorToVector2Int(_mouseWorldPosition)))
                    return;
                if (_selectedObjectType == MazeObjectType.Wall)
                {
                    wallTilemap.SetTile(_mouseTilesetPosition, null);
                }
                else
                {
                    Destroy(_localObjects[_mouseWorldPosition]);
                    _localObjects.Remove(_mouseWorldPosition);
                    
                }

                _currentWorkingMaze.levelObjects.Remove(LevelGridController.VectorToVector2Int(_mouseWorldPosition));
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
        Wall, Pellet, PowerPellet, PacMan,
        Blinky,
        Pinky,
        Inky,
        Clyde
    }
}
