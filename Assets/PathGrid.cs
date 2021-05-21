using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan
{
    public class PathGrid : MonoBehaviour
    {
        private Tilemap _tilemap;
        public Vector2Int[] grid;

        // Start is called before the first frame update
        private void Awake()
        {
            _tilemap = GetComponent<Tilemap>();
            grid = GetAllTilePositions(_tilemap);
        }

        private static Vector2Int[] GetAllTilePositions(Tilemap tilemap)
        {
            List<Vector2Int> buffer = new List<Vector2Int>() { };
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                // Debug.Log(position);
                // Debug.Log($"{position.x}, {position.y}");
                buffer.Add(new Vector2Int(position.x, position.y));
            }

            return buffer.ToArray();
        }
    }
}
