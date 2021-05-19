using UnityEngine;

namespace UnitMan.source {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Actor : MonoBehaviour {
        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rigidBody;
        
        protected virtual void Awake() {
            boxCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
        }
        
    }
}


