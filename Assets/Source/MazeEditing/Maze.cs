using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.MazeEditing
{
    [Serializable]
    public class Maze
    {
        public Vector2Int playerPosition;
        public Vector2Int ghostHousePosition;
        public List<Vector2Int[]> screenWrapPositions = new List<Vector2Int[]>();
        
        public Dictionary<Vector2Int, MazeObjectType> levelObjects = new Dictionary<Vector2Int, MazeObjectType>();

        public Maze()
        {
        }
    }
}