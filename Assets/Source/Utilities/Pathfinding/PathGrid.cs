using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.Utilities.Pathfinding
{
    [RequireComponent(typeof(Tilemap))]
    public class PathGrid : MonoBehaviour
    {
        public static Vector2Int Up = Vector2Int.up;
        public static Vector2Int Down = Vector2Int.down;
        public static Vector2Int Left = Vector2Int.left;
        public static Vector2Int Right = Vector2Int.right;

        public Tilemap walkableTilemap;
        public Tilemap wallTilemap;

        public readonly Dictionary<Vector2Int, bool> grid = new Dictionary<Vector2Int, bool>();


        public static PathGrid Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake() {
            Instance = this;


            foreach (Vector2Int position in GetAllTilePositions(walkableTilemap)) {
                grid.Add(position, true);
            }

            foreach (Vector2Int position in GetAllTilePositions(wallTilemap)) {
                grid.Add(position, false);
            }
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
                    otherTilePosition = tilePosition + Right;
                }
                else {
                    otherTilePosition = tilePosition + Left;
                }
            }
            else if (isBetweenVertical) {
                bool isUp = offsetY > 0f;
                if (isUp) {
                    otherTilePosition = tilePosition + Up;
                }
                else {
                    otherTilePosition = tilePosition + Down;
                }
            }
            
            bool canTurnUp = grid[tilePosition + Up];
            bool canTurnDown = grid[tilePosition + Down];
            bool canTurnLeft = grid[tilePosition + Left];
            bool canTurnRight = grid[tilePosition + Right];
            if (otherTilePosition != tilePosition) {
                canTurnUp = canTurnUp && grid[otherTilePosition + Up];
                canTurnDown =  canTurnDown && grid[otherTilePosition + Down];
                canTurnLeft = canTurnLeft && grid[otherTilePosition + Left];
                canTurnRight = canTurnRight && grid[otherTilePosition + Right];
            }
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
    }
}
