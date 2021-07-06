using UnityEngine;

namespace UnitMan.Source.Utilities
{
    [CreateAssetMenu(menuName = "Maze Data")]
    public class MazeData : ScriptableObject
    {
        public Vector2Int hubPosition;
        public Vector2Int topLeftMapPosition;
        public Vector2Int topRightMapPosition;
        public Vector2Int bottomLeftMapPosition;
        public Vector2Int bottomRightMapPosition;

        public Vector2Int hubExitMarker;
        public Vector2Int restingTargetPosition;

        public int mapWidth;
        public int mapHeight;

        public Vector2Int originPositionGlobal;
    }
}
