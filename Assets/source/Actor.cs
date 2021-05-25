using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class Actor : MonoBehaviour {
        protected CircleCollider2D _circleCollider;
        protected Rigidbody2D _rigidBody;
        protected Transform _transform;
        protected GameObject _gameObject;
        public Vector2 motion = Vector2.zero;
        
        protected virtual void Awake() {
            _circleCollider = GetComponent<CircleCollider2D>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _transform = transform;
            _gameObject = gameObject;
        }
        
    }
    
}


