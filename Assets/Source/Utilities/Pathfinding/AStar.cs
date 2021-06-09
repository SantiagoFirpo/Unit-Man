using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.Pathfinding
{
    public static class AStar
    {
        public static Queue<PathNode> nodePool = new Queue<PathNode>();

        public static void InitializeNodePool() {
            Vector2Int maxPosition = Vector2Int.one * 120;
            for (int i = 0; i < 200; i++) {
                nodePool.Enqueue(new PathNode(maxPosition, maxPosition, maxPosition));
            }
        }
        private static PathNode GetPoolNode(Vector2Int position, Vector2Int startPosition, Vector2Int endPosition) {
            if (nodePool.Count == 0) return null;
            PathNode node = nodePool.Peek();
            nodePool.Dequeue();
            node.Reconstruct(position, startPosition, endPosition);
            return node;
        }
        public static Queue<Vector2Int> ShortestPathBetween(Vector2Int startPosition, Vector2Int endPosition) {
            
            PathNode startNode = new PathNode(startPosition, startPosition, endPosition) {costFromStart = 0};
            PathNode currentNode = startNode;
            
            List<PathNode> nodesToCheck = new List<PathNode>() {startNode};
            // for (int i = 0; i < totalCells; i++)
            while (nodesToCheck.Count > 0) {
                currentNode = nodesToCheck[FindSmallestTotalCost(nodesToCheck)];
                nodesToCheck.Remove(currentNode);
                currentNode.searched = true;

                if (currentNode.position == endPosition) {
                    Queue<Vector2Int> path = TracePath(startNode, currentNode);
                    return path;
                }

                PathNode[] neighborNodes = currentNode.CreateNeighbors();

                SearchNeighbors(neighborNodes, currentNode, nodesToCheck);

                
            }
            // Debug.LogError($"Agent never reached end position, stopped at{currentNode.position}");
            return null;
        }

        private static void SearchNeighbors(PathNode[] neighborNodes, PathNode currentNode, List<PathNode> nodesToCheck) {
            foreach (PathNode neighbor in neighborNodes) {
                if (neighbor == null || neighbor.searched) continue;
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