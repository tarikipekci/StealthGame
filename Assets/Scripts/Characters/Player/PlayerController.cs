using System.Collections.Generic;
using Components;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Player player;
        private Movement movementComponent;

        private readonly Dictionary<KeyCode, int> abilities = new()
        {
            { KeyCode.Alpha1, 0 },
            { KeyCode.Alpha2, 1 },
            { KeyCode.Alpha3, 2 },
            { KeyCode.Alpha4, 3 }
        };

        public int lastAbilityIndex;

        private void Awake()
        {
            player = GetComponent<Player>();

            if (player == null)
            {
                Debug.LogError("Player is missing!");
                enabled = false;
            }
        }

        private void Start()
        {
            movementComponent = player.GetMovementComponent();
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            bool isSprinting = movementComponent.GetIsSprinting();
            bool sprintInput = Input.GetKey(KeyCode.LeftShift);

            if (sprintInput != isSprinting)
            {
                movementComponent.SetIsSprinting(sprintInput);
            }
            
            foreach (var input in abilities)
            {
                if (Input.GetKeyDown(input.Key))
                {
                    UseAbility(input.Value);
                }
            }
            
            Vector2 direction = new Vector2(horizontal, vertical).normalized;
            player.Move(direction);
        }
        
        void UseAbility(int index)
        {
            if (index == lastAbilityIndex)
            {
                player.abilities[index].Deactivate();
                return;
            }
            
            if (index < player.abilities.Count)
            {
                lastAbilityIndex = index;
                player.abilities[index].Activate();
            }
        }
    }
}