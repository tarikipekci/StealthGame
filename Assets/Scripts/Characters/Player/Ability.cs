using UnityEngine;
namespace Characters.Player
{
    public abstract class Ability : MonoBehaviour
    {
        public string abilityName;
        public float cooldown;

        protected PlayerController playerController;

        public abstract void Activate();
        
        public abstract void Deactivate();
    }
}