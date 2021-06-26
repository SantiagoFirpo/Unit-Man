using System.Collections;
using System.Collections.Generic;
using UnitMan.Source;
using UnitMan.Source.Management;
using UnityEngine;

namespace UnitMan
{
    public class PowerPelletController : PelletController
    {
        private PlayerController _playerController;
        protected override void Awake() {
            base.Awake();
            scoreValue = 50;
            _playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        }


        protected override void UpdatePlayerState() { 
            _playerController.SetInvincible();
        }
    }
}
