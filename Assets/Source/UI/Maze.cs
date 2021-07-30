using System;
using System.Collections.Generic;
using UnitMan.Source.Config;
using UnityEngine;

namespace UnitMan.Source.UI
{
    [Serializable]
    public class Maze
    {
        private MazeData _mazeData;
        private Vector2Int[] _wallTiles;
        private Vector2Int[] _pelletPositions;
        private Vector2Int[] _powerPelletPositions;
        private Vector2Int _playerStartingPosition;
        private Vector2Int _ghostHouse;
        private Dictionary<Vector2Int, GhostType> _ghostInitialPositions;
    }

    public enum GhostType
    {
        Blinky, Pinky, Inky, Clyde
    }
}