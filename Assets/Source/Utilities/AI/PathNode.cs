using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class PathNode
    {
        public readonly int distanceToStart = 0;
        public readonly int distanceToEnd = 0;
        public readonly int totalCost = 0;
        public Vector2Int position;
        private readonly Vector2Int _startPosition;
        private readonly Vector2Int _endPosition;


        public PathNode(Vector2Int position, Vector2Int startPosition, Vector2Int endPosition) {
            this.position = position;
            _startPosition = startPosition;
            _endPosition = endPosition;
            distanceToStart = PathGrid.TaxiCabDistance(this.position, _startPosition);
            distanceToEnd = PathGrid.TaxiCabDistance(this.position, _endPosition);
            totalCost = distanceToStart + distanceToEnd;
        }

        public Vector2Int[] GetNeighborPositions() {
            // List<Vector2Int> neighborBuffer = new List<Vector2Int>();
            // for (int neighborX = -1; neighborX <= 1; neighborX++) {
            //     for (int neighborY = -1; neighborY <= 1; neighborY++) {
            //         Vector2Int neighborPosition = new Vector2Int(neighborX, neighborY);
            //         if (Mathf.Abs(neighborX) != Mathf.Abs(neighborY) && this.position + neighborPosition != this.position) {
            //             neighborBuffer.Add(neighborPosition + position);
            //         }
            //     }
            // }

            return GetNeighbors().Select(NeighborPositionSelector).ToArray();
        }

        private static Vector2Int NeighborPositionSelector(PathNode x) => x.position;

        public PathNode[] GetNeighbors() {
            List<PathNode> neighborBuffer = new List<PathNode>();
            for (int neighborX = -1; neighborX <= 1; neighborX++) {
                for (int neighborY = -1; neighborY <= 1; neighborY++) {
                    Vector2Int localNeighborPosition = new Vector2Int(neighborX, neighborY);
                    Vector2Int globalNeighborPosition = position + localNeighborPosition;
                    if (Mathf.Abs(neighborX) != Mathf.Abs(neighborY) && globalNeighborPosition != this.position && PathGrid.Instance.grid.Contains(globalNeighborPosition)) {
                        neighborBuffer.Add(new PathNode(globalNeighborPosition, _startPosition, _endPosition));
                    }
                }
            }

            return neighborBuffer.ToArray();
        }
    }
}