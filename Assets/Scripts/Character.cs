using Components;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Movement movement;

    protected virtual void Awake()
    {
        movement = GetComponent<Movement>();

        if (movement == null)
        {
            Debug.LogError("Movement component is missing!");
            enabled = false;
        }
    }

    public void Move(Vector2 direction)
    {
        movement.Move(direction);
    }
}