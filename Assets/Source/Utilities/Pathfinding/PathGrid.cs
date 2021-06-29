using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.Utilities.Pathfinding
{
    //TODO: refactor/organize this class
    [RequireComponent(typeof(Tilemap))]
    public class PathGrid : MonoBehaviour
    {
        public bool GetGridPosition(int x, int y) {
            // Debug.Log($"{x}, {y}");
            return _newGrid[-y + 4][x+11];
        }
        
        public bool GetGridPosition(Vector2Int vector) {
            // Debug.Log($"{x}, {y}");
            return _newGrid[-vector.y + 4][vector.x + 11];
        }
        private void SetGridPosition(int x, int y, bool value) {
            _newGrid[-y + 4][x+11] = value;
        }

        private const int Y_SIZE = 26;
        private const int X_SIZE = 23;
        public static Vector2Int UpVector2Int = Vector2Int.up;
        public static Vector2Int DownVector2Int = Vector2Int.down;
        public static Vector2Int LeftVector2Int = Vector2Int.left;
        public static Vector2Int RightVector2Int = Vector2Int.right;
        
        public static readonly Vector2 UpVector2 = Vector2.up;
        public static readonly Vector2 DownVector2 = Vector2.down;
        public static readonly Vector2 LeftVector2 = Vector2.left;
        public static readonly Vector2 RightVector2 = Vector2.right;
        

        public Tilemap walkableTilemap;
        public Tilemap wallTilemap;

        private readonly bool[][] _newGrid = new bool[Y_SIZE][]
        {
            new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE],
            new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE],
            new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE],
            new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE], new bool[X_SIZE],
            new bool[X_SIZE], new bool[X_SIZE]
        };
        public readonly Dictionary<Vector2Int, bool> grid = new Dictionary<Vector2Int, bool>();


        public static PathGrid Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake() {
            Instance = this;


            foreach (Vector2Int position in GetAllTilePositions(walkableTilemap)) {
                // grid.Add(position, true);
                SetGridPosition(position.x, position.y, true);
                
            }

            // foreach (Vector2Int position in GetAllTilePositions(wallTilemap)) {
            //     SetGridPosition(position.x, position.y, false);
            //     // grid.Add(position, false);
            //     SetGridPosition(position.x, position.y,false);
            // }
            //bools are canWalk
           
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

        public void CheckPossibleTurns(Vector3 position, bool[] turns, float colliderSize = 0.9f) {
            Vector2Int tilePosition = VectorToVector2Int(position);
            Vector2Int otherTilePosition = tilePosition;
            float offsetX = position.x - tilePosition.x;
            float offsetY = position.y - tilePosition.y;
            
            bool isBetweenHorizontal = Mathf.Abs(offsetY) < 0.1f && Mathf.Abs(offsetX) >= 0.1f;
            bool isBetweenVertical = Mathf.Abs(offsetX) < 0.1f && Mathf.Abs(offsetY) >= 0.1f;
            if (isBetweenHorizontal) {
                bool isRight = offsetX > 0f;
                if (isRight) {
                    otherTilePosition = tilePosition + RightVector2Int;
                }
                else {
                    otherTilePosition = tilePosition + LeftVector2Int;
                }
            }
            else if (isBetweenVertical) {
                bool isUp = offsetY > 0f;
                if (isUp) {
                    otherTilePosition = tilePosition + UpVector2Int;
                }
                else {
                    otherTilePosition = tilePosition + DownVector2Int;
                }
            }
            
            bool canTurnUp = GetGridPosition(tilePosition.x, tilePosition.y + 1);
            bool canTurnDown = GetGridPosition(tilePosition.x, tilePosition.y - 1);
            bool canTurnLeft = GetGridPosition(tilePosition.x - 1, tilePosition.y);
            bool canTurnRight = GetGridPosition(tilePosition.x + 1, tilePosition.y);
            if (otherTilePosition != tilePosition) {
                canTurnUp = canTurnUp && GetGridPosition(otherTilePosition.x, otherTilePosition.y + 1);
                canTurnDown =  canTurnDown && GetGridPosition(otherTilePosition.x, otherTilePosition.y - 1);
                canTurnLeft = canTurnLeft && GetGridPosition(otherTilePosition.x - 1, otherTilePosition.y);
                canTurnRight = canTurnRight && GetGridPosition(otherTilePosition.x - 1, otherTilePosition.y);
            }
            turns[(int) Actor.Direction.Up] = canTurnUp;
            turns[(int) Actor.Direction.Down] = canTurnDown;
            turns[(int) Actor.Direction.Left] = canTurnLeft;
            turns[(int) Actor.Direction.Right] = canTurnRight;
            
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

        private void OnDrawGizmos() {
            for (int x = -11; x <= 11; x++) {
                for (int y = -21; y <= 4; y++) {
                    Gizmos.color = GetGridPosition(x, y) ? Color.green : Color.red;
                    Gizmos.DrawSphere(new Vector3(x, y, 0f), 0.1f);
                }
            }
        }
    }
}
