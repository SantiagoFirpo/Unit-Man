using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public abstract class Actor : MonoBehaviour, IInitializable {
        protected CircleCollider2D circleCollider;
        protected Rigidbody2D rigidBody;
        protected Transform thisTransform;
        
        protected GameObject thisGameObject;

        protected Vector3 startPosition;
        public Vector2 motion = Vector2.zero;

        private const float WALL_CHECK_DISTANCE = 0.8f;
        private const float ALMOST_ONE = 0.9f;

        private readonly Vector2 _upLeft = new Vector2(-0.5f, 0.5f);
        private readonly Vector2 _upRight = new Vector2(0.5f, 0.5f);

        private readonly Vector2 _downLeft = new Vector2(-0.5f, -0.5f);
        private readonly Vector2 _downRight = new Vector2(0.5f, -0.5f);

        private readonly Vector2 _up = Vector2.up;
        private readonly Vector2 _down = Vector2.down;
        private readonly Vector2 _left = Vector2.left;
        private readonly Vector2 _right = Vector2.right;
        
        private Vector2 _almostUpLeft;
        private Vector2 _almostUpRight;
        private Vector2 _almostDownLeft;
        private Vector2 _almostDownRight;
        
        protected LayerMask wallLayer;
        
        [SerializeField]
        protected bool[] possibleTurns = {false, false, false, false};

        protected virtual void Awake() {
            Initialize();
        }

        public virtual void Initialize() {
            circleCollider = GetComponent<CircleCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
            thisTransform = transform;
            thisGameObject = gameObject;
            
            GameManager.OnReset += ResetPosition;

            _almostUpLeft = _upLeft * ALMOST_ONE;
            _almostUpRight = _upRight * ALMOST_ONE;
            _almostDownLeft = _downLeft * ALMOST_ONE;
            _almostDownRight = _downRight * ALMOST_ONE;

            wallLayer = LayerMask.GetMask("Wall");
        }

        private void OnDisable() {
            GameManager.OnReset -= ResetPosition;
        }

        private void ResetPosition() {
            thisTransform.position = startPosition;
        }

        private void CheckPossibleTurns() {
            Vector2 playerPosition = thisTransform.position;
            
            RaycastHit2D upHitOne = Physics2D.Raycast(playerPosition + _almostUpLeft, _up, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D upHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, _up, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D downHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, _down, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D downHitTwo = Physics2D.Raycast(playerPosition + _almostDownRight, _down, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D leftHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, _left, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D leftHitTwo = Physics2D.Raycast(playerPosition + _almostUpLeft, _left, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D rightHitOne = Physics2D.Raycast(playerPosition + _almostDownRight, _right, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D rightHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, _right, WALL_CHECK_DISTANCE, wallLayer);
            
            possibleTurns[0] = !(upHitOne.collider || upHitTwo.collider);
            possibleTurns[1] = !(downHitOne.collider || downHitTwo.collider);
            possibleTurns[2] = !(leftHitOne.collider || leftHitTwo.collider);
            possibleTurns[3] = !(rightHitOne.collider || rightHitTwo.collider);
        }
        
        public static int DirectionToInt(Vector2Int vector) {
            int index = -1;
            if (vector == Vector2Int.up) {
                index = 0;
            }
            else if (vector == Vector2Int.down) {
                index = 1;
            }
            else if (vector == Vector2Int.left) {
                index = 2;
            }
            else if (vector == Vector2Int.right) {
                index = 3;
            }

            return index;
        }
        
        public static Vector2Int IntToDirection(int number) {
            return number switch {
                0 => Vector2Int.up,
                1 => Vector2Int.down,
                2 => Vector2Int.left,
                3 => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }

        protected static bool VectorApproximately(Vector3 v1, Vector2Int v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        protected virtual void FixedUpdate() {
            CheckPossibleTurns();
        }
        
        private void OnDrawGizmos() {
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
    }
}


