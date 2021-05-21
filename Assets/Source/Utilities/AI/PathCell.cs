using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public class PathCell
    {
        public int distanceToStart = 0;
        public int distanceToEnd = 0;
        public int totalCost = 0;
        public Vector2Int position;


        public PathCell(Vector2Int position)
        {
            this.position = position;
        }
    }
}