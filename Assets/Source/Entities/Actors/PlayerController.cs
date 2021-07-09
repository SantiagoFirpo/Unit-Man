using System;
using Input;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.Source.Entities.Actors
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Actor
    {
        //TODO: death animation

        private const float MOVE_SPEED = 5f;
        public const float PLAYER_STEP_TIME = 0.2f;
        private InputMaps _inputMaps;

        private Vector2Int _inputVector;

        public bool isInvincible;
        private Timer _invincibleTimer;

        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");
        private static readonly int DeadAnimatorParam = Animator.StringToHash("Dead");

        public const float INVINCIBLE_TIME_SECONDS = 10f;

        protected override void ResetActor()
        {
            animator.SetBool(DeadAnimatorParam, true);
            animator.enabled = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            _inputMaps = new InputMaps();

            _invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS, false, true);
            
            SubscribeForEvents();

            currentDirection = LevelGridController.rightVector2Int;
            currentMoveSpeed = MOVE_SPEED;
        }

        private void SubscribeForEvents()
        {
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            
            _invincibleTimer.OnEnd += DisableInvincibility;
        }

        protected override void Freeze(Emitter<FreezeType> source, FreezeType freezeType)
        {
            base.Freeze(source, freezeType);
            _invincibleTimer.Stop();
            // Debug.Log(freezeType.ToString());
            if (freezeType != FreezeType.Death) return;
            // Debug.Log("Should Activate Death Animation");
            frozen = false;
            animator.enabled = true;
            animator.SetBool(DeadAnimatorParam, true);
            animator.Play("Death");

        }

        protected override void Unfreeze()
        {
            base.Unfreeze();
            _invincibleTimer.Start();
            animator.enabled = true;
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
            AudioManagerSingle.Instance.PlayClip(AudioManagerSingle.AudioEffectType.Siren, 1, true);
        }

        protected override void Update()
        {
            base.Update();
            animator.enabled = !(thisRigidbody.velocity == Vector2.zero || frozen);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            isInTileCenter = IsInTileCenter;
            if (IsCardinalDirection(_inputVector))
            {
                int index = VectorToInt(_inputVector);
                if (possibleTurns[index] && isInTileCenter)
                {
                    currentDirection = _inputVector;
                }
            }


            UpdateMotion(new Vector2(currentDirection.x, currentDirection.y) * currentMoveSpeed);
        }

        protected override void UpdateAnimation()
        {
            animator.SetInteger(DirectionXAnimator, currentDirection.x);
            animator.SetInteger(DirectionYAnimator, currentDirection.y);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _inputVector = Vector2Int.RoundToInt(context.ReadValue<Vector2>());
        }
    }
}