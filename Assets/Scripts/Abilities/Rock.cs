using Characters.Player;
using UnityEngine;

namespace Abilities
{
    public class Rock : Ability
    {
        [Header("Line Renderer Settings")] [SerializeField]
        private LineRenderer lr;

        [SerializeField] private float throwForce;
        [SerializeField] private Transform throwPoint;
        [SerializeField] private int resolution;
        [SerializeField] private Camera mainCamera;

        private bool isLineVisible;
        private bool isRockThrown;
        private Vector3 fixedEndPosition; 

        [Header("Throw Object Settings")] [SerializeField]
        private GameObject rockPrefab;

        private GameObject currentThrownObject;
        private Rigidbody2D rb;

        public override void Activate()
        {
            isLineVisible = true;
            lr.enabled = true;
        }

        public override void Deactivate()
        {
            isLineVisible = false;
            lr.positionCount = 0;
            lr.enabled = false;
            playerController.lastAbilityIndex = -1;
        }

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            currentThrownObject = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            rb = currentThrownObject.GetComponent<Rigidbody2D>();
            currentThrownObject.SetActive(false);
            lr.enabled = false;
        }

        private void Update()
        {
            DrawTrajectory();

            if (Input.GetMouseButtonDown(0) && isLineVisible)
            {
                ThrowRock();
            }

            if (isRockThrown)
            {
                CheckIfRockReachedEndPosition();
            }
        }

        private void DrawTrajectory()
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - throwPoint.position).normalized;
            float velocity = throwForce;

            Vector2[] points = CalculateTrajectoryPoints(throwPoint.position, direction, velocity);
            lr.positionCount = points.Length;

            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i]);
            }
        }

        private Vector2[] CalculateTrajectoryPoints(Vector2 startPosition, Vector2 direction, float velocity)
        {
            Vector2[] points = new Vector2[resolution];
            const float timeStep = 0.1f;
            Vector2 gravity = Physics2D.gravity;

            for (int i = 0; i < resolution; i++)
            {
                float t = i * timeStep;
                Vector2 position = startPosition + direction * (velocity * t) + gravity * (0.5f * t * t);
                points[i] = position;
            }

            return points;
        }

        private void ThrowRock()
        {
            currentThrownObject.SetActive(true);
            currentThrownObject.transform.position = throwPoint.position;

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - throwPoint.position).normalized;

            rb.velocity = direction * throwForce;
            isRockThrown = true;

            fixedEndPosition = lr.GetPosition(lr.positionCount - 1);
            Deactivate();
        }

        private void CheckIfRockReachedEndPosition()
        {
            if (Vector2.Distance(fixedEndPosition, currentThrownObject.transform.position) < 0.2f)
            {
                rb.velocity = Vector2.zero;
                rb.Sleep();
                isRockThrown = false; 
            }
        }
    }
}
