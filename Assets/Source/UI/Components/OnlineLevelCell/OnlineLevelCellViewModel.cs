using UnitMan.Source.Management.Firebase.Auth;
using UnitMan.Source.UI.Components.LevelCell;
using UnitMan.Source.UI.Routing.Routers;
using UnitMan.Source.Utilities;

namespace UnitMan.Source.UI.Components.OnlineLevelCell
{
    public class OnlineLevelCellViewModel : LevelCellViewModel
    {
        public override void DeletePressed()
        {
            if (level.authorId != FirebaseAuthManager.Instance.User.UserId) return;
            LevelIdToDeleteContainer.Instance.level = level;
            MainMenuRouter.Instance.SetState(MainMenuRouter.MainMenuRoute.ConfirmOnlineLevelDelete);
        }
    }
}
