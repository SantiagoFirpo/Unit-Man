using System;
using System.Collections;
using System.Collections.Generic;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan
{
    [RequireComponent(typeof(Tilemap))]
    public class PathGrid : MonoBehaviour
    {
        private Tilemap _tilemap;
        public Vector2Int[] grid;


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
            _tilemap = GetComponent<Tilemap>();
            grid = GetAllTilePositions(_tilemap);
            Debug.Log(_tilemap.GetTile(new Vector3Int(1, 0, 0)));
        }

        private static Vector2Int[] GetAllTilePositions(Tilemap tilemap)
        {
            List<Vector2Int> buffer = new List<Vector2Int>() { };
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
                Vector3Int tilePosition;
                if (tilemap.HasTile(position)) {
                    buffer.Add(new Vector2Int((int) tilemap.CellToWorld(position).x, (int) tilemap.CellToWorld(position).y));
                }
                // Debug.Log(position);
                // Debug.Log($"{position.x}, {position.y}");
            }

            return buffer.ToArray();
        }
        
        public static int TaxiCabDistance(Vector2Int start, Vector2Int end) {
            return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        }
    }
}
