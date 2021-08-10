using System.Collections.Generic;
using UnitMan.Source.Config;
using UnitMan.Source.Entities;
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
        

        public Tilemap walkableTilemap;

        private bool[][] _grid;
        public MazeData mazeData;
        
        public bool GetGridPosition(int x, int y) {
            // Debug.Log($"{x}, {y}");
            return _grid[-y + mazeData.originPositionGlobal.y][x - mazeData.originPositionGlobal.x];
        }
        
        public bool GetGridPosition(Vector2Int vector) {
            // Debug.Log($"{x}, {y}");
            return _grid[-vector.y + mazeData.originPositionGlobal.y][vector.x - mazeData.originPositionGlobal.x];
        }
        private void SetGridPosition(int x, int y, bool value) {
            _grid[-y + mazeData.originPositionGlobal.y][x - mazeData.originPositionGlobal.x] = value;
        }



        public static LevelGridController Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake()
        {
            Debug.Log("Grid should be initializing now");
            Instance = this;
            mazeData.CalculateBounds();
            InitializeGrid();
            
            //bools are canWalk
           
        }

        private void InitializeGrid()
        {
            _grid = new bool[mazeData.mapDimensions.y][];
            
            int mazeDataMapWidth = mazeData.mapDimensions.x;
            for (int i = 0; i < _grid.Length; i++)
            {
                
                _grid[i] = new bool[mazeDataMapWidth];
            }
            
            foreach (Vector2Int position in GetAllTilePositions(walkableTilemap)) {
                // grid.Add(position, true);
                SetGridPosition(position.x, position.y, true);
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
        
        public static float TaxiCabDistanceVector3(Vector3 start, Vector3 end) {
            return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        }

        public void CheckPossibleTurns(Vector2Int position, bool[] turns) {
        
            bool canTurnUp = GetGridPosition(position.x, position.y + 1);
            bool canTurnDown = GetGridPosition(position.x, position.y - 1);
            bool canTurnLeft = GetGridPosition(position.x - 1, position.y);
            bool canTurnRight = GetGridPosition(position.x + 1, position.y);
           
            turns[(int) Actor.Direction.Up] = canTurnUp;
            turns[(int) Actor.Direction.Down] = canTurnDown;
            turns[(int) Actor.Direction.Left] = canTurnLeft;
            turns[(int) Actor.Direction.Right] = canTurnRight;
            
        }

        public static bool VectorApproximately(Vector3 v1, Vector2Int v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }
        public static bool VectorApproximately(Vector2 v1, Vector2Int v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }
        public static bool VectorApproximately(Vector3 v1, Vector3 v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }
        public static bool VectorApproximately(Vector2Int v1, Vector3 v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        public static Vector2Int VectorToVector2Int(Vector3 vector) {
            return new Vector2Int(Mathf.RoundToInt(vector.x),
                                  Mathf.RoundToInt(vector.y));
        }

        public static Vector3 Vector2ToVector3(Vector2Int vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }
        
        public static Vector3 Vector2ToVector3(Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }
        
        public static Vector3Int Vector2ToVector3Int(Vector2 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
        }
        
        public static Vector3Int Vector3ToVector3Int(Vector3 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
        }

        public static Vector3 Round(Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }

        private void OnDrawGizmos() {
            InitializeGrid();
            for (int x = -11; x <= 11; x++) {
                for (int y = -21; y <= 4; y++) {
                    Gizmos.color = GetGridPosition(x, y) ? Color.green : Color.red;
                    Gizmos.DrawSphere(new Vector3(x, y, 0f), 0.1f);
                }
            }
        }
    }
}
