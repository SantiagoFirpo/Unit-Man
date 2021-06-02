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
        private const float MOVE_SPEED = 5f;
        private InputMaps _inputMaps;

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
        public static event Action<bool> OnInvincibleChanged;
        private const float INVINCIBLE_TIME_SECONDS = 10f;


        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            startPosition = new Vector3(2f, -4f, 0f);
            _inputMaps = new InputMaps();
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            _playerInput = GetComponent<PlayerInput>();
            // _playerInput.onActionTriggered += OnMove;

            _invincibleTimer.OnEnd += DisableInvincibility;
        }

        private void OnDisable() {
            _playerInput.onActionTriggered -= OnMove;
            _inputMaps.Player.Disable();
            _invincibleTimer.OnEnd -= DisableInvincibility;
        }

        private void DisableInvincibility() {
            isInvincible = false;
            OnInvincibleChanged?.Invoke(isInvincible);
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (!IsCardinalDirection(_inputVector)) return;
            int index = Actor.DirectionToInt(_inputVector);

            if (possibleTurns[index]) {
                _currentDirection = _inputVector;
            }
                

            motion = (Vector2) _currentDirection * MOVE_SPEED;

            // if (_rigidBody.velocity == Vector2.zero) {
            //     _transform.position = Vector3Int.RoundToInt(_transform.position);
            // }

            rigidBody.velocity = motion;
        }



        private bool IsCardinalDirection(Vector2 vector) {
            return Mathf.Abs(_inputVector.x) != Mathf.Abs(_inputVector.y);
        }

        private void OnMove(InputAction.CallbackContext context) {
            _inputVector = RoundToInt(context.ReadValue<Vector2>());
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
            OnInvincibleChanged?.Invoke(isInvincible);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (isInvincible) {
                //Eat ghost
            }
            else {
                GameManager.Instance.Die();
            }
        }
        
    }
}