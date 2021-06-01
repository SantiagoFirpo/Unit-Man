using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput), typeof(CircleCollider2D))]
    public class PlayerController : Actor
    {
        public static PlayerController Instance { get; private set; }
        private const float MOVE_SPEED = 3f;
        

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;
        private InputAction.CallbackContext _inputContext;

        private Vector2Int _currentDirection;
        

   

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
            startPosition = new Vector3(2f, -4f, 0f);
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += OnInputChanged;

            if (Instance != null) {GameObject.Destroy(this);}
            Instance = this;

            _invincibleTimer.OnEnd += DisableInvincibility;
        }

        private void DisableInvincibility() {
            isInvincible = false;
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (!IsCardinalDirection(_inputVector)) return;
            int index = DirectionToInt(_inputVector);

            if (possibleTurns[index]) {
                _currentDirection = _inputVector;
            }
                

            motion = (Vector2) _currentDirection * MOVE_SPEED;

            // if (_rigidBody.velocity == Vector2.zero) {
            //     _transform.position = Vector3Int.RoundToInt(_transform.position);
            // }

            _rigidBody.velocity = motion;
        }

        private static int DirectionToInt(Vector2Int vector) {
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
        
        private static Vector2Int IntToDirection(int number) {
            return number switch {
                0 => Vector2Int.right,
                1 => Vector2Int.left,
                2 => Vector2Int.down,
                3 => Vector2Int.right,
                _ => Vector2Int.zero
            };
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

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Enemy")) {
                if (isInvincible) {
                    //Eat ghost
                }
                else {
                    GameManager.Instance.Die();
                }
            }
        }
        
    }
}