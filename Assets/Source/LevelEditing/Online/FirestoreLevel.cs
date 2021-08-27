using Firebase.Firestore;
using UnitMan.Source.UI;

namespace UnitMan.Source.LevelEditing.Online
{
    [FirestoreData]
    public class FirestoreLevel
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public FirestoreVector2Int[] ObjectPositions { get; set; }
        
        [FirestoreProperty]
        public LevelObjectType[] ObjectTypes { get; set; }
        
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
        
        public FirestoreLevel()
        {
        }

        public static FirestoreLevel FromLevel(Level level)
        {
            FirestoreLevel firestoreLevel = new FirestoreLevel
            {
                Id = level.id,
                ObjectPositions = new FirestoreVector2Int[level.objectPositions.Count],
                PelletCount = level.pelletCount,
                TopLeftPosition = FirestoreVector2Int.FromVector2Int(level.topLeftPosition),
                TopRightPosition = FirestoreVector2Int.FromVector2Int(level.topRightPosition),
                BottomLeftPosition = FirestoreVector2Int.FromVector2Int(level.bottomLeftPosition),
                BottomRightPosition = FirestoreVector2Int.FromVector2Int(level.bottomRightPosition),
                GhostDoorPosition = FirestoreVector2Int.FromVector2Int(level.ghostDoorPosition),
                GhostHouse = FirestoreVector2Int.FromVector2Int(level.ghostHousePosition),
                PacManPosition = FirestoreVector2Int.FromVector2Int(level.pacManPosition)
            };
            firestoreLevel.ObjectTypes = new LevelObjectType[firestoreLevel.ObjectPositions.Length];
            for (int i = 0; i < firestoreLevel.ObjectPositions.Length; i++)
            {
                firestoreLevel.ObjectPositions[i] = FirestoreVector2Int.FromVector2Int(level.objectPositions[i]);
                firestoreLevel.ObjectTypes[i] = level.objectTypes[i];
            }

            return firestoreLevel;
        }
        
    }
}