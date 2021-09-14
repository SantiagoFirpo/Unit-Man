using UnitMan.Source.Utilities.ObserverSystem;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class ViewModel<TState>
    {
        private TState _state;
        public readonly Emitter<TState> emitter = new Emitter<TState>();
        public readonly Observer<TState> observer;

        protected ViewModel()
        {
            this.observer = new Observer<TState>(OnStateChangeByUser);
        }

        protected abstract void OnStateChangeByUser(Emitter<TState> source, TState newState);

        public void SetState(TState targetState)
        {
            this._state = targetState;
            emitter.EmitNotification(_state);
        }
    }
}