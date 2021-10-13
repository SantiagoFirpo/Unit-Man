using System;
using System.Collections.Generic;
using System.Threading;
using UnitMan.Source.Management.Firebase.Auth;
using UnityEngine;

namespace UnitMan.Source.UI.Routing.Routers
{
    public class MainMenuRouter : Router<MainMenuRouter.MainMenuRoute>
    {
        private readonly Queue<Action> _threadSafeActionQueue = new Queue<Action>();
        public static MainMenuRouter Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (FirebaseAuthManager.Instance.User is null) return;
            SetState(MainMenuRoute.Home);
        }

        private void Update()
        {
            if (_threadSafeActionQueue.Count != 0)
            {
                _threadSafeActionQueue?.Dequeue().Invoke();
            }
        }

        public enum MainMenuRoute
        {
            Undefined, Auth, Home, LocalLevelExplorer, OnlineLevelExplorer, ConfirmLocalLevelDelete, ConfirmOnlineLevelDelete
        }
        

        public void OnUserLoggedIn()
        {
            Debug.Log("a");
            Debug.Log("b");
            lock (_threadSafeActionQueue)
            {
                _threadSafeActionQueue.Enqueue(UpdateLogin);
            }

        }

        private void UpdateLogin()
        {
            Debug.Log("c");
            if (state.GetValue() == MainMenuRoute.Home) return;
            Debug.Log(Thread.CurrentThread.Name);
            SetState(MainMenuRoute.Home);
        }

        public void OnUserSignedOut()
        {
            if (state.GetValue() != MainMenuRoute.Auth) SetState(MainMenuRoute.Auth);
        }
    }
}