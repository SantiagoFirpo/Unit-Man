using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

namespace UnitMan
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
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(this.gameObject);
            }
            walkableTilemap = GetComponent<Tilemap>();
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
                    positions.Add(Vector2Int.RoundToInt((Vector3) position));
                }
            }
            return positions.ToArray();
        }

        public static int TaxiCabDistance(Vector2Int start, Vector2Int end) {
            return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        }
    }
}
