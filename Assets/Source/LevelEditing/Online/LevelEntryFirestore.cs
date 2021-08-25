using Firebase.Firestore;

namespace UnitMan.Source.LevelEditing.Online
{
    [FirestoreData]
    public class LevelEntryFirestore
    {
        public LevelEntryFirestore()
        {
        }

        [FirestoreProperty]
        public Level Level {get; set; }
    }
}