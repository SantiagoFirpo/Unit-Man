using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput), typeof(CircleCollider2D))]
    public class PlayerController : Actor
    {
        [SerializeField] private int _currentState;
        [SerializeField] private int _previousState;

        private const float MOVE_SPEED = 3f;
        private const float WALL_CHECK_DISTANCE = 0.8f;
        const float ALMOST_ONE = 0.9f;

        private StateMachine _stateMachine;
        private ITransitionProvider _transitionProvider;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;
        private InputAction.CallbackContext inputContext;
        private InputAction.CallbackContext inputBuffer;
        private const float BUFFER_WINDOW_SECONDS = 0.1f;

        private Vector2Int _currentDirection;
        private LayerMask _wallLayer;
        
        private readonly Vector2 _upLeft = new Vector2(-0.5f, 0.5f);
        private readonly Vector2 _upRight = new Vector2(0.5f, 0.5f);

        private readonly Vector2 _downLeft = new Vector2(-0.5f, -0.5f);
        private readonly Vector2 _downRight = new Vector2(0.5f, -0.5f);

        private Vector2 _almostUpLeft;
        private Vector2 _almostUpRight;
        private Vector2 _almostDownLeft;
        private Vector2 _almostDownRight;
        
        Vector2 playerPosition;
        
        private readonly bool[] _possibleTurns = {false, false, false, false};

        private enum Turns
        {
            Up = 0, Down = 1, Left = 2, Right = 3
        }


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

            _almostUpLeft = _upLeft * ALMOST_ONE;
            _almostUpRight = _upRight * ALMOST_ONE;
            _almostDownLeft = _downLeft * ALMOST_ONE;
            _almostDownRight = _downRight * ALMOST_ONE;
        }

        private void FixedUpdate() {
            playerPosition = _transform.position;
            if (!IsCardinalDirection(_inputVector)) return;
            CheckPossibleTurns();
            int index = GetDirectionIndex(_inputVector);

            if (_possibleTurns[index]) {
                _currentDirection = _inputVector;
            }
                

            motion = (Vector2) _currentDirection * MOVE_SPEED;

            // if (_rigidBody.velocity == Vector2.zero) {
            //     _transform.position = Vector3Int.RoundToInt(_transform.position);
            // }

            _rigidBody.velocity = motion;
        }

        private static int GetDirectionIndex(Vector2Int vector) {
            int index = -1;
            if (vector == up) {
                index = 0;
            }
            else if (vector == down) {
                index = 1;
            }
            else if (vector == left) {
                index = 2;
            }
            else if (vector == right) {
                index = 3;
            }

            return index;
        }

        private bool CanTurn() {
            return _possibleTurns[GetDirectionIndex(_inputVector)];
        }

        private void CheckPossibleTurns() {
            playerPosition = _transform.position;
            
            RaycastHit2D upHitOne = Physics2D.Raycast(playerPosition + _almostUpLeft, Vector2.up, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D upHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Vector2.up, WALL_CHECK_DISTANCE, _wallLayer);

            RaycastHit2D downHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Vector2.down, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D downHitTwo = Physics2D.Raycast(playerPosition + _almostDownRight, Vector2.down, WALL_CHECK_DISTANCE, _wallLayer);

            RaycastHit2D leftHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Vector2.left, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D leftHitTwo = Physics2D.Raycast(playerPosition + _almostUpLeft, Vector2.left, WALL_CHECK_DISTANCE, _wallLayer);
            
            RaycastHit2D rightHitOne = Physics2D.Raycast(playerPosition + _almostDownRight, Vector2.right, WALL_CHECK_DISTANCE, _wallLayer);
            RaycastHit2D rightHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Vector2.right, WALL_CHECK_DISTANCE, _wallLayer);
            
            _possibleTurns[0] = !(upHitOne.collider || upHitTwo.collider);
            _possibleTurns[1] = !(downHitOne.collider || downHitTwo.collider);
            _possibleTurns[2] = !(leftHitOne.collider || leftHitTwo.collider);
            _possibleTurns[3] = !(rightHitOne.collider || rightHitTwo.collider);
        }


        private bool IsCardinalDirection(Vector2 vector) {
            return Mathf.Abs(_inputVector.x) != Mathf.Abs(_inputVector.y);
        }

        private void OnInputChanged(InputAction.CallbackContext context) {
            inputContext = context;
            _inputVector = inputContext.action.name switch {
                "Move" => RoundToInt(inputContext.ReadValue<Vector2>()),
                _ => _inputVector
            };
        }

        private void OnDrawGizmos() {
            Vector3 position = transform.position;
            Debug.DrawRay(position + (Vector3) _upLeft * ALMOST_ONE, Vector2.up * WALL_CHECK_DISTANCE, Color.green);
            Debug.DrawRay(position + (Vector3) _upRight * ALMOST_ONE, Vector2.up * WALL_CHECK_DISTANCE, Color.green);
            
            Debug.DrawRay(position + (Vector3) _downLeft * ALMOST_ONE, Vector2.down * WALL_CHECK_DISTANCE, Color.green);
            Debug.DrawRay(position + (Vector3) _downRight * ALMOST_ONE, Vector2.down * WALL_CHECK_DISTANCE, Color.green);
            
            Debug.DrawRay(position + (Vector3) _downLeft * ALMOST_ONE, Vector2.left * WALL_CHECK_DISTANCE, Color.green);
            Debug.DrawRay(position + (Vector3) _upLeft * ALMOST_ONE, Vector2.left * WALL_CHECK_DISTANCE, Color.green);
            
            Debug.DrawRay(position + (Vector3) _downRight * ALMOST_ONE, Vector2.right * WALL_CHECK_DISTANCE, Color.green);
            Debug.DrawRay(position + (Vector3) _upRight * ALMOST_ONE, Vector2.right * WALL_CHECK_DISTANCE, Color.green);
        }
    }
}