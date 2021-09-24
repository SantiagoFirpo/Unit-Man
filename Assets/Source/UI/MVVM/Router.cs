using System;
using UnitMan.Source.Entities.Actors.Ghosts;
using UnitMan.Source.UI.Routers;
using UnityEngine;

namespace UnitMan.Source.UI.MVVM
{
    public abstract class Router<TPageType> : MonoBehaviour, IStateMachine<TPageType> where TPageType : Enum
    {
        protected TPageType state;

        [SerializeField]
        protected Route<TPageType>[] viewsToRender;
        
        [SerializeField]
        protected TPageType initialState;


        private void Start()
        {
            RenderWithValue(initialState);
        }

        public abstract void OnStateEntered();

        public void SetState(TPageType newState)
        {
            OnStateExit();
            this.state = newState;
            OnStateEntered();
        }

        public void OnStateExit()
        {
            throw new NotImplementedException();
        }

        public TPageType GetState()
        {
            return state;
        }
        
        protected void RenderWithValue(TPageType pageValue)
        {
            foreach (Route<TPageType> route in viewsToRender)
            {
                if (route.GetValue().Equals(state)) return;
                if (route.GetValue().Equals(pageValue))
                {
                    route.Render();
                    continue;
                }
                route.Hide();
                
            }
        }
    }
}
