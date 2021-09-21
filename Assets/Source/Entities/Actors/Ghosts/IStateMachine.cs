using System;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public interface IStateMachine<T> where T : Enum
    {
        // public T State { get; set; }
        // public T PreviousState { get; set; }
        public void OnStateEntered();
        public void SetState(T state);
        public void OnStateExit();

        public T GetState();
    }
}