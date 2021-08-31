using System;
using System.Collections.Generic;
using UnitMan.Source.Entities;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityObject = UnityEngine.Object;

namespace UnitMan.Source.Utilities.Pathfinding
{
    //TODO: refactor/organize this class
    [RequireComponent(typeof(Tilemap))]
    public class LevelGridController : MonoBehaviour
    {
        public static readonly Vector2 Zero = Vector2.zero;
        public static readonly Vector2Int UpVector2Int = Vector2Int.up;
        public static readonly Vector2Int DownVector2Int = Vector2Int.down;
        public static readonly Vector2Int LeftVector2Int = Vector2Int.left;
        public static readonly Vector2Int RightVector2Int = Vector2Int.right;
        
        public static readonly Vector2 UpVector2 = Vector2.up;
        public static readonly Vector2 DownVector2 = Vector2.down;
        public static readonly Vector2 LeftVector2 = Vector2.left;
        public static readonly Vector2 RightVector2 = Vector2.right;
        

        // public Tilemap walkableTilemap;
        public Tilemap wallTilemap;
        
        [SerializeField]
        private TileBase wallRuleTile;
        
        // [SerializeField]
        // private TileBase walkableTile;


        private bool[][] _grid;
        // public MazeData mazeData;
        public Level level;
        [SerializeField]
        private GameObject pelletPrefab;

        [SerializeField]
        private GameObject powerPelletPrefab;

        [SerializeField]
        private GameObject blinkyPrefab;

        [SerializeField]
        private GameObject pinkyPrefab;

        [SerializeField]
        private GameObject inkyPrefab;

        [SerializeField]
        private GameObject clydePrefab;

        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private Transform ghostDoorTransform;

        public bool GetGridPosition(Vector2Int vector) {
            // Debug.Log($"{x}, {y}");
            return wallTilemap.GetTile(wallTilemap.WorldToCell(VectorUtil.ToVector3Int(vector))) == null;
        }
        private void SetGridPosition(Vector2Int position, bool value)
        {
            Vector2Int gridOrigin = level.bottomLeftPosition + Vector2Int.one;
            _grid[position.y - gridOrigin.y][position.x - gridOrigin.x] = value;
        }


        public static LevelGridController Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake()
        {
            Debug.Log("Grid should be initializing now");
            Instance = this;
            //bools are canWalk

        }

        private void Start()
        {
            level = CrossSceneLevelContainer.Instance.level;
            LoadLevel();
        }

        private void LoadLevel()
        {
            InitializeGrid();
            PopulateLevel();
            AddUniqueObjects();
        }

        private void AddUniqueObjects()
        {
            playerTransform.position = VectorUtil.ToVector3(level.pacManPosition);
            ghostDoorTransform.position = VectorUtil.ToVector3(level.ghostDoorPosition);
        }

        private void PopulateLevel()
        {
            Quaternion identity = Quaternion.identity;
            for (int i = 0; i < level.objectPositions.Count; i++)
            {
                if (level.objectTypes[i] == LevelObjectType.Wall) continue;
                Instantiate(level.objectTypes[i] switch
                {
                    LevelObjectType.Pellet => pelletPrefab,
                    LevelObjectType.PowerPellet => powerPelletPrefab,
                    LevelObjectType.Blinky => blinkyPrefab,
                    LevelObjectType.Pinky => pinkyPrefab,
                    LevelObjectType.Inky => inkyPrefab,
                    LevelObjectType.Clyde => clydePrefab,
                    _ => throw new ArgumentOutOfRangeException()
                }, VectorUtil.ToVector3(level.objectPositions[i]), identity);
            }
        }

        private void InitializeGrid()
        {
            wallTilemap.ClearAllTiles();
            BuildTilemapFromLevelObject(level);
            BoundsInt cellBounds = wallTilemap.cellBounds;
            _grid = new bool[cellBounds.size.y][];
            
            // int mazeDataMapWidth = cellBounds.size.x;
            for (int i = 0; i < _grid.Length; i++)
            {
                
                _grid[i] = new bool[cellBounds.size.x];
            }
            
            foreach (Vector2Int position in GetAllTilePositions(wallTilemap)) {
                // grid.Add(position, true);
                SetGridPosition(position, true);
            }
            
        }

        private void BuildTilemapFromLevelObject(Level levelObject)
        {
            for (int i = 0; i < levelObject.objectPositions.Count; i++)
            {
                if (levelObject.objectTypes[i] != LevelObjectType.Wall) continue;
                wallTilemap.SetTile(VectorUtil.ToVector3Int(levelObject.objectPositions[i]), wallRuleTile);
            }
            
        }

        private static Vector2Int[] GetAllTilePositions(Tilemap tilemap) {
            List<Vector2Int> positions = new List<Vector2Int>();
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(position)) {
                    positions.Add(VectorUtil.ToVector2Int(position));
                }
            }

            return positions.ToArray();
        }

        public static int TaxiCabDistance(Vector2Int start, Vector2Int end) {
            return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        }
        
        // public static float TaxiCabDistanceVector3(Vector3 start, Vector3 end) {
        //     return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        // }

        public void CheckPossibleTurns(Vector2Int position, bool[] turns) {
            turns[(int) Actor.Direction.Up] = GetGridPosition(position + UpVector2Int);
            turns[(int) Actor.Direction.Down] = GetGridPosition(position + DownVector2Int);
            turns[(int) Actor.Direction.Left] = GetGridPosition(position + LeftVector2Int);
            turns[(int) Actor.Direction.Right] = GetGridPosition(position + RightVector2Int);
            //TODO: remove redundant alloc
        }

        // private void OnDrawGizmos() {
        //     InitializeGrid();
        //     Vector2Int vectorPosition = new Vector2Int();
        //     for (int x = -11; x <= 11; x++) {
        //         for (int y = -21; y <= 4; y++)
        //         {
        //             vectorPosition.x = x;
        //             vectorPosition.y = y;
        //             Gizmos.color = GetGridPosition(vectorPosition) ? Color.green : Color.red;
        //             Gizmos.DrawSphere(new Vector3(x, y, 0f), 0.1f);
        //         }
        //     }
        // }
    }
}
