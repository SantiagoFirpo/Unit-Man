using System;
using System.Collections;
using System.Collections.Generic;
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
        private const float WALL_CHECK_DISTANCE = 0.6f;

        private StateMachine _stateMachine;
        private ITransitionProvider _transitionProvider;
        
        private PlayerInput _playerInput;
        private Vector2 _inputVector;
        private Vector2 _currentDirection;
        private LayerMask _wallLayer;
        
        [SerializeField]
        private Vector2[] _wallChecks;


        public enum PlayerState
        {
            Idle,
            Move
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
            _wallLayer = LayerMask.GetMask("Wall");
        }

        private void Update()
        {
            // _stateMachine.PollState();
            // _currentState = _stateMachine.currentState;
            // _previousState = _stateMachine.previousState;
            // Debug.Log(_stateMachine.currentState); // FOR STATE DEBUGGING
            // Debug.Log(_stateMachine.previousState); // FOR STATE DEBUGGING
        }

        

        private void FixedUpdate()
        {
            _wallChecks = CheckPossibleTurns();
            _currentDirection = GetMoveDirection();
            motion = (Vector2) _currentDirection * MOVE_SPEED;
            
            if (((IList) _wallChecks).Contains(_currentDirection))
            {
                rigidBody.velocity = motion;
            }
            
        }

        private Vector2[] CheckPossibleTurns()
        {
            Vector2 playerPosition = base.thisTransform.position;
            var upHit = Physics2D.Raycast(playerPosition, Vector2.up, WALL_CHECK_DISTANCE, _wallLayer);
            var downHit = Physics2D.Raycast(playerPosition, Vector2.down, WALL_CHECK_DISTANCE, _wallLayer);
            var leftHit = Physics2D.Raycast(playerPosition, Vector2.left, WALL_CHECK_DISTANCE, _wallLayer);
            var rightHit = Physics2D.Raycast(playerPosition, Vector2.right, WALL_CHECK_DISTANCE, _wallLayer);
            
            List<Vector2> results = new List<Vector2>();
            if (!upHit)
            {
                results.Add(Vector2.up);
            }
            if (!downHit)
            {
                results.Add(Vector2.down);
            }
            if (!leftHit)
            {
                results.Add(Vector2.left);
            }
            if (!rightHit)
            {
                results.Add(Vector2.right);
            }
            
            return results.ToArray();
        }

        private Vector2 GetMoveDirection() {
            bool isNewInput = _inputVector != Vector2.zero;
            if (IsCardinalDirection(_inputVector) && isNewInput) {
                return Vector2Int.RoundToInt(_inputVector);
            }
            return _currentDirection;

        }

        private bool IsCardinalDirection(Vector2 vector)
        {
            return _inputVector - Vector2Int.RoundToInt(_inputVector) == Vector2.zero;
        }

        private void OnInputChanged(InputAction.CallbackContext context) {
            switch (context.action.name) {
                case "Move": {
                    _inputVector = context.ReadValue<Vector2>();
                    break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(thisTransform.position, Vector3.up * WALL_CHECK_DISTANCE, Color.green);
        }
    }

    
}


