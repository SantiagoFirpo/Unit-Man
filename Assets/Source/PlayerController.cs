using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Actor
    {
        private const float MOVE_SPEED = 5f;
        private InputMaps _inputMaps;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;

        private Vector2Int _currentDirection;
        
        public bool isInvincible;
        private readonly Timer _invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS);
        public static event Action<bool> OnInvincibleChanged;
        public const float INVINCIBLE_TIME_SECONDS = 10f;


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

        private void Update() {
            if (isInvincible) {
                UIModel.Instance.UpdateInvincibleTime(_invincibleTimer.currentTime);
            }
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
            int index = DirectionToInt(_inputVector);

            if (possibleTurns[index]) {
                _currentDirection = _inputVector;
            }
                

            motion = (Vector2) _currentDirection * MOVE_SPEED;

            // if (_rigidBody.velocity == Vector2.zero) {
            //     _transform.position = Vector3Int.RoundToInt(_transform.position);
            // }

            rigidBody.velocity = motion;
        }
        

        private void OnMove(InputAction.CallbackContext context) {
            _inputVector = RoundToInt(context.ReadValue<Vector2>());
        }

        public void SetInvincible() {
            isInvincible = true;
            _invincibleTimer.Begin();
            OnInvincibleChanged?.Invoke(isInvincible);
        }

    }
}