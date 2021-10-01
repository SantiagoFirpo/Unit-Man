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
            for (int i = 0; i < levelCellViews.Length; i++)
            {
                levelCellViews[i].SetActive(false);
                Debug.Log(i);
                if (i >= localLevels.Length) continue;
                levelCellViews[i].SetActive(true);
                levelCellViewModels[i].RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevels[i])));
            }
        }

        public void NotifyByToast(string message)
        {
            notificationBinding.SetValue(message);
        }

        public void RefreshedPressed()
        {
            string[] localPaths = Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories);
            RenderLevelsFromDisk(localPaths);
        }
        

        public override void OnRendered()
        {
            Debug.Log("Should render levels!");
            string[] localPaths = Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories);
            RenderLevelsFromDisk(localPaths);
        }
    }
}
