using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnitMan.Source.UI.Components.Text;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;

namespace UnitMan
{
    public class ToastViewModel : MonoBehaviour
    {
        private Timer _showTimer;
        [SerializeField]
        private TextViewModel text;

        private void Start()
        {
            _showTimer = new Timer(3f, false, true);
            _showTimer.OnEnd += ShowTimerOnOnEnd;
        }

        private void ShowTimerOnOnEnd()
        {
            text.Set("");
        }

        public void Notify(string message)
        {
            text.Set(message);
            _showTimer.Start();
        }
    }
}
