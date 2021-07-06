using UnityEngine;

namespace UnitMan.Source.Utilities
{
    [CreateAssetMenu(menuName = "Maze Data")]
    public class MazeData : ScriptableObject
    {
        public Vector2Int hubPosition;
        [HideInInspector]
        public Vector2Int topLeftMapPosition;
        [HideInInspector]
        public Vector2Int topRightMapPosition;
        [HideInInspector]
        public Vector2Int bottomLeftMapPosition;
        [HideInInspector]
        public Vector2Int bottomRightMapPosition;

        public Vector2Int hubExitMarker;
        public Vector2Int restingTargetPosition;

        public Vector2Int mapDimensions;

        [HideInInspector] public Vector2Int mapCentralPosition;

        public Vector2Int originPositionGlobal;

        public int pelletCount;

        public void CalculateBounds()
        {
            //origin position = -11, 4
            //map dimensions = 23, 26
            topLeftMapPosition = originPositionGlobal + new Vector2Int(-1, 1); // -12, 5
            
            topRightMapPosition =
                new Vector2Int(originPositionGlobal.x + mapDimensions.x, originPositionGlobal.y + 1); // = 12, 5

            bottomLeftMapPosition =
                new Vector2Int(originPositionGlobal.x - 1, originPositionGlobal.y - mapDimensions.y); //-12, -22

            bottomRightMapPosition = new Vector2Int(originPositionGlobal.x + mapDimensions.x,
                                                    originPositionGlobal.y - mapDimensions.y); //12, -22

            mapCentralPosition = originPositionGlobal + mapDimensions / 2;
        }
    }
}
