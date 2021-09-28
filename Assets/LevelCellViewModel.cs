using UnitMan.Source.LevelEditing;
using UnitMan.Source.UI.MVVM;
using UnityEngine;

namespace UnitMan
{
    public class LevelCellViewModel : View
    {
        [SerializeField]
        private OneWayBinding<string> levelName = new OneWayBinding<string>();

        [SerializeField]
        private OneWayBinding<string> authorName = new OneWayBinding<string>();

        [SerializeField]
        private OneWayBinding<string> levelId = new OneWayBinding<string>();

        public void RenderWithLevelObject(Level levelObject)
        {
            levelName.SetValue(levelObject.name);
            authorName.SetValue(levelObject.authorName);
            levelId.SetValue(levelObject.id);
        }
    }
}
