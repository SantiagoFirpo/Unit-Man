namespace UnitMan.Source
{
    public interface ITransitionProvider {
        abstract int GetTransition(int currentState);
    }
}