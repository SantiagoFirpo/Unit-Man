using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnitMan.Source.Utilities {
    public sealed class FiniteStateMachine
    {
        public delegate int TransitionPoll(int currentState);
        public delegate void StateUpdate(int newState, int oldState);
        public List<int> states;
        public int currentState;
        public int previousState;

        public TransitionPoll getTransition;

        public FiniteStateMachine (TransitionPoll transitionPollMethod, int[] states)
        {
            this.states = states.ToList();
            this.
        }
        void PollState()
        {
            if (currentState == 0) return;
            // StateLogic();
            int transition = getTransition(currentState);
            if (transition == -1 || transition == previousState) return;
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