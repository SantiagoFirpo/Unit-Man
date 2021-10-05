using System;
using UnitMan.Source.Entities.Actors.Ghosts;
using UnitMan.Source.UI.MVVM;
using UnitMan.Source.UI.Routing.Routers;
using UnityEngine;

namespace UnitMan.Source.UI.Routing
{
    public abstract class Router<TPageType> : MonoBehaviour, IStateMachine<TPageType> where TPageType : struct, Enum
    {
        [SerializeField]
        protected ReactiveProperty<TPageType> state = new ReactiveProperty<TPageType>();

        [SerializeField]
        protected Route<TPageType>[] viewsToRender;
        
        [SerializeField]
        protected TPageType initialState;


        protected virtual void Start()
        {
            RenderWithValue(initialState);
        }

        public virtual void OnStateEntered(TPageType newState)
        {
            RenderWithValue(newState);
        }

        public void SetState(TPageType newState)
        {
            OnStateExit();
            state.SetValue(newState);
            OnStateEntered(newState);
        }

        public void OnStateExit()
        {
            // throw new NotImplementedException();
        }

        public TPageType GetState()
        {
            return state.GetValue();
        }
        
        protected void RenderWithValue(TPageType pageValue)
        {
            bool foundNewRoute = false;
            foreach (Route<TPageType> route in viewsToRender)
            {
                // if (route.GetValue().Equals(state)) return;
                if (!foundNewRoute && route.GetValue().Equals(pageValue))
                {
                    route.Render();
                    foundNewRoute = true;
                    continue;
                }
                route.Hide();
                
            }
        }
    }
}
