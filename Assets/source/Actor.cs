using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public abstract class Actor : MonoBehaviour, IInitializable {
        protected CircleCollider2D _circleCollider;
        protected Rigidbody2D _rigidBody;
        protected Transform _transform;
        protected GameObject _gameObject;

        protected Vector3 startPosition;
        public Vector2 motion = Vector2.zero;
        
        protected const float WALL_CHECK_DISTANCE = 0.8f;
        protected const float ALMOST_ONE = 0.9f;
        
        protected readonly Vector2 _upLeft = new Vector2(-0.5f, 0.5f);
        protected readonly Vector2 _upRight = new Vector2(0.5f, 0.5f);

        protected readonly Vector2 _downLeft = new Vector2(-0.5f, -0.5f);
        protected readonly Vector2 _downRight = new Vector2(0.5f, -0.5f);

        private Vector2 _almostUpLeft;
        private Vector2 _almostUpRight;
        private Vector2 _almostDownLeft;
        private Vector2 _almostDownRight;
        
        protected LayerMask wallLayer;
        
        protected readonly bool[] possibleTurns = {false, false, false, false};
        protected int _possibleTurnsAmount;

        protected Dictionary<int, Vector2Int> allDirections = new Dictionary<int, Vector2Int>();
        
        protected virtual void Awake() {
            Initialize();
        }

        public void Initialize() {
            _circleCollider = GetComponent<CircleCollider2D>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _transform = transform;
            _gameObject = gameObject;
            
            GameManager.OnReset += ResetPosition;

            _almostUpLeft = _upLeft * ALMOST_ONE;
            _almostUpRight = _upRight * ALMOST_ONE;
            _almostDownLeft = _downLeft * ALMOST_ONE;
            _almostDownRight = _downRight * ALMOST_ONE;
            
            allDirections.Add(0, Vector2Int.up);
            allDirections.Add(1, Vector2Int.down);
            allDirections.Add(2, Vector2Int.left);
            allDirections.Add(3, Vector2Int.right);

            wallLayer = LayerMask.NameToLayer("Wall");
        }

        private void ResetPosition() {
            _transform.position = startPosition;
        }

        private void CheckPossibleTurns() {
            Vector2 playerPosition = _transform.position;
            
            RaycastHit2D upHitOne = Physics2D.Raycast(playerPosition + _almostUpLeft, Vector2.up, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D upHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Vector2.up, WALL_CHECK_DISTANCE, wallLayer);

            RaycastHit2D downHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Vector2.down, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D downHitTwo = Physics2D.Raycast(playerPosition + _almostDownRight, Vector2.down, WALL_CHECK_DISTANCE, wallLayer);

            RaycastHit2D leftHitOne = Physics2D.Raycast(playerPosition + _almostDownLeft, Vector2.left, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D leftHitTwo = Physics2D.Raycast(playerPosition + _almostUpLeft, Vector2.left, WALL_CHECK_DISTANCE, wallLayer);
            
            RaycastHit2D rightHitOne = Physics2D.Raycast(playerPosition + _almostDownRight, Vector2.right, WALL_CHECK_DISTANCE, wallLayer);
            RaycastHit2D rightHitTwo = Physics2D.Raycast(playerPosition + _almostUpRight, Vector2.right, WALL_CHECK_DISTANCE, wallLayer);
            
            possibleTurns[0] = !(upHitOne.collider || upHitTwo.collider);
            possibleTurns[1] = !(downHitOne.collider || downHitTwo.collider);
            possibleTurns[2] = !(leftHitOne.collider || leftHitTwo.collider);
            possibleTurns[3] = !(rightHitOne.collider || rightHitTwo.collider);
            UpdatePossibleTurnsAmount();
        }
        
        private void UpdatePossibleTurnsAmount() {
            _possibleTurnsAmount = 0;
            foreach (bool isTurnValid in possibleTurns) {
                if (isTurnValid) {
                    _possibleTurnsAmount++;
                }
            }
        }

        protected virtual void FixedUpdate() {
            CheckPossibleTurns();
        }
    }
}


