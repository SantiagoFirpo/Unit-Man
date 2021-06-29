using System;
using System.Collections;
using System.Collections.Generic;
using UnitMan.Source;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        
        private static void OnPressStart() {
            SceneManager.LoadScene("Gameplay");
        }

        private void Start() {
            startButton.onClick.AddListener(OnPressStart);
        }
    }
}
