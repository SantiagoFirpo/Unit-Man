using Firebase.Firestore;

namespace UnitMan.Source.LevelEditing.Online
{
    [FirestoreData]
    public class LevelEntryFirestore
    {
        [FirestoreProperty]
        public LevelEntry LevelEntry { get; set; }
    }
}