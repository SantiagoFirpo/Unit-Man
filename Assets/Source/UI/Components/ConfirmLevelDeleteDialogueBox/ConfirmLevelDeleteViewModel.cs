using System.IO;
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
            base.YesPressed();
            File.Delete($"{FilePaths.LevelsPath}/{LevelIdToDeleteContainer.Instance.levelId}.json");
            notificationBinding.SetValue($"LEVEL {LevelIdToDeleteContainer.Instance.levelName} DELETED SUCCESSFULLY");
        }

        public override void NoPressed()
        {
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.LocalLevelExplorer);
        }

        public override void OnRendered()
        {
            base.OnRendered();
            message.SetValue($"ARE YOU SURE YOU WANT TO DELETE THE LEVEL {LevelIdToDeleteContainer.Instance.levelName}? THERE IS NO WAY OF RECOVERING IT AFTER THE DELETION");
        }
    }
}