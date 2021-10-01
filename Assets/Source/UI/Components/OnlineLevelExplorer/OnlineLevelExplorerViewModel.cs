using System.Collections.Generic;
using System.Linq;
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
        private LevelCellViewModel[] levelCellViewModels = new LevelCellViewModel[7];

        [SerializeField]
        private GameObject[] levelCellViews = new GameObject[7];

        [SerializeField]
        private OneWayBinding<string> notificationBinding;
        
        private IEnumerable<Level> _levels;

        public void OnMainMenuPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.Home);
        }

        private void Start()
        {
            FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
            firestore.Collection("/levels").Listen(ListenCallback);
        }
        
        private void ListenCallback(QuerySnapshot dataSnapshot)
        {
            _levels = dataSnapshot.Documents.Select(LevelDocumentToLevelObject);
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
            Debug.Log(levelArray.Length);
            for (int i = 0; i < levelCellViews.Length; i++)
            {
                levelCellViews[i].SetActive(false);
                Debug.Log(i);
                if (i >= levelArray.Length) continue;
                levelCellViews[i].SetActive(true);
                levelCellViewModels[i].RenderWithLevelObject(levelArray[i]);
            }
        }

        public void NotifyByToast(string message)
        {
            notificationBinding.SetValue(message);
        }

        public void RefreshedPressed()
        {
            RenderLevels(_levels);
        }
        

        public override void OnRendered()
        {
            RenderLevels(_levels);
        }
    }
}
