using Firebase.Firestore;
using UnityEngine;

namespace UnitMan.Source.LevelEditing.Online
{
    [FirestoreData]
    public struct FirestoreVector2Int
    {
        [FirestoreProperty]
        public int X { get; set; }
        
        [FirestoreProperty]
        public int Y { get; set; }

        public static FirestoreVector2Int FromVector2Int(Vector2Int vector2Int)
        {
            FirestoreVector2Int firestoreVector2Int = new FirestoreVector2Int
            {
                X = vector2Int.x,
                Y = vector2Int.y
            };
            return firestoreVector2Int;
        }

        public static Vector2Int ToVector2Int(FirestoreVector2Int firestoreVector2Int)
        {
            return new Vector2Int(firestoreVector2Int.X, firestoreVector2Int.Y);
        }
        
        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(this.X, this.Y);
        }
    }
}