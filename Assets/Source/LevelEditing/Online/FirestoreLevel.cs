using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;
using UnitMan.Source.UI;
using UnitMan.Source.UI.Components.LevelEditor;
using UnityEngine;

namespace UnitMan.Source.LevelEditing.Online
{
    [FirestoreData]
    public class FirestoreLevel
    {
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public string AuthorName { get; set; }
        
        [FirestoreProperty]
        public string AuthorId { get; set; }
        
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public FirestoreVector2Int[] ObjectPositions { get; set; }
        
        [FirestoreProperty]
        public LevelObject[] ObjectTypes { get; set; }
        
        [FirestoreProperty]
        public int PelletCount { get; set; }

        [FirestoreProperty]
        public FirestoreVector2Int TopLeftPosition { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int TopRightPosition { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int BottomLeftPosition { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int BottomRightPosition { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int GhostDoorPosition { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int GhostHouse { get; set; }
        
        [FirestoreProperty]
        public FirestoreVector2Int PacManPosition { get; set; }

        [FirestoreProperty]
        public List<FirestoreVector2Int> WrapPositions { get; set; }
        
        public FirestoreLevel()
        {
        }

        public static FirestoreLevel FromLevel(Level level)
        {
            FirestoreLevel firestoreLevel = new FirestoreLevel
            {
                Id = level.id,
                Name = level.name,
                AuthorId = level.authorId,
                AuthorName = level.authorName,
                ObjectPositions = new FirestoreVector2Int[level.objectPositions.Count],
                PelletCount = level.pelletCount,
                TopLeftPosition = FirestoreVector2Int.FromVector2Int(level.topLeftPosition),
                TopRightPosition = FirestoreVector2Int.FromVector2Int(level.topRightPosition),
                BottomLeftPosition = FirestoreVector2Int.FromVector2Int(level.bottomLeftPosition),
                BottomRightPosition = FirestoreVector2Int.FromVector2Int(level.bottomRightPosition),
                GhostDoorPosition = FirestoreVector2Int.FromVector2Int(level.ghostDoorPosition),
                GhostHouse = FirestoreVector2Int.FromVector2Int(level.ghostHousePosition),
                PacManPosition = FirestoreVector2Int.FromVector2Int(level.pacManPosition),
            };
            firestoreLevel.WrapPositions = new List<FirestoreVector2Int>();
            foreach (Vector2Int screenWrapPosition in level.screenWrapPositions)
            {
                firestoreLevel.WrapPositions.Add(FirestoreVector2Int.FromVector2Int(screenWrapPosition));
            }
            firestoreLevel.ObjectTypes = new LevelObject[firestoreLevel.ObjectPositions.Length];
            for (int i = 0; i < firestoreLevel.ObjectPositions.Length; i++)
            {
                firestoreLevel.ObjectPositions[i] = FirestoreVector2Int.FromVector2Int(level.objectPositions[i]);
                firestoreLevel.ObjectTypes[i] = level.objectTypes[i];
            }

            return firestoreLevel;
        }
        
    }
}