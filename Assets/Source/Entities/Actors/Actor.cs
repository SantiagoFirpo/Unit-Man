using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;

namespace UnitMan.Source.Entities.Actors {
    [RequireComponent(typeof(CircleCollider2D),
                    typeof(Rigidbody2D),
                    typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Actor : MonoBehaviour, IInitializable
    {
        //TODO: refactor/organize this class
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
        protected float currentMoveSpeed;

        protected bool IsInTileCenter => PathGrid.VectorApproximately(thisTransform.position, gridPosition, 0.1f);

        [SerializeField] protected bool[] possibleTurns = {false, false, false, false};
        
        [HideInInspector]
        public Animator animator;
        
        public Vector2Int gridPosition;
        private bool _isWrapping;

        public enum Direction
        {
            Up, Down, Left, Right
        }

    protected void Awake() {
            Initialize();
        }

    protected abstract void Reset();

    protected virtual void Freeze()
    {
        animator.enabled = false;
        thisRigidbody.simulated = false;
    }

    protected virtual void Unfreeze()
    {
        animator.enabled = true;
        thisRigidbody.simulated = true;
    }

    public virtual void Initialize() {
            thisRigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            thisTransform = transform;
            thisGameObject = gameObject;
            
            SubscribeForEvents();
            StartPosition = thisTransform.position;
        }

    private void SubscribeForEvents()
    {
        SessionManagerSingle.OnReset += ResetPosition;
        SessionManagerSingle.OnFreeze += Freeze;
        SessionManagerSingle.OnUnfreeze += Unfreeze;
    }

    protected virtual void Update()
    {
        if (!thisRigidbody.simulated) return;
            UpdateAnimation();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        protected virtual void UnsubscribeFromEvents()
        {
            SessionManagerSingle.OnReset -= ResetPosition;
            SessionManagerSingle.OnFreeze -= Freeze;
            SessionManagerSingle.OnUnfreeze -= Unfreeze;
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
            if (vector == PathGrid.upVector2Int) {
                index = (int) Direction.Up;
            }
            else if (vector == PathGrid.downVector2Int) {
                index = (int) Direction.Down;
            }
            else if (vector == PathGrid.leftVector2Int) {
                index = (int) Direction.Left;
            }
            else if (vector == PathGrid.rightVector2Int) {
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
                Direction.Up => PathGrid.upVector2Int,
                Direction.Down => PathGrid.downVector2Int,
                Direction.Left => PathGrid.leftVector2Int,
                Direction.Right => PathGrid.rightVector2Int,
                _ => Vector2Int.zero
            };
        }

        protected virtual void FixedUpdate() {
            UpdateGridPosition();
            PathGrid.Instance.CheckPossibleTurns(gridPosition, possibleTurns);
            
            if (PathGrid.VectorApproximately(thisTransform.position,
                                            SessionManagerSingle.Instance.rightWrap.position,
                                            0.1f)
                && !_isWrapping)
            {
                thisTransform.position = SessionManagerSingle.Instance.leftWrap.position;
                _isWrapping = true;
            }
            
            else if (PathGrid.VectorApproximately(thisTransform.position,
                                                SessionManagerSingle.Instance.leftWrap.position,
                                                0.1f)
                     && !_isWrapping)
            {
                thisTransform.position = SessionManagerSingle.Instance.rightWrap.position;
                _isWrapping = true;
            }

            else //if (PathGrid.TaxiCabDistanceVector3(thisTransform.position, leftWrap.position) > 1f
                //         && PathGrid.TaxiCabDistanceVector3(thisTransform.position, rightWrap.position) > 1f)
            {
                _isWrapping = false;
            }
        }

        protected void UpdateAnimation() {
            animator.SetInteger(DirectionXAnimator, currentDirection.x);
            animator.SetInteger(DirectionYAnimator, currentDirection.y);
        }
    }
}


