using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();
        
            if (player == null)
            {
                Debug.LogError("Player is missing!");
                enabled = false;
            }
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(horizontal, vertical).normalized;

            player.Move(direction);
        }
    }
}