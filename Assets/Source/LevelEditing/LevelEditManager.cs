using System;
using System.Collections.Generic;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.UI.Components.LevelEditor;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnitMan.Source.UI
{
    public class LevelEditManager
    {
        private readonly LevelEditorViewModel _levelEditorViewModel;

        public LevelEditManager(LevelEditorViewModel levelEditorViewModel)
        {
            _levelEditorViewModel = levelEditorViewModel;
        }

        public void PlaceLevelObjectAndUpdateMaze(BrushType brush, Vector3 position)
        {
            Vector2Int positionV2Int = VectorUtil.ToVector2Int(position);
            if (!_levelEditorViewModel.IsPositionValid(positionV2Int)) return;
            switch (brush)
            {
                case BrushType.Wall:
                {
                    _levelEditorViewModel.wallTilemap.SetTile(_levelEditorViewModel.mouseTilesetPosition, _levelEditorViewModel.wallRuleTile);
                    _levelEditorViewModel.currentWorkingLevel.AddLevelObject(LevelObjectType.Wall, positionV2Int);
                    break;
                }
                case BrushType.PacMan:
                {
                    _levelEditorViewModel.currentWorkingLevel.pacManPosition = positionV2Int;
                    _levelEditorViewModel.pacManTransform.position = position;
                    break;
                }
                case BrushType.GhostHouse:
                {
                    _levelEditorViewModel.currentWorkingLevel.ghostHousePosition = positionV2Int;
                    _levelEditorViewModel.ghostHouseTransform.position = position;
                    break;
                }
                case BrushType.GhostDoor:
                {
                    _levelEditorViewModel.currentWorkingLevel.ghostDoorPosition = positionV2Int;
                    _levelEditorViewModel.ghostDoor.position = position;
                    break;
                }
                default:
                {
                    if (brush == BrushType.Pellet)
                    {
                        _levelEditorViewModel.currentWorkingLevel.pelletCount++;
                    }

                    LevelObjectType brushToLevelObjectType = LevelEditorViewModel.BrushToLevelObjectType(brush);
                    _levelEditorViewModel.currentWorkingLevel.AddLevelObject(brushToLevelObjectType, positionV2Int);
                    _levelEditorViewModel.localObjects.Add(position, CreateGameObject(brushToLevelObjectType, position));
                    break;
                }
            }
            
            
        }

        public void AddLocalLevelObject(LevelObjectType objectType, Vector3 position)
        {
            if (objectType == LevelObjectType.Wall)
                _levelEditorViewModel.wallTilemap.SetTile(VectorUtil.ToVector3Int(position), _levelEditorViewModel.wallRuleTile);
            else
                _levelEditorViewModel.localObjects.Add(position, CreateGameObject(objectType, position));
        }

        public GameObject CreateGameObject(LevelObjectType objectType, Vector3 position)
        {
            return objectType == LevelObjectType.Wall ? null :
                UnityObject.Instantiate(LevelObjectTypeToPrefab(objectType), position, _levelEditorViewModel.identity);
        }

        private GameObject LevelObjectTypeToPrefab(LevelObjectType objectType)
        {
            return objectType switch
            {
                LevelObjectType.Blinky => _levelEditorViewModel.blinkyMarkerPrefab,
                LevelObjectType.Pinky => _levelEditorViewModel.pinkyMarkerPrefab,
                LevelObjectType.Inky => _levelEditorViewModel.inkyMarkerPrefab,
                LevelObjectType.Clyde => _levelEditorViewModel.clydeMarkerPrefab,
                LevelObjectType.Pellet => _levelEditorViewModel.pelletMarkerPrefab,
                LevelObjectType.PowerPellet => _levelEditorViewModel.powerMarkerPrefab,
                LevelObjectType.Wall => null,
                _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null)
            };
        }

        public void PopulateEditorFromLevelObject(Level level)
        {
            ClearLevel();
            AddLocalObjects(level);
            SetUniqueObjectPositions(level);
        }

        private void AddLocalObjects(Level level)
        {
            int objectPositionsCount = level.objectPositions.Count;
            for (int i = 0; i < objectPositionsCount; i++)
            {
                Vector3Int positionV3Int = VectorUtil.ToVector3Int(level.objectPositions[i]);
                // if (level.objectTypes[i] == LevelObjectType.Wall)
                // {
                //     wallTilemap.SetTile(positionV3Int, wallRuleTile);
                // }
                AddLocalLevelObject(level.objectTypes[i], VectorUtil.ToVector3(positionV3Int));
                
            }
        }

        private void SetUniqueObjectPositions(Level level)
        {
            _levelEditorViewModel.pacManTransform.position = VectorUtil.ToVector3(level.pacManPosition);
            _levelEditorViewModel.ghostHouseTransform.position = VectorUtil.ToVector3(level.ghostHousePosition);
            _levelEditorViewModel.ghostDoor.position = VectorUtil.ToVector3(level.ghostDoorPosition);
        }

        private void ClearLevel()
        {
            foreach (KeyValuePair<Vector3, GameObject> localObject in _levelEditorViewModel.localObjects)
            {
                UnityObject.Destroy(localObject.Value);
            }

            _levelEditorViewModel.localObjects.Clear();
            _levelEditorViewModel.wallTilemap.ClearAllTiles();
        }

        public void ComputeScatterTargets()
        {
            BoundsInt cellBounds = _levelEditorViewModel.wallTilemap.cellBounds;
            _levelEditorViewModel.currentWorkingLevel.topLeftPosition =
                new Vector2Int(cellBounds.xMin, cellBounds.yMax) + new Vector2Int(-1, 1);
            Vector2Int vectorOne = Vector2Int.one;
            _levelEditorViewModel.currentWorkingLevel.topRightPosition = VectorUtil.ToVector2Int(cellBounds.max) + vectorOne;
            _levelEditorViewModel.currentWorkingLevel.bottomLeftPosition = VectorUtil.ToVector2Int(cellBounds.min) - vectorOne;
            _levelEditorViewModel.currentWorkingLevel.bottomRightPosition = new Vector2Int(cellBounds.xMax, cellBounds.yMin) + new Vector2Int(1, -1);
        }

        public void EraseObject(Vector3 position)
        {
            _levelEditorViewModel.currentWorkingLevel.RemoveLevelObject(VectorUtil.ToVector2Int(position));
            bool isWall = _levelEditorViewModel.wallTilemap.GetTile(VectorUtil.ToVector3Int(position)) != null;
            if (isWall)
            {
                _levelEditorViewModel.wallTilemap.SetTile(_levelEditorViewModel.mouseTilesetPosition, null);
            }
            else
            {
                UnityObject.Destroy(_levelEditorViewModel.localObjects[position]);
                _levelEditorViewModel.localObjects.Remove(position);
            }
        }
    }
}