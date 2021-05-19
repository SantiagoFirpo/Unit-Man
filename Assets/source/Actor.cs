using UnityEngine;

namespace UnitMan.Source {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Actor : MonoBehaviour {
        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rigidBody;
        protected Transform thisTransform;
        protected GameObject thisGameObject;
        
        protected virtual void Awake() {
            boxCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
        }
        
    }
    
}


