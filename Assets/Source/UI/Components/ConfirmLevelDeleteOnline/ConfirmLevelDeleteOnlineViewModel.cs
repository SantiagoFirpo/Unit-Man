using System;
using System.IO;
using Firebase.Firestore;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.ConfirmLevelDeleteDialogueBox;
using UnitMan.Source.UI.Components.YesNoDialogueBox;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source.UI.Components.ConfirmLevelDeleteOnline
{
    public class ConfirmLevelDeleteOnlineViewModel : ConfirmLevelDeleteViewModel
    {
        public override void YesPressed()
        {
            FirebaseFirestore.DefaultInstance.Document($"levels/{LevelIdToDeleteContainer.Instance.level.id}")
                .DeleteAsync();
        }

        public override void NoPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.OnlineLevelExplorer);
        }
    }
}