using System;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan.Source
{
    public class NetworkChecker : MonoBehaviour
    {
        private Timer networkCheckTimer;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            networkCheckTimer = new Timer(180f, true, false);
            networkCheckTimer.OnEnd += NetworkCheckTimerOnOnEnd;
        }

        private void NetworkCheckTimerOnOnEnd()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable) return;
            SceneManager.LoadScene("Main Menu");
        }
    }
}
