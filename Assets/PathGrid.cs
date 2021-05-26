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
            // _tilemap.SetTile(Vector3Int.zero, null);
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
