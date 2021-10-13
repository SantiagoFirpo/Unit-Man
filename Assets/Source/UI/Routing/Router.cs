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
        protected Reactive<TPageType> state = new Reactive<TPageType>();

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
            Route<TPageType> targetRoute = null;
            foreach (Route<TPageType> route in viewsToRender)
            {
                if (!route.GetValue().Equals(pageValue)) continue;
                targetRoute = route;
                route.Render();
            }

            if ((targetRoute is { hideOtherRoutes: false })) return;
            {
                foreach (Route<TPageType> route in viewsToRender)
                {
                    if (route != targetRoute) route.Hide();
                }
            }
        }
    }
}
