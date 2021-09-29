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
        private LevelCellViewModel[] levelCells = new LevelCellViewModel[7];

        [SerializeField]
        private OneWayBinding<string> notificationBinding;

        public void OnMainMenuPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }

        private void Start()
        {
            string[] localLevels = Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories);
            for (int i = 0; i < levelCells.Length; i++)
            {
                Debug.Log(i);
                if (i == localLevels.Length) return;
                levelCells[i].RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevels[i])));
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
    }
}
