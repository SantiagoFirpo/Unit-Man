using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.LevelEditor;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source.UI.Components.LevelCell
{
    public class LevelCellViewModel : ViewModel
    {
        [SerializeField]
        private Level level;
        [SerializeField]
        private Reactive<string> levelName = new Reactive<string>();

        [SerializeField]
        private Reactive<string> authorName = new Reactive<string>();

        [SerializeField]
        private Reactive<string> levelId = new Reactive<string>();

        [SerializeField]
        private Reactive<string> notificationBinding;

        [SerializeField]
        private ReactiveEvent levelIsAuthoredByUserEvent;


        public void RenderWithLevelObject(Level levelObject)
        {
            // thisTransform.parent.gameObject.SetActive(true);
            level = levelObject;
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
            CrossSceneLevelContainer.Instance.level = level;
        }

        public void OnEditButtonPressed()
        {
            SetLoadedLevelToContainer();
            SceneManager.LoadScene("Level Editor");
        }

        public void OnUploadButtonPressed()
        {
            LevelEditorViewModel.UploadLevelToFirestore(level);
            notificationBinding.SetValue($"LEVEL {level.name} UPLOADED SUCCESSFULLY");
        }

        public void LeaderboardPressed()
        {
            SetLoadedLevelToContainer();
            SceneManager.LoadScene("Scoreboard");
        }

        public void DeletePressed()
        {
            LevelIdToDeleteContainer.Instance.levelId = levelId.GetValue();
            LevelIdToDeleteContainer.Instance.levelName = levelName.GetValue();
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.ConfirmLevelDelete);
        }

        public override void OnRendered()
        {
            Debug.Log(level.authorId == FirebaseAuthManager.Instance.User.UserId);
            if (level.authorId == FirebaseAuthManager.Instance.User.UserId)
            {
                levelIsAuthoredByUserEvent.Call();
            }
        }
    }
}
