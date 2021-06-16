using System;
using System.Collections;
using System.Collections.Generic;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitMan
{
    public class GameOverController : MonoBehaviour
    {
        // Start is called before the first frame update
        private readonly Timer _returnTimer = new Timer(autoStart: true, oneTime: true);

        private void Awake() {
            _returnTimer.OnEnd += ReturnToMainScreen;
        }

        private static void ReturnToMainScreen() {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
