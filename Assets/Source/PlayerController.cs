using System;
using Input;
using UnitMan.Source.Utilities.Pathfinding;
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
        //TODO: screen wrap
        //TODO: death animation

        private const float MOVE_SPEED = 5f;
        public const float PLAYER_STEP_TIME = 0.2f;
        private InputMaps _inputMaps;

        private PlayerInput _playerInput;
        private Vector2Int _inputVector;

        public bool isInvincible;
        public readonly Timer invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS, false, true);
        public static event Action<bool> OnInvincibleChanged;
        public const float INVINCIBLE_TIME_SECONDS = 10f;
        
        public override void Initialize()
        {
            base.Initialize();
            _inputMaps = new InputMaps();
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            _playerInput = GetComponent<PlayerInput>();

            invincibleTimer.OnEnd += DisableInvincibility;
            currentDirection = PathGrid.RightVector2Int;
            currentMoveSpeed = MOVE_SPEED;
        }

        protected override void Update()
        {
            base.Update();
            if (isInvincible)
            {
                SessionDataModel.Instance.UpdateInvincibleTime(invincibleTimer.currentTime);
            }
        }
        

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _playerInput.onActionTriggered -= OnMove;
            invincibleTimer.OnEnd -= DisableInvincibility;
            
            _inputMaps.Player.Disable();
        }

        private void DisableInvincibility()
        {
            isInvincible = false;
            OnInvincibleChanged?.Invoke(isInvincible);
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsCardinalDirection(_inputVector))
            {
                int index = VectorToInt(_inputVector);

                if (possibleTurns[index] && IsInTileCenter)
                {
                    currentDirection = _inputVector;
                }
            }


            UpdateMotion(new Vector2(currentDirection.x, currentDirection.y) * currentMoveSpeed);
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            _inputVector = RoundToInt(context.ReadValue<Vector2>());
        }

        public void SetInvincible()
        {
            isInvincible = true;
            invincibleTimer.Start();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.PowerPellet, 1, true);
            OnInvincibleChanged?.Invoke(isInvincible);
        }
    }
}