using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.LevelEditing
{
    [Serializable]
    public class Level
    {
        public string id;
        public Vector2Int pacManPosition = Vector2Int.zero;
        public Vector2Int ghostHousePosition = new Vector2Int(0, 3);

        public Vector2Int ghostDoorPosition = new Vector2Int(0, 1);
        //TODO: change to Y-value and X-value lists
        public List<Vector2Int[]> screenWrapPositions = new List<Vector2Int[]>();
        public Vector2Int topRightPosition = Vector2Int.zero;
        public Vector2Int topLeftPosition = Vector2Int.zero;
        public Vector2Int bottomRightPosition = Vector2Int.zero;
        public Vector2Int bottomLeftPosition = Vector2Int.zero;
        public int pelletCount;

        public List<Vector2Int> objectPositions = new List<Vector2Int>();
        public List<LevelObjectType> objectTypes = new List<LevelObjectType>();


        public void AddLevelObject(LevelObjectType objectType, Vector2Int position)
        {
            objectTypes.Add(objectType);
            objectPositions.Add(position);
        }
        
        public void RemoveLevelObject(Vector2Int position)
        {
            objectTypes.RemoveAt(objectPositions.IndexOf(position));
            objectPositions.Remove(position);
        }
    }
}