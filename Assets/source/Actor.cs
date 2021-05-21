using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class Actor : MonoBehaviour {
        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rigidBody;
        protected Transform thisTransform;
        protected GameObject thisGameObject;
        public Vector2 motion = Vector2.zero;
        
        protected virtual void Awake() {
            boxCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
            thisTransform = transform;
            thisGameObject = gameObject;
        }
        
    }
    
}


