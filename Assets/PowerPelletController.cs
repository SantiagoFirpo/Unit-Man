using System.Collections;
using System.Collections.Generic;
using UnitMan.Source;
using UnityEngine;

namespace UnitMan
{
    public class PowerPelletController : PelletController
    {
        protected override void Awake() {
            base.Awake();
            base.scoreValue = 100;
        }


        protected override void UpdatePlayerState() {
            PlayerController.Instance.SetInvincible();
        }
    }
}
