using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class VisionCone : MonoBehaviour
    {
        [Range(1, 20)]
        public float viewRadius = 5f;
        [Range(0, 360)] public float viewAngle = 60f;

        public LayerMask targetMask;
        public LayerMask obstacleMask;

        public List<Transform> visibleTargets;

        [SerializeField] private int resolution = 10;
        public Material viewMaterial;

        private Mesh viewMesh;
        private MeshRenderer meshRenderer;
        private Coroutine colorChangeCoroutine;
        private Color originalColor;

        private void Start()
        {
            viewMesh = new Mesh();
            GameObject viewMeshObject = new GameObject("ViewMesh")
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity
                }
            };

            meshRenderer = viewMeshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = viewMaterial;
            meshRenderer.sortingOrder = -1;

            MeshFilter meshFilter = viewMeshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = viewMesh;

            // Set the original color with transparency
            meshRenderer.material.color = new Color(0f, 1f, 0f, 0.6f);
            originalColor = meshRenderer.material.color;

            StartCoroutine(nameof(FindTargetsWithDelay), 0.2f);
        }

        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void Update()
        {
            DrawFieldOfView();
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            foreach (var targetCollider in targetsInViewRadius)
            {
                Transform target = targetCollider.transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;

                if (Vector2.Angle(transform.right, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }

            if (visibleTargets.Count > 0)
            {
                // Target is visible, change to red
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(ChangeColor(Color.red));
            }
            else
            {
                // No target visible, revert to the original color
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(ChangeColor(originalColor));
            }
        }

        private IEnumerator ChangeColor(Color targetColor)
        {
            const float duration = 1f;
            Color startColor = meshRenderer.material.color;
            float time = 0;

            while (time < duration)
            {
                meshRenderer.material.color = Color.Lerp(startColor, targetColor, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            meshRenderer.material.color = targetColor;
        }

        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * resolution / 360);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo viewCast = ViewCast(angle);
                viewPoints.Add(viewCast.point);
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            uvs[0] = new Vector2(0.5f, 0);

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                float uvX = (float)i / (vertexCount - 2);
                uvs[i + 1] = new Vector2(uvX, 1);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.uv = uvs;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
            viewMesh.RecalculateBounds();
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector2 direction = DirFromAngle(globalAngle, true);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewRadius, obstacleMask);

            if (hit)
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }

            return new ViewCastInfo(false, (Vector2)transform.position + direction * viewRadius, viewRadius,
                globalAngle);
        }

        private Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.z;
            }

            return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
        }

        private struct ViewCastInfo
        {
            public bool hit;
            public readonly Vector2 point;
            public float distance;
            public float angle;

            public ViewCastInfo(bool _hit, Vector2 _point, float _distance, float _angle)
            {
                hit = _hit;
                point = _point;
                distance = _distance;
                angle = _angle;
            }
        }
    }
}
