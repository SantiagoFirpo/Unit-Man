using System;

namespace UnitMan.Source.Entities.Actors.Ghosts
{
    public interface IStateMachine<T> //WHERE T IS ENUM
    {
        // public T State { get; set; }
        // public T PreviousState { get; set; }
        public void OnStateEntered();
        public void SetState(T newState);
        public void OnStateExit();

        public T GetState();
    }
}