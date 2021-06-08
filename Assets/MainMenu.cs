using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnitMan
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        // Start is called before the first frame update
        private static void OnPressStart() {
            SceneManager.LoadScene("Gameplay");
        }

        private void Start() {
            startButton.onClick.AddListener(OnPressStart);
        }
    }
}
