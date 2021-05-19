using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnitMan.Source.Utilities;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : Actor
    {
        [SerializeField]
        private int _currentState;
        [SerializeField]
        private int _previousState;        
        private const float MOVE_SPEED = 3f;
        public Vector2 motion;
        
        private StateMachine _stateMachine;
        private ITransitionProvider _transitionProvider;
        
        private PlayerInput _playerInput;
        private Vector2 _inputVector;
        

        public enum PlayerState
        {
            Idle, Move
        }


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            _transitionProvider = new PlayerTransitionProvider(this);
            _stateMachine = new StateMachine(
                _transitionProvider,
                new int[] {
                    (int) PlayerState.Idle,
                    (int) PlayerState.Move
                });
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;
        }

        private void Update()
        {
            _stateMachine.PollState();
            _currentState = _stateMachine.currentState;
            _previousState = _stateMachine.previousState;
            // Debug.Log(_stateMachine.currentState); // FOR STATE DEBUGGING
            // Debug.Log(_stateMachine.previousState); // FOR STATE DEBUGGING
        }

        

        private void FixedUpdate() {
            motion = GetMotion();
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


