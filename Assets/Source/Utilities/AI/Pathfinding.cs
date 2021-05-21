using UnityEngine;

namespace UnitMan.Source.Utilities.AI
{
    public static class Pathfinding
    {
        private static int TaxiCabDistance(Vector2Int start, Vector2Int end)
        {
            return Mathf.Abs(start.y - end.y) + Mathf.Abs(start.x - end.x);
        }
    }
}