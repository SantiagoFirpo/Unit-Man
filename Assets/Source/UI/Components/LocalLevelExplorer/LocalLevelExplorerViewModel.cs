using System.Collections.Generic;
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
        private ReactiveProperty<string> notificationBinding;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private GameObject levelCell;

        private List<GameObject> _activeLevelCells = new List<GameObject>();

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
            DestroyAllCells(); //TODO: recycle level cells
            Debug.Log(localLevels.Length);
            foreach (string localLevelPath in localLevels)
            {
                GameObject levelCellGameObject = Instantiate(levelCell, Vector3.zero, Quaternion.identity, contentTransform);
                levelCellGameObject.SetActive(true);
                levelCellGameObject.GetComponentInChildren<LevelCellViewModel>().RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevelPath)));
                _activeLevelCells.Add(levelCellGameObject);
                // levelCellViews[i].SetActive(false);
                // Debug.Log(i);
                // if (i >= localLevels.Length) continue;
                // levelCellViews[i].SetActive(true);
                // levelCellViewModels[i].RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevels[i])));
            }
        }
        
        private void DestroyAllCells()
        {
            foreach (GameObject activeLevelCell in _activeLevelCells)
            {
                Destroy(activeLevelCell);
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
