using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class Agent : MonoBehaviour
    {
        private Queue<Vector2Int> destinations;
        private Vector2Int gridPosition;

        // private static Vector2Int[] ShortestPath(Vector2Int start, Vector2Int end, PathGrid grid)
        // {
        //     Vector2Int[] neighborPositions = GetNeighborPositions(start);
        //     Debug.Log(neighborPositions);
        //     return neighborPositions;
        // }

        private static Vector2Int[] GetNeighborPositions(Vector2Int position)
        {
            List<Vector2Int> neighborBuffer = new List<Vector2Int>();
            for (int neighborX = -1; neighborX < 1; neighborX++)
            {
                for (int neighborY = -1; neighborY < 1; neighborY++)
                {
                    if (neighborX != neighborY)
                    {
                        neighborBuffer.Add(new Vector2Int(neighborX, neighborY) + position);
                    }
                    
                }
            }

            return neighborBuffer.ToArray();
        }

        private void Awake()
        {
            Debug.Log(GetNeighborPositions(Vector2Int.zero)[0]);
        }
    }
}