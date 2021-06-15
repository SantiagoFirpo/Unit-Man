using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.Utilities.Pathfinding
{
    [RequireComponent(typeof(Tilemap))]
    public class PathGrid : MonoBehaviour
    {
        
        public Tilemap walkableTilemap;
        public Tilemap wallTilemap;
        
        public readonly Dictionary<Vector2Int, bool> grid = new Dictionary<Vector2Int, bool>();


        public static PathGrid Instance {get; private set; }
        // Start is called before the first frame update

        private void Awake()
        {
            Instance = this;
            
           
            foreach (Vector2Int position in GetAllTilePositions(walkableTilemap)) {
                grid.Add(position, true);
            }
            foreach (Vector2Int position in GetAllTilePositions(wallTilemap)) {
                grid.Add(position, false);
            }
            // walkableTilemap.SetTile(Vector3Int.zero, null);
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

        // public static void CheckPossibleTurns(Vector3 position,  out bool[] turns, float colliderSize = 0.9f) {
        //     Vector2Int tilePosition = VectorToVector2Int(position);
        //     Vector2Int secondaryPosition = tilePosition;
        //     
        //     if (position.x - tilePosition.x > 0.1f) {
        //         Vector2Int roundToInt = Vector2Int.RoundToInt(Actor.Right);
        //         secondaryPosition =  tilePosition + roundToInt;
        //     }
        //
        //     else if (position.y - tilePosition.y > ) {
        //         
        //     }
        //     
        //     for (int x = -1; x <= 1; x++) {
        //         for (int y = -1; y <= 1; y++) {
        //             if (x == y) continue;
        //             Vector2Int localPosition = new Vector2Int(x, y);
        //             Vector2Int neighborPosition = tilePosition + localPosition;
        //             Vector2 colliderPosition = tilePosition + (Vector2) localPosition * colliderSize;
        //             
        //         }
        //     }
        // }
        
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
