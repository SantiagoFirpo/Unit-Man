using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.MazeEditing
{
    [Serializable]
    public class Level : ScriptableObject
    {
        public Vector2Int pacManPosition = Vector2Int.zero;
        public Vector2Int ghostHousePosition = new Vector2Int(0, 3);
        public List<Vector2Int[]> screenWrapPositions = new List<Vector2Int[]>();
        public Vector2Int blinkyScatterTarget = Vector2Int.zero;
        public Vector2Int pinkyScatterTarget = Vector2Int.zero;
        public Vector2Int inkyScatterTarget = Vector2Int.zero;
        public Vector2Int clydeScatterTarget = Vector2Int.zero;
        public int pelletCount;

        public List<Vector2Int> objectPositions = new List<Vector2Int>();
        public List<LevelObjectType> objectTypes = new List<LevelObjectType>();
        
        // public Dictionary<Vector2Int, MazeObjectType> levelObjects = new Dictionary<Vector2Int, MazeObjectType>();

        // public void SerializeLevelObjects()
        // {
        //     objectPositions = levelObjects.Keys.ToList();
        //     objectTypes = levelObjects.Values.ToList();
        // }

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