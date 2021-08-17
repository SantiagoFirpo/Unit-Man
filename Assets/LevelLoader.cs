using System;
using System.Collections;
using System.Collections.Generic;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnitMan
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField]
        private Tilemap wallTilemap;
        [SerializeField]
        private Tilemap walkableTilemap;

        [SerializeField]
        private TileBase wallRuleTile;
        
        // private Dictionary<Vector>
        //TODO: Create wrapper class for loading
    }
}
