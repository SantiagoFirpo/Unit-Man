using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class Agent : MonoBehaviour
    {
        private Queue<Vector2Int> destinations;
        private Vector2Int gridPosition;

        public Queue<PathNode> ShortestPathBetween(Vector2Int startPosition, Vector2Int endPosition) {
            PathNode currentNode = new PathNode(startPosition, startPosition, endPosition);
            PathNode endNode = new PathNode(endPosition, startPosition, endPosition);
            
            List<PathNode> checkedNodes = new List<PathNode>();
            Queue<PathNode> nodesToCheck = new Queue<PathNode>();

            Queue<PathNode> path = new Queue<PathNode>();

            int totalCells = PathGrid.Instance.grid.GetLength(0);
            for (int i = 0; i < totalCells; i++) {
                path.Enqueue(currentNode);
                checkedNodes.Add(currentNode);
                List<PathNode> neighborNodes = currentNode.CreateNeighbors()
                    .Except(checkedNodes).ToList();
                // Vector2Int[] neighborPositions = currentNode.GetNeighborPositions().Except(pathPositions).Except(PathGrid.Instance.grid).ToArray();
                PathNode cheapestNeighbor = neighborNodes[FindSmallestCost(neighborNodes.ToArray())];
                currentNode = cheapestNeighbor;
                neighborNodes.Remove(cheapestNeighbor);
                checkedNodes.AddRange(neighborNodes);
                foreach (PathNode node in neighborNodes) {
                    Debug.Log($"{node.position} neighbor has a cost of {node.totalCost}");
                }

            i++;
            Debug.Log(
                $"iteration {i}, in position {currentNode.position}, with {currentNode.distanceToEnd} tiles remaining and {currentNode.totalCost} cost");
            path.Enqueue(currentNode);
            checkedNodes.Add(currentNode);
            }

            if (currentNode.position == endPosition) {
                return path;
            }
            else {
                Debug.LogError($"Agent never reached end position, stopped at{currentNode.position}");
                return null;
            }
        }

        private static readonly Func<PathNode, Vector2Int> PathNodePositionSelector = (PathNode x) => x.position;

        private static int FindSmallestCost(PathNode[] nodes) {
            PathNode currentCheapest = nodes[0];
            for (int i = 1; i < nodes.Length; i++) {
                if (currentCheapest.totalCost > nodes[i].totalCost ||
                    (currentCheapest.totalCost == nodes[i].totalCost &&
                     currentCheapest.distanceToEnd > nodes[i].distanceToEnd))
                    currentCheapest = nodes[i];
            }

            return Array.IndexOf(nodes, currentCheapest);
        }
        // private void Awake() {
        //     Debug.Log(ShortestPathBetween(Vector2Int.zero, 5 * Vector2Int.up + 2 * Vector2Int.left).Peek().distanceToStart);
        // }
    }
}