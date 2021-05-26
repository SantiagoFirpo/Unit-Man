using System;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Utilities.AI;
using UnityEngine;

namespace UnitMan.Source.Utilities
{
    public static class Astar
    {
        public static Queue<PathNode> ShortestPathBetween(Vector2Int startPosition, Vector2Int endPosition) {
            PathNode startNode = new PathNode(startPosition, startPosition, endPosition);
            startNode.costFromStart = 0;
            startNode.CalculateTotalCost();
            PathNode endNode = new PathNode(endPosition, startPosition, endPosition);

            

            List<PathNode> nodesToCheck = new List<PathNode>() {startNode};
            List<PathNode> checkedNodes = new List<PathNode>();

            Queue<PathNode> path = new Queue<PathNode>();

            int totalCells = PathGrid.Instance.grid.GetLength(0);
            // for (int i = 0; i < totalCells; i++)
            while (nodesToCheck.Count > 0) {
                PathNode currentNode = nodesToCheck[FindSmallestTotalCost(nodesToCheck.ToArray())];
                
                if (currentNode.position == endPosition) {
                    return TracePath(startNode, endNode);
                }

                nodesToCheck.Remove(currentNode);
                checkedNodes.Add(currentNode);
                List<PathNode> neighborNodes = currentNode.CreateNeighbors().ToList();

                foreach (PathNode neighbor in neighborNodes) {
                    if (checkedNodes.Contains(neighbor)) continue;
                    int tentativeCost = currentNode.costFromStart +
                                                 PathGrid.TaxiCabDistance(neighbor.position, currentNode.position);
                    if (tentativeCost < neighbor.costFromStart) {
                        neighbor.previousNode = currentNode;
                        neighbor.costFromStart = tentativeCost;
                        neighbor.CalculateTotalCost();

                        if (!nodesToCheck.Contains(neighbor)) {
                            nodesToCheck.Add(neighbor);
                        }
                    }
                }
                // // Vector2Int[] neighborPositions = currentNode.GetNeighborPositions().Except(pathPositions).Except(PathGrid.Instance.grid).ToArray();
                // PathNode cheapestNeighbor = neighborNodes[FindSmallestTotalCost(neighborNodes.ToArray())];
                // currentNode = cheapestNeighbor;
                // neighborNodes.Remove(cheapestNeighbor);
                // checkedNodes.AddRange(neighborNodes);
                // foreach (PathNode node in neighborNodes) {
                //     Debug.Log($"{node.position} neighbor has a cost of {node.totalCost}");
                // }
                //
                // i++;
                // Debug.Log(
                //     $"iteration {i}, in position {currentNode.position}, with {currentNode.distanceToEndHeuristic} tiles remaining and {currentNode.totalCost} cost");
                // path.Enqueue(currentNode);
                // checkedNodes.Add(currentNode);

                Debug.LogError($"Agent never reached end position, stopped at{currentNode.position}");
                return null;
            }


            return TracePath(startNode, endNode);
        }

        private static Queue<PathNode> TracePath(PathNode startNode, PathNode endNode) {
            Queue<PathNode> path = new Queue<PathNode>();
            PathNode currentNode = endNode;
            path.Enqueue(endNode);
            PathNode previousNode = currentNode.previousNode;
            while (previousNode != null) {
                path.Enqueue(previousNode);
                currentNode = previousNode;
            }

            return path;
        }

        private static readonly Func<PathNode, Vector2Int> PathNodePositionSelector = (PathNode x) => x.position;

        private static int FindSmallestTotalCost(PathNode[] nodes) {
            PathNode currentCheapest = nodes[0];
            for (int i = 1; i < nodes.Length; i++) {
                if (currentCheapest.totalCost > nodes[i].totalCost ||
                    (currentCheapest.totalCost == nodes[i].totalCost))
                    currentCheapest = nodes[i];
            }

            return Array.IndexOf(nodes, currentCheapest);
        }
    }
}