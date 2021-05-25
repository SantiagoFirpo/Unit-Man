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

        private static Queue<PathNode> ShortestPathBetween(Vector2Int startPosition, Vector2Int endPosition) {
            PathNode currentNode = new PathNode(startPosition, startPosition, endPosition);

            Queue<PathNode> path = new Queue<PathNode>();
            Queue<Vector2Int> pathPositions = new Queue<Vector2Int>(path.Select(PathNodePositionSelector));

            // int i = 0;
            while (currentNode.distanceToEnd > 0) {
                path.Enqueue(currentNode);
                PathNode[] neighborNodes = currentNode.GetNeighbors().Except(path).ToArray();
                Vector2Int[] neighborPositions = currentNode.GetNeighborPositions().Except(pathPositions).ToArray();
                currentNode = neighborNodes[FindSmallestCost(neighborNodes.ToArray())];
                foreach (PathNode node in neighborNodes) {
                    Debug.Log($"{node.position} neighbor has a cost of {node.totalCost}");
                }

                // i++;
                // Debug.Log(
                //     $"iteration {i}, in position {currentNode.position}, with {currentNode.distanceToEnd} tiles remaining and {currentNode.totalCost} cost");
            }


            return path;
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