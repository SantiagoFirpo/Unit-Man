﻿using System;
using System.Collections.Generic;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.UI.Components.LevelEditor;
using UnityEngine;

namespace UnitMan.Source.LevelEditing
{
    [Serializable]
    public class Level
    {
        public string name;
        public string authorName;
        public string authorId;
        public string id;
        public Vector2Int pacManPosition = Vector2Int.zero;
        public Vector2Int ghostHousePosition = new Vector2Int(0, 3);

        public Vector2Int ghostDoorPosition = new Vector2Int(0, 4);
        //TODO: change to Y-value and X-value lists
        public List<Vector2Int[]> screenWrapPositions = new List<Vector2Int[]>();
        public Vector2Int topRightPosition = Vector2Int.zero;
        public Vector2Int topLeftPosition = Vector2Int.zero;
        public Vector2Int bottomRightPosition = Vector2Int.zero;
        public Vector2Int bottomLeftPosition = Vector2Int.zero;
        public int pelletCount;

        public List<Vector2Int> objectPositions = new List<Vector2Int>();
        public List<LevelObjectType> objectTypes = new List<LevelObjectType>();


        public Level(string name, string authorName, string authorId)
        {
            this.name = name == "" ? LevelEditorViewModel.DEFAULT_LEVEL_NAME : name;
            this.authorName = authorName;
            this.authorId = authorId;
            id = LevelEditorViewModel.GetUniqueId();
        }

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

        public static Level FromFirestoreLevel(FirestoreLevel firestoreLevel)
        {
            Level level = new Level(firestoreLevel.Name, firestoreLevel.AuthorName, firestoreLevel.AuthorId)
                {
                    authorName = firestoreLevel.AuthorName,
                    pelletCount = firestoreLevel.PelletCount,
                    bottomLeftPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.BottomLeftPosition),
                    bottomRightPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.BottomRightPosition),
                    topLeftPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.TopLeftPosition),
                    topRightPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.TopRightPosition),
                    ghostDoorPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.GhostDoorPosition),
                    ghostHousePosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.GhostHouse),
                    pacManPosition = FirestoreVector2Int.ToVector2Int(firestoreLevel.PacManPosition),
                    
                };
            // Debug.Log($"is ObjectPositions null? {firestoreLevel.ObjectPositions == null}");
            if (firestoreLevel.ObjectPositions == null) return level;
            for (int i = 0; i < firestoreLevel.ObjectPositions.Length; i++)
            {
                level.objectPositions.Add(FirestoreVector2Int.ToVector2Int(firestoreLevel.ObjectPositions[i]));
                level.objectTypes.Add(firestoreLevel.ObjectTypes[i]);
            }


            return level;
        }
        
        public static Level FromJson(string levelJson)
        {
            return JsonUtility.FromJson<Level>(levelJson);
        }
    }
}