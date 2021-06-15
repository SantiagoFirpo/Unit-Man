using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(CircleCollider2D),
                    typeof(Rigidbody2D),
                    typeof(Animator))]
    public abstract class Actor : MonoBehaviour, IInitializable
    {
        protected CircleCollider2D circleCollider;
        public Rigidbody2D rigidBody;
        protected Transform thisTransform;
        
        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");

        protected GameObject thisGameObject;

        protected Vector3 startPosition;
        public Vector2 motion = Vector2.zero;
        protected Vector2Int currentDirection;

        private const float WALL_CHECK_DISTANCE = 0.8f;
        private const float ALMOST_ONE = 0.9f;

        private readonly Vector2 _upLeft = new Vector2(-0.5f, 0.5f);
        private readonly Vector2 _upRight = new Vector2(0.5f, 0.5f);

        private readonly Vector2 _downLeft = new Vector2(-0.5f, -0.5f);
        private readonly Vector2 _downRight = new Vector2(0.5f, -0.5f);

        public static readonly Vector2 Up = Vector2.up;
        public static readonly Vector2 Down = Vector2.down;
        public static readonly Vector2 Left = Vector2.left;
        public static readonly Vector2 Right = Vector2.right;

        private Vector2 _almostUpLeft;
        private Vector2 _almostUpRight;
        private Vector2 _almostDownLeft;
        private Vector2 _almostDownRight;

        protected LayerMask wallLayer;

        [SerializeField] protected bool[] possibleTurns = {false, false, false, false};
        private Animator _animator;

        public enum Direction
        {
            Up, Down, Left, Right
        }

    protected virtual void Awake() {
            Initialize();
        }

        public virtual void Initialize() {
            circleCollider = GetComponent<CircleCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            thisTransform = transform;
            thisGameObject = gameObject;
            
            GameManager.OnReset += ResetPosition;

            _almostUpLeft = _upLeft * ALMOST_ONE;
            _almostUpRight = _upRight * ALMOST_ONE;
            _almostDownLeft = _downLeft * ALMOST_ONE;
            _almostDownRight = _downRight * ALMOST_ONE;

            wallLayer = LayerMask.GetMask("Wall");
        }

        protected virtual void Update() {
            UpdateAnimation();
        }

        private void OnDisable() {
            GameManager.OnReset -= ResetPosition;
        }

        private void ResetPosition() {
            thisTransform.position = startPosition;
        }

        private void CheckPossibleTurns() {
            Vector2 playerPosition = thisTransform.position;
            
            RaycastHit2D upHitOne = Physics2D.Raycast(playerPosition + _almostUpLeft, Up, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D upHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Up, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D downHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Down, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D downHitTwo = Physics2D.Raycast(playerPosition + _almostDownRight, Down, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D leftHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Left, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D leftHitTwo = Physics2D.Raycast(playerPosition + _almostUpLeft, Left, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D rightHitOne = Physics2D.Raycast(playerPosition + _almostDownRight, Right, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D rightHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Right, WALL_CHECK_DISTANCE, wallLayer);
            
            possibleTurns[0] = !(upHitOne.collider || upHitTwo.collider);
            possibleTurns[1] = !(downHitOne.collider || downHitTwo.collider);
            possibleTurns[2] = !(leftHitOne.collider || leftHitTwo.collider);
            possibleTurns[3] = !(rightHitOne.collider || rightHitTwo.collider);
        }
        
        public static int DirectionToInt(Vector2 vector) {
            int index = -1;
            if (vector == Up) {
                index = (int) Direction.Up;
            }
            else if (vector == Down) {
                index = (int) Direction.Down;
            }
            else if (vector == Left) {
                index = (int) Direction.Left;
            }
            else if (vector == Right) {
                index = (int) Direction.Right;
            }

            return index;
        }
        
        protected static bool IsCardinalDirection(Vector2 vector) {
            return Mathf.Abs(vector.x) - Mathf.Abs(vector.y) != 0f;
        }
        
        public static Vector2Int EnumToVector2Int(int enumDirection) {
            return enumDirection switch {
                (int) Direction.Up => Vector2Int.up,
                (int) Direction.Down => Vector2Int.down,
                (int) Direction.Left => Vector2Int.left,
                (int) Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }

        protected virtual void FixedUpdate() {
            CheckPossibleTurns();
        }
        
        protected virtual void OnDrawGizmos() {
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

        protected void UpdateAnimation() {
            _animator.enabled = rigidBody.velocity != Vector2.zero;
            _animator.SetInteger(DirectionXAnimator, currentDirection.x);
            _animator.SetInteger(DirectionYAnimator, currentDirection.y);
        }
    }
}


