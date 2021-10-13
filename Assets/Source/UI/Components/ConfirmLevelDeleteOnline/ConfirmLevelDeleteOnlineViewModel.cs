using Firebase.Firestore;
using UnitMan.Source.UI.Components.ConfirmLevelDeleteDialogueBox;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source.UI.Components.ConfirmLevelDeleteOnline
{
    public class ConfirmLevelDeleteOnlineViewModel : ConfirmLevelDeleteViewModel
    {
        public override void YesPressed()
        {
            Debug.Log("ConfirmLevelDeleteOnline");
            FirebaseFirestore.DefaultInstance.Document($"levels/{LevelIdToDeleteContainer.Instance.level.id}")
                .DeleteAsync();
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.OnlineLevelExplorer);
        }

        public override void NoPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.OnlineLevelExplorer);
        }
    }
}