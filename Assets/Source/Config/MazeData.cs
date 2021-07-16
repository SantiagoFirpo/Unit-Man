using UnityEngine;

namespace UnitMan.Source.Config
{
    [CreateAssetMenu(menuName = "Maze Data")]
    public class MazeData : ScriptableObject
    {
        
        [Header("Map Position and Dimensions")]
        
        public Vector2Int mapDimensions;

        public Vector2Int originPositionGlobal;
        
        [Header("Ghost markers")]
        
        public Vector2Int hubPosition;
        
        
        public Vector2Int hubExitMarker;
        public Vector2Int restingTargetPosition;
        
        [Header("Collectibles")]
        
        public int pelletCount;
        
        //CALCULATED DURING RUNTIME:
        
        [HideInInspector] public Vector2Int mapCentralPosition;
        
        [HideInInspector]
        public Vector2Int topLeftMapPosition;
        [HideInInspector]
        public Vector2Int topRightMapPosition;
        [HideInInspector]
        public Vector2Int bottomLeftMapPosition;
        [HideInInspector]
        public Vector2Int bottomRightMapPosition;
        

        public void CalculateBounds()
        {
            topLeftMapPosition = originPositionGlobal + new Vector2Int(-1, 1);
            
            topRightMapPosition =
                new Vector2Int(originPositionGlobal.x + mapDimensions.x, originPositionGlobal.y + 1);

            bottomLeftMapPosition =
                new Vector2Int(originPositionGlobal.x - 1, originPositionGlobal.y - mapDimensions.y);

            bottomRightMapPosition = new Vector2Int(originPositionGlobal.x + mapDimensions.x,
                                                    originPositionGlobal.y - mapDimensions.y);

            mapCentralPosition = originPositionGlobal + mapDimensions / 2;
        }
    }
}
