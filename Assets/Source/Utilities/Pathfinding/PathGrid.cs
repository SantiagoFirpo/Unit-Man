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
    }
}
