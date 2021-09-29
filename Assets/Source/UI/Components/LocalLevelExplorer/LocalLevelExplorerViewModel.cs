using System.IO;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.UI.Components.LevelCell;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source.UI.Components.LocalLevelExplorer
{
    public class LocalLevelExplorerViewModel : ViewModel
    {
        [SerializeField]
        private LevelCellViewModel[] levelCells = new LevelCellViewModel[7];
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
    }
}
