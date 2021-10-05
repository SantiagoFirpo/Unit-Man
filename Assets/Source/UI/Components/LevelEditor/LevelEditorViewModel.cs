using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.Management.Firebase.FirestoreLeaderboard;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.UI.Components.LevelEditor
{
    public class LevelEditorViewModel : ViewModel
    {
        public const string DEFAULT_LEVEL_NAME = "New Level";


        [SerializeField]
        private ReactiveProperty<BrushType> selectedBrushBinding;
        [SerializeField]
        public Level currentWorkingLevel;

        private Vector3 _mouseWorldPosition;


        [FormerlySerializedAs("_wallTilemap")] [SerializeField]
        public Tilemap wallTilemap;

        [SerializeField]
        public TileBase wallRuleTile;
        
        // private Camera _mainCamera;
        // private bool _isLeftClicking;
        // private bool _isRightClicking;
        
        [SerializeField]
        public GameObject pelletMarkerPrefab;
        
        [SerializeField]
        public GameObject powerMarkerPrefab;
        
        [SerializeField]
        public GameObject blinkyMarkerPrefab;
        
        [SerializeField]
        public GameObject pinkyMarkerPrefab;
        
        [SerializeField]
        public GameObject inkyMarkerPrefab;
        
        [SerializeField]
        public GameObject clydeMarkerPrefab;

        public Vector3Int mouseTilesetPosition;

        public readonly Dictionary<Vector3, GameObject> localObjects = new Dictionary<Vector3, GameObject>();
        
        [SerializeField]
        public Transform pacManTransform;

        [SerializeField]
        public Transform ghostHouseTransform;

        public Quaternion identity;
        [SerializeField]
        public Transform ghostDoor;

        [SerializeField]
        private ReactiveProperty<bool> isUIActiveBinding;

        [SerializeField]
        private ReactiveEvent clipboardPingBinding;

        private string _levelId;
        [SerializeField]
        private ReactiveEvent levelUploadPing;

        private bool _isRightClicking;
        private bool _isLeftClicking;
        private bool _isPointerOverGameObject;
        private readonly LevelEditManager _levelEditManager;

        public LevelEditorViewModel()
        {
            _levelEditManager = new LevelEditManager(this);
        }
        
        private void Awake()
        {
            Level instanceLevel = CrossSceneLevelContainer.Instance.level;
            if (instanceLevel != null)
            {
                currentWorkingLevel = instanceLevel;
                Debug.Log("Loading Level...");
                LoadCurrentLevelIntoEditor();
                
            }
            else
            {
                Debug.Log("creating new level...");
                currentWorkingLevel = new Level(DEFAULT_LEVEL_NAME,
                    FirebaseAuthManager.GetDisplayName(),
                    FirebaseAuthManager.Instance.auth.CurrentUser.UserId);
            }
            identity = Quaternion.identity;
        }

        public void OnToggleUI()
        {
            isUIActiveBinding.SetValue(!isUIActiveBinding.GetValue());
        }

        public void OnLevelIdChanged(string newValue)
        {
            _levelId = newValue;
        }
        
        public void OnBrushSelected(BrushType brushType)
        {
            selectedBrushBinding.SetValue(brushType);
        }

        public void OnLevelSaved(string levelName)
        {
            currentWorkingLevel.name = levelName;
            OnSaveButtonPressed();
        }

        public void OnMouseTilesetPositionChanged(Vector3Int newValue)
        {
            mouseTilesetPosition = newValue;
        }

        public bool IsPositionValid(Vector2Int positionV2Int)
        {
            Vector3Int tilesetPosition = wallTilemap.WorldToCell(VectorUtil.ToVector3(positionV2Int));
            return currentWorkingLevel.pacManPosition != positionV2Int &&
                    currentWorkingLevel.ghostDoorPosition != positionV2Int &&
                    currentWorkingLevel.ghostHousePosition != positionV2Int &&
                    !currentWorkingLevel.objectPositions.Contains(positionV2Int) &&
                    wallTilemap.GetTile(tilesetPosition) != wallRuleTile;
        }

        public static LevelObjectType BrushToLevelObjectType(BrushType brushType)
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

        public void OnSaveButtonPressed()
        {
            Debug.Log("Save button pressed!");
            if (currentWorkingLevel.name == DEFAULT_LEVEL_NAME)
            {
                LevelEditorRouter.Instance.SetState(LevelEditorRoute.SaveAs);
            }
            else
            {
                if (currentWorkingLevel.id == "")
                {
                    currentWorkingLevel.id = GetUniqueId();
                }
                SaveLevelToDisk();
            }

        }

        private void SaveLevelToDisk()
        {
            _levelEditManager.ComputeScatterTargets();
            CopyIdToClipboard();
            string json = JsonUtility.ToJson(currentWorkingLevel, false);
            Debug.Log(json);
            FirestoreListener.SaveStringIntoJson(json, currentWorkingLevel.id);
        }

        public void CopyIdToClipboard()
        {
            if (currentWorkingLevel.id is null) return;
            CopyStringToClipboard(currentWorkingLevel.id);
            clipboardPingBinding.Call();
        }

        private static void CopyStringToClipboard(string hash)
        {
            GUIUtility.systemCopyBuffer = hash;
        }

        public static string GetUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public void OnMainMenuSelected()
        {
            SceneManager.LoadScene("Main Menu");
        }


        public void Load()
        {
            try
            {
                JsonUtility.FromJsonOverwrite(FirestoreListener.LoadStringFromJson(_levelId), currentWorkingLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            LoadCurrentLevelIntoEditor();
        }

        private void LoadCurrentLevelIntoEditor()
        {
            _levelEditManager.PopulateEditorFromLevelObject(currentWorkingLevel);
        }

        public void Upload()
        {
            UploadLevelToFirestore(currentWorkingLevel);
            levelUploadPing.Call();
        }

        public static void UploadLevelToFirestore(Level level)
        {
            if (level.id == null) return;
            string path = $"levels/{level.id}";
            FirestoreLevel firestoreLevel = FirestoreLevel.FromLevel(level);
            Debug.Log("Converted level to FirestoreLevel");
            FirebaseFirestore.DefaultInstance.Document(path).SetAsync(firestoreLevel);
            Debug.Log($"Saved level into {path}");
        }

        public void OnMouseWorldPositionChanged(Vector3 newValue)
        {
            _mouseWorldPosition = newValue;
        }

        public void OnPointerOverGameObjectChanged(bool newValue)
        {
            _isPointerOverGameObject = newValue;
        }

        private void Update()
        {
            
            if (_isPointerOverGameObject) return;
            if (_isLeftClicking)
            {
                _levelEditManager.PlaceLevelObjectAndUpdateMaze(selectedBrushBinding.GetValue(), _mouseWorldPosition);
            }
            
            else if (_isRightClicking)
            {
                if (!currentWorkingLevel.objectPositions.Contains(
                    VectorUtil.ToVector2Int(_mouseWorldPosition)))
                    return;
                if (selectedBrushBinding.GetValue() == BrushType.Pellet)
                {
                    currentWorkingLevel.pelletCount--;
                }

                _levelEditManager.EraseObject(_mouseWorldPosition);


            }
        }

        public void OnLeftClickChanged(bool newValue)
        {
            _isLeftClicking = newValue;
        }

        public void OnRightClickChanged(bool newValue)
        {
            _isRightClicking = newValue;
        }

        public override void OnRendered()
        {
            throw new NotImplementedException();
        }
    }

    public enum BrushType
    {
        Wall, Pellet, PowerPellet, PacMan,
        Blinky,
        Pinky,
        Inky,
        Clyde,
        GhostHouse, GhostDoor
    }
    
    [Serializable]
    public enum LevelObjectType
    {
        Wall, Pellet, PowerPellet,
        Blinky,
        Pinky,
        Inky,
        Clyde
    }
}
