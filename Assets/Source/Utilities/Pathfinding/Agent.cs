using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.Utilities.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        private Queue<Vector2Int> destinations;
        private Vector2Int gridPosition;

        
        // private void Awake() {
        //     Debug.Log(ShortestPathBetween(Vector2Int.zero, 5 * Vector2Int.up + 2 * Vector2Int.left).Peek().costFromStart);
        // }
    }
}