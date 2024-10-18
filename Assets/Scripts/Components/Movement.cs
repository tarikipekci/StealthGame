using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed;

        [SerializeField] private float sprintSpeed;

        private bool isSprinting;
        
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 direction)
        {
            if (!isSprinting)
            {
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                rb.velocity = direction * sprintSpeed;
            }
        }

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }

        public bool GetIsSprinting()
        {
            return isSprinting;
        }
        
        public void SetIsSprinting(bool newIsSprinting)
        {
            isSprinting = newIsSprinting;
        }
    }
}