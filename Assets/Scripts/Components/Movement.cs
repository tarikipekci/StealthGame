using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 direction)
        {
            rb.velocity = direction * moveSpeed;
        }

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }
    }
}