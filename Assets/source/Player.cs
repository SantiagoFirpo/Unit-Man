using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnitMan.Source.Utilities;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : Actor
    {
        private PlayerInput _playerInput;
        private Vector2 _inputVector;
        private const float MOVE_SPEED = 3f;
        private FiniteStateMachine _finiteStateMachine;
        private Vector2 _motion;

        public enum PlayerState
        {
            Idle, Move
        }


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            _finiteStateMachine = new FiniteStateMachine(PollTransition, new int[] {(int) PlayerState.Idle, (int) PlayerState.Move});
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;
        }

        private void Update()
        {
            PollTransition(_finiteStateMachine.currentState);
            Debug.Log(_finiteStateMachine.currentState); // FOR STATE DEBUGGING
            Debug.Log(_finiteStateMachine.previousState); // FOR STATE DEBUGGING
        }

        private int PollTransition(int currentState) {
            const int nullTransition = -1;
            switch (_finiteStateMachine.currentState) {
                case (int) PlayerState.Idle: {
                    if (_motion != Vector2.zero) {
                        return (int) PlayerState.Move;
                    }
                    
                    return nullTransition;

                }
                case (int) PlayerState.Move: {
                    if (_motion == Vector2.zero) {
                        return (int) PlayerState.Idle;
                    }

                    return nullTransition;
                }
                default: {
                    return nullTransition;
                }
            };
        }

        private void FixedUpdate() {
            _motion = GetMotion();
            base.rigidBody.velocity = GetMotion();
        }

        private Vector2 GetMotion() {
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


