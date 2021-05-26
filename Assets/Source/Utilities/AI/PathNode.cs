using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class PathNode
    {
        public int costFromStart = 999;
        public int distanceToEndHeuristic = 0;
        public int totalCost = 0;
        
        public PathNode previousNode;
        public Vector2Int position;
        private readonly Vector2Int _startPosition;
        private readonly Vector2Int _endPosition;


        public PathNode(Vector2Int position, Vector2Int startPosition, Vector2Int endPosition) {
            this.position = position;
            _startPosition = startPosition;
            _endPosition = endPosition;
            distanceToEndHeuristic = PathGrid.TaxiCabDistance(this.position, _endPosition);
            totalCost = costFromStart + distanceToEndHeuristic;
        }

        public void CalculateTotalCost() {
            totalCost = costFromStart + distanceToEndHeuristic;
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

            return CreateNeighbors().Select(NeighborPositionSelector).ToArray();
        }

        private static Vector2Int NeighborPositionSelector(PathNode x) => x.position;

        public PathNode[] CreateNeighbors() {
            List<PathNode> neighborBuffer = new List<PathNode>() {};
            for (int neighborX = -1; neighborX <= 1; neighborX++) {
                for (int neighborY = -1; neighborY <= 1; neighborY++) {
                    Vector2Int localNeighborPosition = new Vector2Int(neighborX, neighborY);
                    Vector2Int globalNeighborPosition = position + localNeighborPosition;
                    var grid = PathGrid.Instance.grid;
                    bool isPositionValid = grid.Contains(globalNeighborPosition);
                    bool isCardinal = Mathf.Abs(neighborX) != Mathf.Abs(neighborY);
                    if (isCardinal
                        && isPositionValid) {
                        neighborBuffer.Add(new PathNode(globalNeighborPosition, _startPosition, _endPosition));
                    }
                }
            }

            return neighborBuffer.ToArray();
        }
    }
}