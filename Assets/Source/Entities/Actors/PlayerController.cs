using System;
using Input;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Vector2Int;

namespace UnitMan.Source.Entities.Actors
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Actor
    {
        //TODO: screen wrap
        //TODO: death animation

        private const float MOVE_SPEED = 5f;
        public const float PLAYER_STEP_TIME = 0.2f;
        private InputMaps _inputMaps;

        private Vector2Int _inputVector;

        public bool isInvincible;
        private readonly Timer _invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS, false, true);

        private bool _frozen;

        public static event Action<bool> OnInvincibleChanged;
        public const float INVINCIBLE_TIME_SECONDS = 10f;

        protected override void Reset()
        { }

        public override void Initialize()
        {
            base.Initialize();
            _inputMaps = new InputMaps();
            
            SubscribeForEvents();

            currentDirection = PathGrid.rightVector2Int;
            currentMoveSpeed = MOVE_SPEED;
        }

        private void SubscribeForEvents()
        {
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            
            _invincibleTimer.OnEnd += DisableInvincibility;
        }

        protected override void Update()
        {
            // if (!thisRigidbody.simulated) return;
            base.Update();
            animator.enabled = thisRigidbody.velocity != PathGrid.zero && !_frozen;
            if (isInvincible)
            {
                SessionDataModel.Instance.UpdateInvincibleTime(_invincibleTimer.currentTime);
            }
        }

        protected override void Freeze()
        {
            base.Freeze();
            _invincibleTimer.Stop();
            _frozen = true;
        }

        protected override void Unfreeze()
        {
            base.Unfreeze();
            _invincibleTimer.Start();
            _frozen = false;
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _invincibleTimer.OnEnd -= DisableInvincibility;
            _inputMaps.Player.Move.performed -= OnMove;
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
            _invincibleTimer.Start();
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.PowerPellet, 1, true);
            OnInvincibleChanged?.Invoke(isInvincible);
        }
    }
}