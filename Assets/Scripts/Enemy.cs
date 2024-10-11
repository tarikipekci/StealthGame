using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class Enemy : Character
{
    private Rigidbody2D rb;
    private FieldOfView fov;
    private Node behaviorTree;
    public Transform[] waypoints;

    private int currentWaypointIndex;
    private float waitCounter;
    private bool waiting;
    [SerializeField] private float targetThreshold;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
        behaviorTree = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new ActionNode(CheckForPlayer),
                new ActionNode(ChasePlayer)
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(Patrol),
                new ActionNode(Wait)
            })
        });
    }

    private void Update()
    {
        behaviorTree.Execute();
    }

    private Node.NodeState CheckForPlayer()
    {
        if (fov.visibleTargets.Count > 0)
        {
            return Node.NodeState.Success;
        }

        return Node.NodeState.Failure;
    }

    private Node.NodeState ChasePlayer()
    {
        return Node.NodeState.Running;
    }

    private Node.NodeState Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float distance = Vector2.Distance(transform.position, targetWaypoint.position);

        if (distance <= targetThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            Vector2 direction = (targetWaypoint.position - transform.position).normalized;
            rb.velocity = direction * movement.GetMoveSpeed();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        
        return Node.NodeState.Running;
    }


    private Node.NodeState Wait()
    {
        return Node.NodeState.Running;
    }
}