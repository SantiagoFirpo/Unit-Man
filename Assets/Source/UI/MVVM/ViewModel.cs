using UnitMan.Source.Utilities.ObserverSystem;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class ViewModel<TState> where TState : struct
    {
        private TState _state;
        public readonly Emitter<TState> emitter = new Emitter<TState>();
        public readonly Observer<TState> observer;

        protected ViewModel()
        {
            this.observer = new Observer<TState>(OnStateChangeFromView);
        }

        protected abstract void OnStateChangeFromView(TState newState);

        public void SetState(TState targetState)
        {
            this._state = targetState;
            emitter.EmitNotification(_state);
        }

        public TState GetState()
        {
            return _state;
        }
    }
}