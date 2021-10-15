using System;
using System.Collections.Generic;
using UnitMan.Source.UI.Components.LevelEditor;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnitMan.Source.LevelEditing
{
    public class LevelLoadManager
    {
        private readonly LevelEditManager _levelEditManager;

        public LevelLoadManager(LevelEditManager levelEditManager)
        {
            _levelEditManager = levelEditManager;
        }

        public void PopulateEditorFromLevelObject(Level level)
        {
            ClearLevel();
            AddLocalObjects(level);
            AddScreenWrapPositions(level);
            SetUniqueObjectPositions(level);
        }

        private void AddScreenWrapPositions(Level level)
        {
            for (int index = 0; index < level.screenWrapPositions.Count; index += 2)
            {
                Vector2Int screenWrapPosition = level.screenWrapPositions[index];
                GameObject screenWrap = _levelEditManager.AddLocalLevelObject(LevelObjectType.ScreenWrap, VectorUtil.ToVector3(screenWrapPosition));
                if (index % 2 != 0) continue;
                WrapController wrapController = screenWrap.GetComponent<WrapController>();
                wrapController.SetDestination(
                    _levelEditManager.AddLocalLevelObject(LevelObjectType.ScreenWrap,
                        VectorUtil.ToVector3(level.screenWrapPositions[index + 1])).GetComponent<WrapController>());
                wrapController.SyncWithDestination();
            }
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
                _levelEditManager.AddLocalLevelObject(level.objectTypes[i], VectorUtil.ToVector3(positionV3Int));
                
            }
        }

        private void SetUniqueObjectPositions(Level level)
        {
            _levelEditManager.levelEditorViewModel.pacManTransform.position = VectorUtil.ToVector3(level.pacManPosition);
            _levelEditManager.levelEditorViewModel.ghostHouseTransform.position = VectorUtil.ToVector3(level.ghostHousePosition);
            _levelEditManager.levelEditorViewModel.ghostDoor.position = VectorUtil.ToVector3(level.ghostDoorPosition);
        }

        private void ClearLevel()
        {
            foreach (KeyValuePair<Vector3, GameObject> localObject in _levelEditManager.levelEditorViewModel.localObjects)
            {
                UnityObject.Destroy(localObject.Value);
            }

            _levelEditManager.levelEditorViewModel.localObjects.Clear();
            _levelEditManager.levelEditorViewModel.wallTilemap.ClearAllTiles();
        }

        public void ComputeScatterTargets()
        {
            BoundsInt cellBounds = _levelEditManager.levelEditorViewModel.wallTilemap.cellBounds;
            _levelEditManager.levelEditorViewModel.currentWorkingLevel.topLeftPosition =
                new Vector2Int(cellBounds.xMin, cellBounds.yMax) + new Vector2Int(-1, 1);
            Vector2Int vectorOne = Vector2Int.one;
            _levelEditManager.levelEditorViewModel.currentWorkingLevel.topRightPosition = VectorUtil.ToVector2Int(cellBounds.max) + vectorOne;
            _levelEditManager.levelEditorViewModel.currentWorkingLevel.bottomLeftPosition = VectorUtil.ToVector2Int(cellBounds.min) - vectorOne;
            _levelEditManager.levelEditorViewModel.currentWorkingLevel.bottomRightPosition = new Vector2Int(cellBounds.xMax, cellBounds.yMin) + new Vector2Int(1, -1);
        }
    }
}