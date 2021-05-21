using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class PathNode
    {
        public int distanceToStart = 0;
        public int distanceToEnd = 0;
        public int totalCost = 0;
        public Vector2Int position;


        public PathNode(Vector2Int position)
        {
            this.position = position;
        }
    }
}