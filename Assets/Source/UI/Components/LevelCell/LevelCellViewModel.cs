using System.IO;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.UI.Components.LevelEditor;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI.Components.LevelCell
{
    public class LevelCellViewModel : ViewModel
    {
        private Level _level;
        [SerializeField]
        private OneWayBinding<string> levelName = new OneWayBinding<string>();

        [SerializeField]
        private OneWayBinding<string> authorName = new OneWayBinding<string>();

        [SerializeField]
        private OneWayBinding<string> levelId = new OneWayBinding<string>();

        public void RenderWithLevelObject(Level levelObject)
        {
            _level = levelObject;
            levelName.SetValue(levelObject.name);
            authorName.SetValue(levelObject.authorName);
            levelId.SetValue(levelObject.id);
        }

        public void OnPlayButtonPressed()
        {
            SetLoadedLevelToContainer();
            SceneManager.LoadScene("Gameplay");
        }

        private void SetLoadedLevelToContainer()
        {
            CrossSceneLevelContainer.Instance.level = _level;
        }

        public void OnEditButtonPressed()
        {
            SetLoadedLevelToContainer();
            SceneManager.LoadScene("Level Editor");
        }

        public void OnUploadButtonPressed()
        {
            LevelEditorViewModel.UploadLevelToFirestore(_level);
        }

        public void LeaderboardPressed()
        {
            SetLoadedLevelToContainer();
            SceneManager.LoadScene("Scoreboard");
        }

        public void DeletePressed()
        {
            File.Delete(@$"{FilePaths.LevelsPath}/{levelId.GetValue()}");
        }
    }
}
