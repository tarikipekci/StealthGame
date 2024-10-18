using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

namespace Characters
{
    public class Enemy : Character
    {
        private Rigidbody2D rb;
        private Node behaviorTree;
        public Transform[] waypoints;

        private int currentWaypointIndex;
        [SerializeField] private float waitTime;
        private bool isWaiting;
        [SerializeField] private float targetThreshold;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
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
            //if (fov.visibleTargets.Count > 0)
            {
                // return Node.NodeState.Success;
            }

            return Node.NodeState.Failure;
        }

        private Node.NodeState ChasePlayer()
        {
            return Node.NodeState.Running;
        }

        private Node.NodeState Patrol()
        {
            if (!isWaiting)
            {
                Transform targetWaypoint = waypoints[currentWaypointIndex];
                float distance = Vector2.Distance(transform.position, targetWaypoint.position);

                if (distance <= targetThreshold)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                    isWaiting = true;
                    rb.velocity = Vector2.zero;
                    return Node.NodeState.Success;
                }

                Vector2 direction = (targetWaypoint.position - transform.position).normalized;
                rb.velocity = direction * movement.GetMoveSpeed();

                return Node.NodeState.Running;
            }

            return Node.NodeState.Success;
        }

        private Node.NodeState Wait()
        {
            var targetWaypoint = waypoints[currentWaypointIndex];
            Vector2 direction = (targetWaypoint.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 150f * Time.deltaTime);

            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDifference < 1f)
            {
                transform.rotation = targetRotation;
                isWaiting = false;
                return Node.NodeState.Success;
            }

            return Node.NodeState.Running;
        }
    }
}