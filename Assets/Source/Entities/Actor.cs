using UnitMan.Source.Management.Session;
using UnitMan.Source.Utilities.ObserverSystem;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;

namespace UnitMan.Source.Entities {
    [RequireComponent(typeof(CircleCollider2D),
                    typeof(Rigidbody2D),
                    typeof(Animator))]
    
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Actor : MonoBehaviour
    {
        //TODO: refactor/organize this class

        protected Rigidbody2D thisRigidbody;

        private Transform _thisTransform;

        protected GameObject thisGameObject;

        protected Vector3 StartPosition {get; private set;}
        
        private bool _isCenteredAtWrap;


        [HideInInspector]
        public Vector2 motion = Vector2.zero;
        public Vector2Int currentDirection;
        protected float currentMoveSpeed;

        protected bool isInTileCenter;

        protected bool IsInTileCenter => VectorUtil.VectorApproximately(_thisTransform.position, gridPosition, 0.11f);

        [SerializeField] protected bool[] possibleTurns = {false, false, false, false};
        
        protected Animator animator;


        public Vector2Int gridPosition;
        private bool _isWrapping;

        private Observer _resetObserver;
        private Observer _freezeObserver;
        private Observer _unfreezeObserver;

        public enum Direction
        {
            Up, Down, Left, Right
        }
        

    protected virtual void ResetActor()
    {
        ResetPosition();
    }

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

    protected virtual void ResolveDependencies() {
        SubscribeForEvents();
            
    }

    private void SubscribeForEvents()
    {
        SessionManagerSingle.Instance.resetObservable.Attach(_resetObserver);
        SessionManagerSingle.Instance.freezeObservable.Attach(_freezeObserver);
        SessionManagerSingle.Instance.unfreezeObservable.Attach(_unfreezeObserver);
    }

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        _resetObserver = new Observer(ResetActor);
        _freezeObserver = new Observer(Freeze);
        _unfreezeObserver = new Observer(Unfreeze);
        thisRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _thisTransform = transform;
        AssignStartPosition();
        thisGameObject = gameObject;
    }

    public void AssignStartPosition()
    {
        StartPosition = _thisTransform.position;
    }

    private void Start()
    {
        ResolveDependencies();
        ResetActor();
        
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
            SessionManagerSingle.Instance.resetObservable.Detach(_resetObserver);
        }

        private void ResetPosition() {
            _thisTransform.position = StartPosition;
        }

        protected void UpdateGridPosition() {
           gridPosition = Vector2Int.RoundToInt(_thisTransform.position);
       }

        protected static int VectorToInt(Vector2Int vector) {
            int index = -1;
            if (vector == LevelGridController.UpVector2Int) {
                index = (int) Direction.Up;
            }
            else if (vector == LevelGridController.DownVector2Int) {
                index = (int) Direction.Down;
            }
            else if (vector == LevelGridController.LeftVector2Int) {
                index = (int) Direction.Left;
            }
            else if (vector == LevelGridController.RightVector2Int) {
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

        protected static Vector2Int DirectionToVector2Int(int enumDirection) {
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

        protected void UpdateMotion(float x, float y)
        {
            motion.x = x;
            motion.y = y;
            thisRigidbody.velocity = motion;
        }

        protected static Vector2Int DirectionToVector2Int(Direction enumDirection) {
            return enumDirection switch {
                Direction.Up => LevelGridController.UpVector2Int,
                Direction.Down => LevelGridController.DownVector2Int,
                Direction.Left => LevelGridController.LeftVector2Int,
                Direction.Right => LevelGridController.RightVector2Int,
                _ => Vector2Int.zero
            };
        }
        
        protected bool IsCenteredAt(Vector2Int position)
        {
            return VectorUtil.VectorApproximately(position, _thisTransform.position, 0.1f); //OLD: 0.1f
        }
        
        protected bool IsCenteredAt(Vector2Int position, float toleranceInclusive)
        {
            return VectorUtil.VectorApproximately(position, _thisTransform.position, toleranceInclusive); //OLD: 0.1f
        }

        private bool IsCenteredAt(Vector3 position)
        {
            return VectorUtil.VectorApproximately(position, _thisTransform.position, 0.1f);
        }

        protected virtual void FixedUpdate() {
            UpdateGridPosition();
            LevelGridController.Instance.CheckPossibleTurns(gridPosition, possibleTurns);
            
            ComputeScreenWraps();
        }

        private void ComputeScreenWraps()
        {
            foreach (Vector2Int position in LevelGridController.Instance.level.screenWrapPositions)
            {
                // Debug.Log(i);
                _isCenteredAtWrap = IsCenteredAt(position, 0.11f);

                if (_isCenteredAtWrap)
                {
                    if (_isWrapping) return;
                    Vector2Int? destination = LevelGridController.Instance.FindWrapDestinationFromPosition(position);
                    Wrap(destination);
                    return;
                }

                if (!IsInTileCenter)
                {
                    _isWrapping = false;
                }
            }
        }

        private void Wrap(Vector2Int? destination)
        {
            if (!destination.HasValue) return;
            _isWrapping = true;
            _thisTransform.position = VectorUtil.ToVector3(destination.Value);
        }

        protected abstract void UpdateAnimation();
    }
}


