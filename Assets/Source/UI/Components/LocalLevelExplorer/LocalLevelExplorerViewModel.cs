using System.IO;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.UI.Components.LevelCell;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LocalLevelExplorerViewModel : ViewModel
    {
        [SerializeField]
        private LevelCellViewModel[] levelCellViewModels = new LevelCellViewModel[7];

        [SerializeField]
        private GameObject[] levelCellViews = new GameObject[7];

        [SerializeField]
        private OneWayBinding<string> notificationBinding;

        public void OnMainMenuPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }

        // private void Start()
        // {
        //     string[] localLevels = Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories);
        //     RenderLevelsFromDisk(localLevels);
        // }

        private void RenderLevelsFromDisk(string[] localLevels)
        {
            Debug.Log(localLevels.Length);
            for (int i = 0; i < localLevels.Length; i++)
            {
                Debug.Log(i);
                if (i == levelCellViewModels.Length) return;
                levelCellViews[i].SetActive(true);
                levelCellViewModels[i].RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevels[i])));
            }
        }

        public void NotifyLevelUpload(string levelName)
        {
            notificationBinding.SetValue($"LEVEL {levelName} WAS UPLOADED");
        }

        public void NotifyLevelDeletion(string levelName)
        {
            notificationBinding.SetValue($"LEVEL {levelName} WAS DELETED");
        }
        

        public override void OnRendered()
        {
            Debug.Log("Should render levels!");
            RenderLevelsFromDisk(Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories));
        }
    }
}
