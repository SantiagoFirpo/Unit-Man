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

        public void PlaceLevelObjectAndUpdateMaze(BrushMode brush, Vector3 position)
        {
            Vector2Int positionV2Int = VectorUtil.ToVector2Int(position);
            if (!levelEditorViewModel.IsPositionValid(positionV2Int)) return;
            switch (brush)
            {
                case BrushMode.Wall:
                {
                    levelEditorViewModel.wallTilemap.SetTile(levelEditorViewModel.mouseTilesetPosition, levelEditorViewModel.wallRuleTile);
                    levelEditorViewModel.currentWorkingLevel.AddLevelObject(LevelObject.Wall, positionV2Int);
                    break;
                }
                case BrushMode.PacMan:
                {
                    levelEditorViewModel.currentWorkingLevel.pacManPosition = positionV2Int;
                    levelEditorViewModel.pacManTransform.position = position;
                    break;
                }
                case BrushMode.GhostHouse:
                {
                    levelEditorViewModel.currentWorkingLevel.ghostHousePosition = positionV2Int;
                    levelEditorViewModel.ghostHouseTransform.position = position;
                    break;
                }
                case BrushMode.GhostDoor:
                {
                    levelEditorViewModel.currentWorkingLevel.ghostDoorPosition = positionV2Int;
                    levelEditorViewModel.ghostDoor.position = position;
                    break;
                }
                case BrushMode.ScreenWrap:
                {
                    levelEditorViewModel.currentWorkingLevel.screenWrapPositions.Add(positionV2Int);
                    levelEditorViewModel.localObjects.Add(position, CreateGameObject(LevelObject.ScreenWrap, position));
                    break;
                }
                default:
                {
                    if (brush == BrushMode.Pellet)
                    {
                        levelEditorViewModel.currentWorkingLevel.pelletCount++;
                    }

                    LevelObject brushToLevelObject = LevelEditorViewModel.BrushToLevelObjectType(brush);
                    levelEditorViewModel.currentWorkingLevel.AddLevelObject(brushToLevelObject, positionV2Int);
                    levelEditorViewModel.localObjects.Add(position, CreateGameObject(brushToLevelObject, position));
                    break;
                }
            }
            
            
        }

        public GameObject AddLocalLevelObject(LevelObject levelObject, Vector3 position)
        {
            GameObject gameObject = null;
            switch (levelObject)
            {
                case LevelObject.Wall:
                    levelEditorViewModel.wallTilemap.SetTile(VectorUtil.ToVector3Int(position), levelEditorViewModel.wallRuleTile);
                    break;
                default:
                    gameObject = CreateGameObject(levelObject, position);
                    levelEditorViewModel.localObjects.Add(position, gameObject);
                    break;
            }
            return gameObject;
        }

        public GameObject CreateGameObject(LevelObject @object, Vector3 position)
        {
            return @object == LevelObject.Wall ? null :
                Object.Instantiate(LevelObjectTypeToPrefab(@object), position, levelEditorViewModel.identity);
        }

        private GameObject LevelObjectTypeToPrefab(LevelObject @object)
        {
            return @object switch
            {
                LevelObject.Blinky => levelEditorViewModel.blinkyMarkerPrefab,
                LevelObject.Pinky => levelEditorViewModel.pinkyMarkerPrefab,
                LevelObject.Inky => levelEditorViewModel.inkyMarkerPrefab,
                LevelObject.Clyde => levelEditorViewModel.clydeMarkerPrefab,
                LevelObject.Pellet => levelEditorViewModel.pelletMarkerPrefab,
                LevelObject.PowerPellet => levelEditorViewModel.powerMarkerPrefab,
                LevelObject.Wall => null,
                LevelObject.ScreenWrap => levelEditorViewModel.screenWrapMarkerPrefab,
                _ => throw new ArgumentOutOfRangeException(nameof(@object), @object, null)
            };
        }

        public void EraseObject(Vector3 position)
        {
            levelEditorViewModel.currentWorkingLevel.RemoveLevelObject(VectorUtil.ToVector2Int(position));
            Vector3 removeScreenWrapPair = levelEditorViewModel.currentWorkingLevel.RemoveScreenWrapPair(VectorUtil.ToVector2Int(position));
            DeleteObject(position);
            DeleteObject(removeScreenWrapPair);
            bool isWall = levelEditorViewModel.wallTilemap.GetTile(VectorUtil.ToVector3Int(position)) != null;
            if (isWall)
            {
                DeleteTile(levelEditorViewModel.mouseTilesetPosition);
            }
            
            else
            {
                DeleteObject(position);
            }
        }

        private void DeleteTile(Vector3Int position)
        {
            levelEditorViewModel.wallTilemap.SetTile(position, null);
        }

        private void DeleteObject(Vector3 position)
        {
            if (!levelEditorViewModel.localObjects.ContainsKey(position)) return;
            Object.Destroy(levelEditorViewModel.localObjects[position]);
            levelEditorViewModel.localObjects.Remove(position);
        }
    }
}