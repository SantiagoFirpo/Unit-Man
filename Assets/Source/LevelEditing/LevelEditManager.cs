using System;
using System.Linq;
using UnitMan.Source.UI.Components.LevelEditor;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnitMan.Source.LevelEditing
{
    public class LevelEditManager
    {
        public LevelEditManager(LevelEditorViewModel levelEditorViewModel)
        {
            this.levelEditorViewModel = levelEditorViewModel;
            levelLoadManager = new LevelLoadManager(this);
        }

        public readonly LevelLoadManager levelLoadManager;

        public readonly LevelEditorViewModel levelEditorViewModel;

        public void PlaceLevelObjectAndUpdateMaze(BrushType brush, Vector3 position)
        {
            Vector2Int positionV2Int = VectorUtil.ToVector2Int(position);
            if (!levelEditorViewModel.IsPositionValid(positionV2Int)) return;
            switch (brush)
            {
                case BrushType.Wall:
                {
                    levelEditorViewModel.wallTilemap.SetTile(levelEditorViewModel.mouseTilesetPosition, levelEditorViewModel.wallRuleTile);
                    levelEditorViewModel.currentWorkingLevel.AddLevelObject(LevelObjectType.Wall, positionV2Int);
                    break;
                }
                case BrushType.PacMan:
                {
                    levelEditorViewModel.currentWorkingLevel.pacManPosition = positionV2Int;
                    levelEditorViewModel.pacManTransform.position = position;
                    break;
                }
                case BrushType.GhostHouse:
                {
                    levelEditorViewModel.currentWorkingLevel.ghostHousePosition = positionV2Int;
                    levelEditorViewModel.ghostHouseTransform.position = position;
                    break;
                }
                case BrushType.GhostDoor:
                {
                    levelEditorViewModel.currentWorkingLevel.ghostDoorPosition = positionV2Int;
                    levelEditorViewModel.ghostDoor.position = position;
                    break;
                }
                default:
                {
                    if (brush == BrushType.Pellet)
                    {
                        levelEditorViewModel.currentWorkingLevel.pelletCount++;
                    }

                    LevelObjectType brushToLevelObjectType = LevelEditorViewModel.BrushToLevelObjectType(brush);
                    levelEditorViewModel.currentWorkingLevel.AddLevelObject(brushToLevelObjectType, positionV2Int);
                    levelEditorViewModel.localObjects.Add(position, CreateGameObject(brushToLevelObjectType, position));
                    break;
                }
            }
            
            
        }

        public GameObject AddLocalLevelObject(LevelObjectType objectType, Vector3 position)
        {
            GameObject gameObject = null;
            switch (objectType)
            {
                case LevelObjectType.Wall:
                    levelEditorViewModel.wallTilemap.SetTile(VectorUtil.ToVector3Int(position), levelEditorViewModel.wallRuleTile);
                    break;
                default:
                    gameObject = CreateGameObject(objectType, position);
                    levelEditorViewModel.localObjects.Add(position, gameObject);
                    break;
            }
            return gameObject;
        }

        public GameObject CreateGameObject(LevelObjectType objectType, Vector3 position)
        {
            return objectType == LevelObjectType.Wall ? null :
                Object.Instantiate(LevelObjectTypeToPrefab(objectType), position, levelEditorViewModel.identity);
        }

        private GameObject LevelObjectTypeToPrefab(LevelObjectType objectType)
        {
            return objectType switch
            {
                LevelObjectType.Blinky => levelEditorViewModel.blinkyMarkerPrefab,
                LevelObjectType.Pinky => levelEditorViewModel.pinkyMarkerPrefab,
                LevelObjectType.Inky => levelEditorViewModel.inkyMarkerPrefab,
                LevelObjectType.Clyde => levelEditorViewModel.clydeMarkerPrefab,
                LevelObjectType.Pellet => levelEditorViewModel.pelletMarkerPrefab,
                LevelObjectType.PowerPellet => levelEditorViewModel.powerMarkerPrefab,
                LevelObjectType.Wall => null,
                LevelObjectType.ScreenWrap => levelEditorViewModel.screenWrapMarkerPrefab,
                _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null)
            };
        }

        public void EraseObject(Vector3 position)
        {
            levelEditorViewModel.currentWorkingLevel.RemoveLevelObject(VectorUtil.ToVector2Int(position));
            bool isWall = levelEditorViewModel.wallTilemap.GetTile(VectorUtil.ToVector3Int(position)) != null;
            if (isWall)
            {
                levelEditorViewModel.wallTilemap.SetTile(levelEditorViewModel.mouseTilesetPosition, null);
            }
            else
            {
                Object.Destroy(levelEditorViewModel.localObjects[position]);
                levelEditorViewModel.localObjects.Remove(position);
            }
        }
    }
}