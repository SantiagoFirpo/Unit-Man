using System;
using System.IO;
using Firebase.Firestore;
using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.YesNoDialogueBox;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source.UI.Components.ConfirmLevelDeleteDialogueBox
{
    public class ConfirmLevelDeleteViewModel : YesNoDialogueBoxViewModel
    {
        [SerializeField]
        private Reactive<string> notificationBinding;

        public override void YesPressed()
        {
            if (LevelIdToDeleteContainer.Instance.level.authorId != FirebaseAuthManager.Instance.User.UserId)
            {
                try
                {
                    File.Delete($"{FilePaths.LevelsPath}/{LevelIdToDeleteContainer.Instance.level.id}.json");

                }
                catch (Exception e)
                {
                    
                    Debug.LogException(e);
                    throw;
                }
                
            }
            notificationBinding.SetValue($"LEVEL {LevelIdToDeleteContainer.Instance.level.name} DELETED SUCCESSFULLY");
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.LocalLevelExplorer);
        }

        public override void NoPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.LocalLevelExplorer);
        }

        public override void OnRendered()
        {
            base.OnRendered();
            message.SetValue($"ARE YOU SURE YOU WANT TO DELETE THE LEVEL {LevelIdToDeleteContainer.Instance.level.name}? THERE IS NO WAY OF RECOVERING IT AFTER THE DELETION");
        }
    }
}