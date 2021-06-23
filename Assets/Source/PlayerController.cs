using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source
{
    [RequireComponent(typeof(PlayerInput),
        typeof(SpriteRenderer),
        typeof(Animator))]
    public class PlayerController : Actor
    {
        private const float MOVE_SPEED = 5f;
        private InputMaps _inputMaps;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;

        public bool isInvincible;
        public readonly Timer invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS);
        private SpriteRenderer _spriteRenderer;
        public static event Action<bool> OnInvincibleChanged;
        public const float INVINCIBLE_TIME_SECONDS = 10f;


        // Start is called before the first frame update
        protected override void Awake() {
            base.Awake();
            startPosition = thisTransform.position;
            _inputMaps = new InputMaps();
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            _playerInput = GetComponent<PlayerInput>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            // _playerInput.onActionTriggered += OnMove;

            invincibleTimer.OnEnd += DisableInvincibility;
        }

        protected override void Update() {
            base.Update();
            if (isInvincible) {
                UIModel.Instance.UpdateInvincibleTime(invincibleTimer.currentTime);
            }
        }

        // protected override void UpdateAnimation() {
        //     if (rigidBody.velocity == Vector2.zero) {
        //         _animator.enabled = false;
        //         return;
        //     }
        //     _animator.enabled = true;
        //     Vector3 currentRotation = thisTransform.eulerAngles;
        //     if (currentDirection == Up) {
        //         thisTransform.eulerAngles = new Vector3(currentRotation.x,
        //             currentRotation.y,
        //             90f);
        //     }
        //     else if (currentDirection == Down) {
        //         thisTransform.eulerAngles = new Vector3(currentRotation.x,
        //             currentRotation.y,
        //             -90f);
        //     }
        //     else if (currentDirection == Left) {
        //         thisTransform.eulerAngles = new Vector3(currentRotation.x,
        //             currentRotation.y, 180f);
        //     }
        //     else if (currentDirection == Right) {
        //         thisTransform.eulerAngles = new Vector3(currentRotation.x,
        //             currentRotation.y,
        //             0f);
        //     }
        //     // _spriteRenderer.flipX = _currentDirection.x < 0;
        //     // thisTransform.eulerAngles = new Vector3(currentRotation.x,
        //     //                                         currentRotation.y,
        //     //                                         _currentDirection.y < 0 ? -90f : 90f);
        // }

        private void OnDisable() {
            _playerInput.onActionTriggered -= OnMove;
            _inputMaps.Player.Disable();
            invincibleTimer.OnEnd -= DisableInvincibility;
        }

        private void DisableInvincibility() {
            isInvincible = false;
            OnInvincibleChanged?.Invoke(isInvincible);
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.Siren, 1, true);
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (!IsCardinalDirection(_inputVector)) return;
            int index = VectorToInt(_inputVector);

            if (possibleTurns[index]) {
                currentDirection = _inputVector;
            }
                

            motion = (Vector2) currentDirection * MOVE_SPEED;

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
            invincibleTimer.Start();
            AudioManager.Instance.PlayClip(AudioManager.AudioEffectType.PowerPellet, 1, true);
            OnInvincibleChanged?.Invoke(isInvincible);
        }

    }
}