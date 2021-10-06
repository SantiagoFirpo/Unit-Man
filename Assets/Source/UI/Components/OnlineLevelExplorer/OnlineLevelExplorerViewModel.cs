using System.Collections.Generic;
using System.Linq;
using Firebase.Extensions;
using Firebase.Firestore;
using UnitMan.Source.LevelEditing;
using UnitMan.Source.LevelEditing.Online;
using UnitMan.Source.UI.Components.LevelCell;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnityEngine;

namespace UnitMan.Source.UI.Components.OnlineLevelExplorer
{
    public class OnlineLevelExplorerViewModel : ViewModel
    {
        [SerializeField]
        private ReactiveProperty<string> notificationBinding;
        
        private IEnumerable<Level> _levels;
        [SerializeField]
        private GameObject levelCell;

        private readonly List<GameObject> _activeLevelCells = new List<GameObject>();

        [SerializeField]
        private Transform contentTransform;

        public void OnMainMenuPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }

        private void ListenCallback(QuerySnapshot dataSnapshot)
        {
            _levels = dataSnapshot.Documents.Select(LevelDocumentToLevelObject);
            Debug.Log(_levels);
        }

        public static Level LevelDocumentToLevelObject(DocumentSnapshot documentSnapshot)
        {
            FirestoreLevel firestoreLevel = documentSnapshot.ConvertTo<FirestoreLevel>();
            Level levelBeforeJson = Level.FromFirestoreLevel(firestoreLevel);
            string levelJson = JsonUtility.ToJson(levelBeforeJson);
            return JsonUtility.FromJson<Level>(levelJson);
        }

        // private void Start()
        // {
        //     string[] localLevels = Directory.GetFiles(FilePaths.LevelsPath, "*.json", SearchOption.AllDirectories);
        //     RenderLevelsFromDisk(localLevels);
        // }

        private void RenderLevels(IEnumerable<Level> newLevels)
        {
            Level[] levelArray = newLevels.ToArray();

            foreach (Level level in levelArray)
            {
                GameObject levelCellGameObject = Instantiate(levelCell, Vector3.zero, Quaternion.identity, contentTransform);
                levelCellGameObject.SetActive(true);
                levelCellGameObject.GetComponentInChildren<LevelCellViewModel>().RenderWithLevelObject(level);
                _activeLevelCells.Add(levelCellGameObject);
                // levelCellViews[i].SetActive(false);
                // Debug.Log(i);
                // if (i >= localLevels.Length) continue;
                // levelCellViews[i].SetActive(true);
                // levelCellViewModels[i].RenderWithLevelObject(Level.FromJson(File.ReadAllText(localLevels[i])));
            }
            Debug.Log(levelArray.Length);
            // for (int i = 0; i < levelCellViews.Length; i++)
            // {
            //     levelCellViews[i].SetActive(false);
            //     Debug.Log(i);
            //     if (i >= levelArray.Length) continue;
            //     levelCellViews[i].SetActive(true);
            //     levelCellViewModels[i].RenderWithLevelObject(levelArray[i]);
            // }
        }

        public void NotifyByToast(string message)
        {
            notificationBinding.SetValue(message);
        }

        public void Refresh()
        {
            DestroyAllCells();
            FirebaseFirestore.DefaultInstance.Collection("/levels").GetSnapshotAsync().ContinueWithOnMainThread(
                task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.Log(task.Exception.Message);
                    }
                    
                    RenderLevels(task.Result.Select(LevelDocumentToLevelObject));
                });
        }

        private void DestroyAllCells()
        {
            foreach (GameObject activeLevelCell in _activeLevelCells)
            {
                Destroy(activeLevelCell);
            }
        }


        public override void OnRendered()
        {
            Debug.Log("Online Level Explorer Rendered");
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            // firestore.Collection("/levels").Listen(ListenCallback);
            Refresh();
        }
    }
}
