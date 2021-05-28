using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan
{
    public class Gamemanager : MonoBehaviour
    {
        public static Gamemanager Instance { get; private set; }

        public int score = 0;
        // Start is called before the first frame update
        private void Awake() {
            if (Instance != null) {GameObject.Destroy(gameObject);}
            Instance = this;
        }
    }
}
