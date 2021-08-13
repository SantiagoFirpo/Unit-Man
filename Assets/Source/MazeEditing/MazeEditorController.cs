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
        private BrushType _selectedBrush = BrushType.Wall;
        [SerializeField]
        private Level currentWorkingLevel;
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

        private Quaternion _identity;

        private void Awake()
        {
            _inputMap = new Gameplay();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.started += OnLeftClicked;
            _inputMap.UI.Click.canceled += OnLeftUnclicked;
            _inputMap.UI.RightClick.started += OnRightClicked;
            _inputMap.UI.RightClick.canceled += OnRightUnclicked;
            currentWorkingLevel = ScriptableObject.CreateInstance<Level>();
            _brushPreviewSprite = brushPreviewTransform.GetComponent<SpriteRenderer>();
            
            _identity = Quaternion.identity;
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
            _selectedBrush = BrushType.Wall;
            _brushPreviewSprite.sprite = wallIcon;
        }
        
        public void OnPelletButtonSelected()
        {
            _selectedBrush = BrushType.Pellet;
            _brushPreviewSprite.sprite = pelletIcon;
        }
        
        public void OnBlinkyButtonSelected()
        {
            _selectedBrush = BrushType.Blinky;
            _brushPreviewSprite.sprite = blinkyIcon;
        }
        
        public void OnPinkyButtonSelected()
        {
            _selectedBrush = BrushType.Pinky;
            _brushPreviewSprite.sprite = pinkyIcon;
        }
        
        public void OnInkyButtonSelected()
        {
            _selectedBrush = BrushType.Inky;
            _brushPreviewSprite.sprite = inkyIcon;
        }
        
        public void OnClydeButtonSelected()
        {
            _selectedBrush = BrushType.Clyde;
            _brushPreviewSprite.sprite = clydeIcon;
        }
        
        public void OnPowerPelletButtonSelected()
        {
            _selectedBrush = BrushType.PowerPellet;
            _brushPreviewSprite.sprite = powerPelletIcon;
        }
        
        public void OnPacManButtonSelected()
        {
            _selectedBrush = BrushType.PacMan;
            _brushPreviewSprite.sprite = pacManIcon;
        }

        public void OnGhostHouseButtonSelected()
        {
            _selectedBrush = BrushType.GhostHouse;
            _brushPreviewSprite.sprite = houseIcon;
        }
        private void PlaceLevelObjectAndUpdateMaze(BrushType brush, Vector3 position)
        {
            Vector2Int positionV2Int = VectorUtil.VectorToVector2Int(position);
            Vector3Int tilesetPosition = wallTilemap.WorldToCell(position);
            if (currentWorkingLevel.pacManPosition == positionV2Int) return;
            if (currentWorkingLevel.objectPositions.Contains(positionV2Int)) return;
            if (wallTilemap.GetTile(tilesetPosition) == wallRuleTile) return;
            switch (brush)
            {
                case BrushType.Wall:
                {
                    wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
                    currentWorkingLevel.AddLevelObject(LevelObjectType.Wall, positionV2Int);
                    break;
                }
                case BrushType.PacMan:
                {
                    currentWorkingLevel.pacManPosition = positionV2Int;
                    pacManTransform.position = position;
                    break;
                }
                case BrushType.GhostHouse:
                {
                    currentWorkingLevel.ghostHousePosition = positionV2Int;
                    ghostHouseTransform.position = position;
                    break;
                }
                default:
                {
                    if (brush == BrushType.Pellet)
                    {
                        currentWorkingLevel.pelletCount++;
                    }

                    LevelObjectType brushToLevelObjectType = BrushToLevelObjectType(brush);
                    currentWorkingLevel.AddLevelObject(brushToLevelObjectType, positionV2Int);
                    _localObjects.Add(position, CreateGameObject(brushToLevelObjectType, position));
                    break;
                }
            }
            
            
        }

        private static LevelObjectType BrushToLevelObjectType(BrushType brushType)
        {
            return brushType switch
            {
                BrushType.Blinky => LevelObjectType.Blinky,
                BrushType.Pinky => LevelObjectType.Pinky,
                BrushType.Inky => LevelObjectType.Inky,
                BrushType.Clyde => LevelObjectType.Clyde,
                BrushType.Pellet => LevelObjectType.Pellet,
                BrushType.PowerPellet => LevelObjectType.PowerPellet,
                BrushType.Wall => LevelObjectType.Wall,
                BrushType.PacMan => throw new ArgumentException(
                                                        "This PacMan brush cannot be converted to a LevelObject type!"),
                BrushType.GhostHouse => throw new ArgumentException(
                                                            "This GhostHouse brush cannot be converted to a LevelObject type!"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void AddLocalLevelObject(LevelObjectType objectType, Vector3 position)
        {
            if (objectType == LevelObjectType.Wall)
                wallTilemap.SetTile(_mouseTilesetPosition, wallRuleTile);
            else
                _localObjects.Add(position, CreateGameObject(objectType, position));
        }
        
        private GameObject CreateGameObject(LevelObjectType objectType, Vector3 position)
        {
            return objectType == LevelObjectType.Wall ? null :
                                Instantiate(LevelObjectTypeToPrefab(objectType), position, _identity);
        }

        private GameObject LevelObjectTypeToPrefab(LevelObjectType objectType)
        {
            return objectType switch
            {
                LevelObjectType.Blinky => blinkyMarkerPrefab,
                LevelObjectType.Pinky => pinkyMarkerPrefab,
                LevelObjectType.Inky => inkyMarkerPrefab,
                LevelObjectType.Clyde => clydeMarkerPrefab,
                LevelObjectType.Pellet => pelletMarkerPrefab,
                LevelObjectType.PowerPellet => powerMarkerPrefab,
                LevelObjectType.Wall => null,
                _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null)
            };
        }

        public void Save()
        {
            ComputeScatterTargets();
            // string prettyJson = JsonUtility.ToJson(currentWorkingLevel, false);
            // Debug.Log(JsonUtility.ToJson(currentWorkingLevel, true));
            FirestoreListener.SaveStringIntoJson(JsonUtility.ToJson(currentWorkingLevel, false), "currentWorkingMaze");
        }
        
        

        public void Load()
        {
            JsonUtility.FromJsonOverwrite(FirestoreListener.LoadStringFromJson("currentWorkingMaze"), currentWorkingMaze);
            CreateMarkersFromMaze(currentWorkingMaze);
        }

        public void CreateMarkersFromMaze(Maze maze)
        {
            for (int i = 0; i < maze.objectTypes.Count; i++)
            {
                PlaceLevelObject(maze.objectTypes[i], LevelGridController.Vector2IntToVector3(maze.objectPositions[i]));
            }
        }

        private void ComputeScatterTargets()
        {
            BoundsInt cellBounds = wallTilemap.cellBounds;
            currentWorkingLevel.pinkyScatterTarget =
                new Vector2Int(cellBounds.xMin, cellBounds.yMax) + new Vector2Int(-1, 1);
            Vector2Int vectorOne = Vector2Int.one;
            currentWorkingLevel.blinkyScatterTarget = VectorUtil.ToVector2Int(cellBounds.max) + vectorOne;
            currentWorkingLevel.clydeScatterTarget = VectorUtil.ToVector2Int(cellBounds.min) + -vectorOne;
            currentWorkingLevel.inkyScatterTarget = new Vector2Int(cellBounds.xMax, cellBounds.yMin) + new Vector2Int(1, -1);
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
            _mouseWorldPosition = VectorUtil.Round(_mainCamera.ScreenToWorldPoint(_mouseScreenPosition)); 
            _mouseWorldPosition.z = 0f;
            if (_eventSystem.IsPointerOverGameObject()) return;
            brushPreviewTransform.position = _mouseWorldPosition;
        }
        
        private void Update()
        {
            
            if (_eventSystem.IsPointerOverGameObject()) return;
            if (_isLeftClicking)
            {
                PlaceLevelObjectAndUpdateMaze(_selectedBrush, _mouseWorldPosition);
            }
            
            else if (_isRightClicking)
            {
                if (!currentWorkingLevel.objectPositions.Contains(
                    VectorUtil.VectorToVector2Int(_mouseWorldPosition)))
                    return;
                if (_selectedBrush == BrushType.Pellet)
                {
                    currentWorkingLevel.pelletCount--;
                }

                EraseObject(_mouseWorldPosition);


            }
        }

        private void EraseObject(Vector3 position)
        {
            currentWorkingLevel.RemoveLevelObject(VectorUtil.VectorToVector2Int(position));
            bool isWall = wallTilemap.GetTile(VectorUtil.ToVector3Int(position)) != null;
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

    public enum BrushType
    {
        Wall, Pellet, PowerPellet, PacMan,
        Blinky,
        Pinky,
        Inky,
        Clyde,
        GhostHouse
    }
    
    public enum LevelObjectType
    {
        Wall, Pellet, PowerPellet,
        Blinky,
        Pinky,
        Inky,
        Clyde
    }
}
