using UnitMan.Source.Utilities.Pathfinding;
using UnitMan.Source.Utilities.TimeTracking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.Source.Entities.Actors
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Actor
    {

        private const float MOVE_SPEED = 5f;
        public const float PLAYER_STEP_TIME = 0.2f;
        private Gameplay _inputMaps;

        private Vector2Int _inputVector;

        public bool isInvincible;
        public Timer invincibleTimer;

        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");
        private static readonly int OnDeathAnimTrigger = Animator.StringToHash("OnDeath");
        private bool _wasStoppedLastFrame = true;
        private bool _isDead;
        private static readonly int OnReset = Animator.StringToHash("OnReset");

        public const float INVINCIBLE_TIME_SECONDS = 10f;

        public void ResetAnimation()
        {
            animator.enabled = true;
            animator.Play("Up");
            animator.SetTrigger(OnReset);
            animator.ResetTrigger(OnReset);
        }
        
        protected override void ResetActor()
        {
            base.ResetActor();
            _isDead = false;
            animator.enabled = false;
            
        }

        protected override void Initialize()
        {
            base.Initialize();
            _inputMaps = new Gameplay();

            invincibleTimer = new Timer(INVINCIBLE_TIME_SECONDS + 1.8f, false, true);
            
            SubscribeForEvents();

            currentDirection = LevelGridController.RightVector2Int;
            currentMoveSpeed = MOVE_SPEED;
        }

        private void SubscribeForEvents()
        {
            _inputMaps.Player.Enable();
            _inputMaps.Player.Move.performed += OnMove;
            
            invincibleTimer.OnEnd += DisableInvincibility;
        }

        protected override void Freeze()
        {
            base.Freeze();
            invincibleTimer.Stop();

        }

        protected override void Unfreeze()
        {
            base.Unfreeze();
            invincibleTimer.Start();
            animator.enabled = true;
        }

        public void Die()
        {
            _isDead = true;
            animator.enabled = true;
            animator.Play("Death");
            animator.SetTrigger(OnDeathAnimTrigger);
            animator.ResetTrigger(OnDeathAnimTrigger);
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            invincibleTimer.OnEnd -= DisableInvincibility;
            _inputMaps.Player.Move.performed -= OnMove;
            _inputMaps.Player.Disable();
        }

        private void DisableInvincibility()
        {
            isInvincible = false;

        }

        protected override void Update()
        {
            base.Update();
            bool isStopped = (thisRigidbody.velocity == Vector2.zero || !thisRigidbody.simulated) && !_isDead;
            if (!_wasStoppedLastFrame)
            {
                animator.enabled = !isStopped;
            }

            _wasStoppedLastFrame = isStopped;
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