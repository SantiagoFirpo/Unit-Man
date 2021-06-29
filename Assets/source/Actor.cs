using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(CircleCollider2D),
                    typeof(Rigidbody2D),
                    typeof(Animator))]
    public abstract class Actor : MonoBehaviour, IInitializable
    {
        //TODO: refactor/organize this class
        protected CircleCollider2D circleCollider;

        public Rigidbody2D Rigidbody => thisRigidbody;

        protected Rigidbody2D thisRigidbody;

        protected Transform thisTransform;

        private static readonly int DirectionXAnimator = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYAnimator = Animator.StringToHash("DirectionY");

        protected GameObject thisGameObject;

        protected Vector3 StartPosition {get; private set;}

        [HideInInspector]
        public Vector2 motion = Vector2.zero;
        public Vector2Int currentDirection;

        private const float WALL_CHECK_DISTANCE = 0.8f;
        private const float ALMOST_ONE = 0.9f;

        private readonly Vector2 _upLeft = new Vector2(-0.5f, 0.5f);
        private readonly Vector2 _upRight = new Vector2(0.5f, 0.5f);

        private readonly Vector2 _downLeft = new Vector2(-0.5f, -0.5f);
        private readonly Vector2 _downRight = new Vector2(0.5f, -0.5f);

        protected bool IsInTileCenter => PathGrid.VectorApproximately(thisTransform.position, gridPosition, 0.1f);

        [SerializeField] protected bool[] possibleTurns = {false, false, false, false};
        
        [HideInInspector]
        public Animator animator;
        
        public Vector2Int gridPosition;

        public enum Direction
        {
            Up, Down, Left, Right
        }

    protected virtual void Awake() {
            Initialize();
        }

        public virtual void Initialize() {
            circleCollider = GetComponent<CircleCollider2D>();
            thisRigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            thisTransform = transform;
            thisGameObject = gameObject;
            
            GameManager.OnReset += ResetPosition;
            StartPosition = thisTransform.position;
        }

        protected virtual void Update() {
            UpdateAnimation();
        }

        private void OnDisable() {
            GameManager.OnReset -= ResetPosition;
        }

        private void ResetPosition() {
            thisTransform.position = StartPosition;
        }

        protected void UpdateGridPosition() {
           gridPosition = Vector2Int.RoundToInt(thisTransform.position);
       }


        public static int VectorToInt(Vector2 vector) {
            int index = -1;
            if (vector == PathGrid.UpVector2) {
                index = (int) Direction.Up;
            }
            else if (vector == PathGrid.DownVector2) {
                index = (int) Direction.Down;
            }
            else if (vector == PathGrid.LeftVector2) {
                index = (int) Direction.Left;
            }
            else if (vector == PathGrid.DownVector2) {
                index = (int) Direction.Right;
            }

            return index;
        }
        
        public static int VectorToInt(Vector2Int vector) {
            int index = -1;
            if (vector == PathGrid.UpVector2Int) {
                index = (int) Direction.Up;
            }
            else if (vector == PathGrid.DownVector2Int) {
                index = (int) Direction.Down;
            }
            else if (vector == PathGrid.LeftVector2Int) {
                index = (int) Direction.Left;
            }
            else if (vector == PathGrid.RightVector2Int) {
                index = (int) Direction.Right;
            }

            return index;
        }

        protected static bool IsCardinalDirection(Vector2 vector) {
            return Mathf.Abs(vector.x) - Mathf.Abs(vector.y) != 0f;
        }
        
        protected static bool IsCardinalDirection(Vector2Int vector) {
            return Mathf.Abs(vector.x) - Mathf.Abs(vector.y) != 0f;
        }
        
        public static Vector2Int DirectionToVector2Int(int enumDirection) {
            return enumDirection switch {
                (int) Direction.Up => Vector2Int.up,
                (int) Direction.Down => Vector2Int.down,
                (int) Direction.Left => Vector2Int.left,
                (int) Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
        
        protected void UpdateMotion(Vector2 newMotion)
        {
            motion = newMotion;
            thisRigidbody.velocity = motion;
        }
        
        public static Vector2Int DirectionToVector2Int(Direction enumDirection) {
            return enumDirection switch {
                Direction.Up => PathGrid.UpVector2Int,
                Direction.Down => PathGrid.DownVector2Int,
                Direction.Left => PathGrid.LeftVector2Int,
                Direction.Right => PathGrid.RightVector2Int,
                _ => Vector2Int.zero
            };
        }

        protected virtual void FixedUpdate() {
            UpdateGridPosition();
            PathGrid.Instance.CheckPossibleTurns(gridPosition, possibleTurns);
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
            animator.enabled = thisRigidbody.velocity != Vector2.zero;
            animator.SetInteger(DirectionXAnimator, currentDirection.x);
            animator.SetInteger(DirectionYAnimator, currentDirection.y);
        }
    }
}


