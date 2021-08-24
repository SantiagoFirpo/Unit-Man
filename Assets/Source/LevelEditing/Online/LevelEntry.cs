using System;

namespace UnitMan.Source.LevelEditing.Online
{
    [Serializable]
    public class LevelEntry
    {
        public Level level;

        public string levelId;
        // public LevelMeta levelMeta;
        public LevelEntry(Level level, string levelId)
        {
            this.level = level;
            this.levelId = levelId;
        }
    }
}