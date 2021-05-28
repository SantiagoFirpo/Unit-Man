using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitMan.Source.Utilities;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput), typeof(CircleCollider2D))]
    public class PlayerController : Actor
    {
        public static PlayerController Instance { get; private set; }
        private const float MOVE_SPEED = 3f;
        private const float WALL_CHECK_DISTANCE = 0.8f;
        private const float ALMOST_ONE = 0.9f;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;
        private InputAction.CallbackContext _inputContext;

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

        private readonly bool[] _possibleTurns = {false, false, false, false};

        private enum Turns
        {
            Up = 0, Down = 1, Left = 2, Right = 3
        }

        public bool isInvincible = false;
        private readonly Timer _invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS, 0f);
        private const float INVINCIBLE_TIME_SECONDS = 10f;


        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;
            _wallLayer = LayerMask.GetMask("Wall");
            
            if (Instance != null) {GameObject.Destroy(this);}
            Instance = this;

            _almostUpLeft = _upLeft * ALMOST_ONE;
            _almostUpRight = _upRight * ALMOST_ONE;
            _almostDownLeft = _downLeft * ALMOST_ONE;
            _almostDownRight = _downRight * ALMOST_ONE;

            _invincibleTimer.OnEnd += DisableInvincibility;
        }

        private void DisableInvincibility() {
            isInvincible = false;
        }

        private void FixedUpdate() {
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

        private void CheckPossibleTurns() {
            Vector2 playerPosition = _transform.position;
            
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
            _inputContext = context;
            _inputVector = _inputContext.action.name switch {
                "Move" => RoundToInt(_inputContext.ReadValue<Vector2>()),
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

        public void SetInvincible() {
            isInvincible = true;
            _invincibleTimer.Begin();
        }
    }
}