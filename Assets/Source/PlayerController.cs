using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput), typeof(CircleCollider2D))]
    public class PlayerController : Actor
    {
        [SerializeField] private int _currentState;
        [SerializeField] private int _previousState;

        private const float MOVE_SPEED = 3f;
        private const float WALL_CHECK_DISTANCE = 0.6f;

        private StateMachine _stateMachine;
        private ITransitionProvider _transitionProvider;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;
        private InputAction.CallbackContext inputContext;
        private InputAction.CallbackContext inputBuffer;
        private const float BUFFER_WINDOW_SECONDS = 0.1f;

        private Vector2Int _currentDirection;
        private LayerMask _wallLayer;


        [FormerlySerializedAs("_wallChecks")] [SerializeField]
        private Vector2Int[] _possibleTurns;


        public enum PlayerState
        {
            Idle,
            Move
        }


        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            _transitionProvider = new PlayerTransitionProvider(this);
            _stateMachine = new StateMachine(
                _transitionProvider,
                new[] {
                    (int) PlayerState.Idle,
                    (int) PlayerState.Move
                });
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;
            _wallLayer = LayerMask.GetMask("Wall");
        }

        private void FixedUpdate() {
            _possibleTurns = CheckPossibleTurns();
            if (IsCardinalDirection(_inputVector) && _possibleTurns.Contains(_inputVector))
                _currentDirection = _inputVector;

            motion = (Vector2) _currentDirection * MOVE_SPEED;

            if (CanTurn()) _rigidBody.velocity = motion;
        }

        private void PollBufferedInput() {
            bool isInBufferWindow = Time.realtimeSinceStartup - inputContext.startTime <= BUFFER_WINDOW_SECONDS;
            if (isInBufferWindow && CanTurn()) _rigidBody.velocity = motion;
        }

        private bool CanTurn() {
            return ((IList) _possibleTurns).Contains(_currentDirection);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer == _wallLayer)
                if (_possibleTurns.Length == 2)
                    _currentDirection = _possibleTurns[0];
        }

        private Vector2Int[] CheckPossibleTurns() {
            Vector2 playerPosition = _transform.position;
            RaycastHit2D upHit = Physics2D.Raycast(playerPosition, Vector2.up, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D downHit = Physics2D.Raycast(playerPosition, Vector2.down, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D leftHit = Physics2D.Raycast(playerPosition, Vector2.left, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D rightHit = Physics2D.Raycast(playerPosition, Vector2.right, WALL_CHECK_DISTANCE, _wallLayer);

            List<Vector2Int> results = new List<Vector2Int>();
            if (!upHit) results.Add(Vector2Int.up);
            if (!downHit) results.Add(Vector2Int.down);
            if (!leftHit) results.Add(Vector2Int.left);
            if (!rightHit) results.Add(Vector2Int.right);

            return results.ToArray();
        }


        private bool IsCardinalDirection(Vector2 vector) {
            return Mathf.Abs(_inputVector.x) != Mathf.Abs(_inputVector.y);
        }

        private void OnInputChanged(InputAction.CallbackContext context) {
            inputContext = context;
            switch (inputContext.action.name) {
                case "Move": {
                    _inputVector = Vector2Int.RoundToInt(inputContext.ReadValue<Vector2>());
                    break;
                }
            }
        }

        private void OnDrawGizmos() {
            Debug.DrawRay(transform.position, Vector3.up * WALL_CHECK_DISTANCE, Color.green);
        }
    }
}