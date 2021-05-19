using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.source
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : Actor
    {
        private PlayerInput _playerInput;
        private Vector2 _inputVector;
        private const float MOVE_SPEED = 3f;


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;
        }

        private void FixedUpdate()
        {
            base.rigidBody.velocity = GetMotion();
        }

        private Vector2 GetMotion()
        {
            return _inputVector * MOVE_SPEED;
        }

        // Update is called once per frame
        private void OnInputChanged(InputAction.CallbackContext context) {
            switch (context.action.name) {
                case "Move": {
                    _inputVector = context.ReadValue<Vector2>();
                    break;
                }
            }
        }
    }
}


