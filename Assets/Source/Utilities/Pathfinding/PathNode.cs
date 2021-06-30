using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.Pathfinding
{
    public class PathNode
    {
        public int costFromStart = 999;
        public int distanceToEndHeuristic = 0;

        public int TotalCost => costFromStart + distanceToEndHeuristic;

        public PathNode previousNode;
        public Vector2Int position;
        public bool searched = false;
        
        private Vector2Int _startPosition;
        private Vector2Int _endPosition;


        public PathNode(Vector2Int position, Vector2Int startPosition, Vector2Int endPosition) {
            this.position = position;
            _startPosition = startPosition;
            _endPosition = endPosition;
            distanceToEndHeuristic = LevelGridController.TaxiCabDistance(this.position, _endPosition);
        }

        public void Reconstruct(Vector2Int position, Vector2Int startPosition, Vector2Int endPosition) {
            this.position = position;
            _startPosition = startPosition;
            _endPosition = endPosition;
            distanceToEndHeuristic = LevelGridController.TaxiCabDistance(this.position, _endPosition);
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
           PathNode[] neighbors = new PathNode[4];
            int i = 0;
            for (int neighborX = -1; neighborX <= 1; neighborX++) {
                for (int neighborY = -1; neighborY <= 1; neighborY++) {
                    Vector2Int localNeighborPosition = new Vector2Int(neighborX, neighborY);
                    Vector2Int globalNeighborPosition = position + localNeighborPosition;
                    bool isPositionValid = LevelGridController.Instance.GetGridPosition(globalNeighborPosition);
                    bool isCardinal = Mathf.Abs(neighborX) != Mathf.Abs(neighborY);
                    if (!isCardinal || !isPositionValid) continue;
                    neighbors[i] = new PathNode(globalNeighborPosition, _startPosition, _endPosition);
                    i++;

                }
            }

            return neighbors;
        }
    }
}