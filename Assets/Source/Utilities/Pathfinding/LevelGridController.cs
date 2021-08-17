using System.Collections.Generic;
using UnitMan.Source.Entities;
using UnitMan.Source.LevelEditing;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        public bool GetGridPosition(Vector2Int vector) {
            // Debug.Log($"{x}, {y}");
            return wallTilemap.GetTile(wallTilemap.WorldToCell(VectorUtil.ToVector3Int(vector))) == null;
        }
        private void SetGridPosition(int x, int y, bool value)
        {
            Vector3Int cellPosition = wallTilemap.WorldToCell(new Vector3(x, y, 0f));
            _grid[cellPosition.y][cellPosition.x] = value;
        }



        public static LevelGridController Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake()
        {
            Debug.Log("Grid should be initializing now");
            Instance = this;
            InitializeGrid();
            
            //bools are canWalk
           
        }

        private void InitializeGrid()
        {
            BuildTilemapFromLevelObject(level);
            BoundsInt cellBounds = wallTilemap.cellBounds;
            _grid = new bool[cellBounds.y][];
            
            int mazeDataMapWidth = cellBounds.x;
            for (int i = 0; i < _grid.Length; i++)
            {
                
                _grid[i] = new bool[mazeDataMapWidth];
            }
            
            foreach (Vector2Int position in GetAllTilePositions(wallTilemap)) {
                // grid.Add(position, true);
                SetGridPosition(position.x, position.y, true);
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
                    positions.Add(new Vector2Int(position.x, position.y));
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

        private void OnDrawGizmos() {
            InitializeGrid();
            Vector2Int vectorPosition = new Vector2Int();
            for (int x = -11; x <= 11; x++) {
                for (int y = -21; y <= 4; y++)
                {
                    vectorPosition.x = x;
                    vectorPosition.y = y;
                    Gizmos.color = GetGridPosition(vectorPosition) ? Color.green : Color.red;
                    Gizmos.DrawSphere(new Vector3(x, y, 0f), 0.1f);
                }
            }
        }
    }
}
