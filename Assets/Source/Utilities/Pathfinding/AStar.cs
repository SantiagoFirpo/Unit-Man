using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.Pathfinding
{
    public static class AStar
    {
        public static Queue<Vector2Int> ShortestPathBetween(Vector2Int startPosition, Vector2Int endPosition) {
            
            PathNode startNode = new PathNode(startPosition, startPosition, endPosition) {costFromStart = 0};
            PathNode currentNode = startNode;
            

            List<PathNode> nodesToCheck = new List<PathNode>() {startNode};
            List<PathNode> checkedNodes = new List<PathNode>();
            // for (int i = 0; i < totalCells; i++)
            while (nodesToCheck.Count > 0) {
                currentNode = nodesToCheck[FindSmallestTotalCost(nodesToCheck)];
                nodesToCheck.Remove(currentNode);
                checkedNodes.Add(currentNode);

                if (currentNode.position == endPosition) {
                    Queue<Vector2Int> path = TracePath(startNode, currentNode);
                    return path;
                }

                PathNode[] neighborNodes = currentNode.CreateNeighbors();

                foreach (PathNode neighbor in neighborNodes) {
                    if (checkedNodes.Contains(neighbor) || neighbor == null) continue;
                    int tentativeCost = currentNode.costFromStart +
                                                 PathGrid.TaxiCabDistance(neighbor.position, currentNode.position);
                    if (tentativeCost >= neighbor.costFromStart) continue;
                    neighbor.previousNode = currentNode;
                    neighbor.costFromStart = tentativeCost;

                    if (!nodesToCheck.Contains(neighbor)) {
                        nodesToCheck.Add(neighbor);
                    }
                }

                
            }
            Debug.LogError($"Agent never reached end position, stopped at{currentNode.position}");
            return null;
        }

        private static Queue<Vector2Int> TracePath(PathNode startNode, PathNode endNode) {
            Queue<Vector2Int> path = new Queue<Vector2Int>();
            PathNode currentNode = endNode;
            while (currentNode.previousNode != null) {
                path.Enqueue(currentNode.position);
                currentNode = currentNode.previousNode;
            }

            path = new Queue<Vector2Int>(path.Reverse());
            return path;
        }

        private static readonly Func<PathNode, Vector2Int> PathNodePositionSelector = (PathNode x) => x.position;

        private static int FindSmallestTotalCost(List<PathNode> nodes) {
            PathNode currentCheapest = nodes[0];
            for (int i = 1; i < nodes.Count; i++) {
                if (currentCheapest.TotalCost > nodes[i].TotalCost)
                    currentCheapest = nodes[i];
            }

            return nodes.IndexOf(currentCheapest);
        }
    }
}