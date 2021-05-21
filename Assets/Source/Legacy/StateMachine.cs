using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnitMan.Source.Utilities {
    public sealed class StateMachine
    {
        public delegate void StateUpdate(int newState, int oldState);
        public List<int> states;
        public int currentState;
        public int previousState;

        private readonly ITransitionProvider _transitionProvider;

        public StateMachine (ITransitionProvider transitionProvider, int[] states)
        {
            this.states = states.ToList();
            _transitionProvider = transitionProvider;
        }
        public void PollState()
        {
            if (this.currentState == 0) return;
            // StateLogic();
            int transition = _transitionProvider.GetTransition(currentState);
            if (transition == -1 || transition == previousState) return;
            // Debug.Log(transition);
            SetState(transition);
        }

        private void OnStateEnter(int newState, int oldState) {}

        private void AddState(int state) => states.Add(state);

        private void SetState(int newState)
        {
            previousState = currentState;
            currentState = newState;
            if (previousState == 0) return;
            if (newState == 0 ) return;
            OnStateEnter(newState, previousState);
        }
    }
}