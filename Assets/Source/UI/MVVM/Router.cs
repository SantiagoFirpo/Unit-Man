using System;
using UnitMan.Source.Entities.Actors.Ghosts;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class Router<TPageType> : MonoBehaviour, IStateMachine<TPageType> where TPageType : Enum
    {
        private TPageType _state;

        public void OnStateEntered()
        {
            throw new NotImplementedException();
        }

        public void SetState(TPageType state)
        {
           
        }

        public void OnStateExit()
        {
            throw new NotImplementedException();
        }

        public TPageType GetState()
        {
            return _state;
        }
    }
}
