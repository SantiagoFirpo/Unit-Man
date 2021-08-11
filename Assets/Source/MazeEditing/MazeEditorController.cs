using System;
using System.Collections.Generic;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
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
        [SerializeField]
        private Maze currentWorkingMaze;
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

        [SerializeField]
        private Transform ghostHouseTransform;
        
        private EventSystem _eventSystem;

        [SerializeField] private Transform brushPreviewTransform;
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
        
        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.started += OnLeftClicked;
            _inputMap.UI.Click.canceled += OnLeftUnclicked;
            _inputMap.UI.RightClick.started += OnRightClicked;
            _inputMap.UI.RightClick.canceled += OnRightUnclicked;
            currentWorkingMaze = ScriptableObject.CreateInstance<Maze>();
            _brushPreviewSprite = brushPreviewTransform.GetComponent<SpriteRenderer>();
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
        }

        public void OnWallButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Wall;
            _brushPreviewSprite.sprite = wallIcon;
        }
        
        public void OnPelletButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Pellet;
            _brushPreviewSprite.sprite = pelletIcon;
        }
        
        public void OnBlinkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Blinky;
            _brushPreviewSprite.sprite = blinkyIcon;
        }
        
        public void OnPinkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Pinky;
            _brushPreviewSprite.sprite = pinkyIcon;
        }
        
        public void OnInkyButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Inky;
            _brushPreviewSprite.sprite = inkyIcon;
        }
        
        public void OnClydeButtonSelected()
        {
            _selectedObjectType = MazeObjectType.Clyde;
            _brushPreviewSprite.sprite = clydeIcon;
        }
        
        public void OnPowerPelletButtonSelected()
        {
            _selectedObjectType = MazeObjectType.PowerPellet;
            _brushPreviewSprite.sprite = powerPelletIcon;
        }
        
        public void OnPacManButtonSelected()
        {
            _selectedObjectType = MazeObjectType.PacMan;
            _brushPreviewSprite.sprite = pacManIcon;
        }

        public void OnGhostHouseButtonSelected()
        {
            _selectedObjectType = MazeObjectType.GhostHouse;
            _brushPreviewSprite.sprite = houseIcon;
        }
        private void PlaceLevelObject(MazeObjectType objectType, Vector3 position)
        {
            Vector2Int positionV2Int = LevelGridController.VectorToVector2Int(position);
            if (currentWorkingMaze.playerPosition == LevelGridController.VectorToVector2Int(position)) return;
            if (currentWorkingMaze.levelObjects.ContainsKey(positionV2Int)) return;
            if (wallTilemap.GetTile(_mouseTilesetPosition) == wallRuleTile) return;
            switch (objectType)
            {
                case MazeObjectType.Wall:
                {
                    wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
                    currentWorkingMaze.levelObjects.Add(positionV2Int, objectType);
                    break;
                }
                case MazeObjectType.PacMan:
                {
                    currentWorkingMaze.playerPosition = LevelGridController.VectorToVector2Int(position);
                    pacManTransform.position = position;
                    break;
                }
                case MazeObjectType.GhostHouse:
                {
                    ghostHouseTransform.position = position;
                    break;
                }
                default:
                {
                    if (objectType == MazeObjectType.Pellet)
                    {
                        currentWorkingMaze.pelletCount++;
                    }
                    currentWorkingMaze.levelObjects.Add(positionV2Int, objectType);
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
            
            
        }

        private GameObject InstantiateLevelObject(GameObject prefab, Vector3 position)
        {
            return prefab is null ? null : Instantiate(prefab, position, Quaternion.identity);
        }

        public void Save()
        {
            ComputeScatterTargets();
            currentWorkingMaze.SerializeLevelObjects();
            string prettyJson = JsonUtility.ToJson(currentWorkingMaze, true);
            Debug.Log(prettyJson);
            FirestoreListener.SaveStringIntoJson(prettyJson, "currentWorkingMaze");
        }

        private void ComputeScatterTargets()
        {
            BoundsInt cellBounds = wallTilemap.cellBounds;
            currentWorkingMaze.pinkyScatterTarget =
                new Vector2Int(cellBounds.xMin, cellBounds.yMax) + new Vector2Int(-1, 1);
            Vector2Int vectorOne = Vector2Int.one;
            currentWorkingMaze.blinkyScatterTarget = LevelGridController.Vector3IntToVector2Int(cellBounds.max) + vectorOne;
            currentWorkingMaze.clydeScatterTarget = LevelGridController.Vector3IntToVector2Int(cellBounds.min) + -vectorOne;
            currentWorkingMaze.inkyScatterTarget = new Vector2Int(cellBounds.xMax, cellBounds.yMin) + new Vector2Int(1, -1);
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
            if (_eventSystem.IsPointerOverGameObject()) return;
            brushPreviewTransform.position = _mouseWorldPosition;
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
                if (!currentWorkingMaze.levelObjects.ContainsKey(
                    LevelGridController.VectorToVector2Int(_mouseWorldPosition)))
                    return;
                switch (_selectedObjectType)
                {
                    case MazeObjectType.Wall:
                        wallTilemap.SetTile(_mouseTilesetPosition, null);
                        EraseObject(true, _mouseWorldPosition);

                        break;
                    case MazeObjectType.Pellet:
                        currentWorkingMaze.pelletCount--;
                        EraseObject(false, _mouseWorldPosition);
                        break;
                    default:
                        EraseObject(false, _mouseWorldPosition);
                        break;
                }

            }
        }

        private void EraseObject(bool isWall, Vector3 position)
        {
            currentWorkingMaze.levelObjects.Remove(LevelGridController.VectorToVector2Int(position));
            if (isWall)
            {
                wallTilemap.SetTile(_mouseTilesetPosition, null);
            }
            else
            {
                Destroy(_localObjects[position]);
                _localObjects.Remove(position);
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
        Clyde,
        GhostHouse
    }
}
