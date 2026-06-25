using UnityEngine;

public class enemyVision : MonoBehaviour
{
    // How far the enemy can see and how wide the field of view is.
    [SerializeField] float viewDistance = 100f;
    [SerializeField] float viewAngle = 60f;
    // More segments makes the cone smoother, but costs a bit more to rebuild.
    [SerializeField] int coneSegments = 24;
    // Layers that block line of sight and clip the cone shape.
    [SerializeField] LayerMask obstacleMask;
    // Base color for the cone. The alpha controls how transparent it looks.
    static float transparency = 0.12f;
    static Color yellow = new Color(1f, 1f, 0f, transparency);
    static Color red = new Color(1f, 0f, 0f, transparency);
    [SerializeField] Color coneColor = yellow;

    t1enemyMovement enemyMovement;
    Mesh coneMesh;
    MeshFilter coneMeshFilter;
    MeshRenderer coneMeshRenderer;
    Material coneRuntimeMaterial;

    void Awake()
    {
        // Find the movement script so we can alert the enemy when the player is seen.
        enemyMovement = GetComponentInParent<t1enemyMovement>();
        CreateConeObject();
    }

    void LateUpdate()
    {
        // Rebuild the visible cone every frame so it follows enemy rotation and wall hits.
        UpdateConeMesh();
        // Check whether the player is inside the cone and not blocked by walls.
        if (DetectPlayer())
        {
            coneColor = red;
        } else
        {
            coneColor = yellow;
        }
    }

    void CreateConeObject()
    {
        // Build a child object so the vision cone does not need special prefab setup.
        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform, false);
        // Nudge the cone slightly forward so it renders in front of the enemy sprite.
        coneObject.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        coneObject.transform.localRotation = Quaternion.identity;
        coneObject.transform.localScale = Vector3.one;

        coneMeshFilter = coneObject.AddComponent<MeshFilter>();
        coneMeshRenderer = coneObject.AddComponent<MeshRenderer>();
        // Draw the cone above most world sprites.
        coneMeshRenderer.sortingOrder = 1000;

        // Use a simple unlit shader so the cone stays bright and readable.
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        coneRuntimeMaterial = new Material(shader);
        coneRuntimeMaterial.color = coneColor;
        coneMeshRenderer.material = coneRuntimeMaterial;

        // Allocate the mesh once, then update its vertices each frame.
        coneMesh = new Mesh();
        coneMesh.name = "Enemy Vision Cone";
        coneMeshFilter.mesh = coneMesh;
    }

    void UpdateConeMesh()
    {
        if (coneMesh == null)
        {
            return;
        }

        // Build the cone from a fan of triangles centered on the enemy origin.
        int segmentCount = Mathf.Max(2, coneSegments);
        float halfAngle = viewAngle * 0.5f;
        float angleStep = viewAngle / segmentCount;

        Vector3[] vertices = new Vector3[segmentCount + 2];
        int[] triangles = new int[segmentCount * 6];

        // The first vertex is always the enemy position.
        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = -halfAngle + angleStep * i;
            Vector3 localDirection = Quaternion.Euler(0f, 0f, angle) * Vector3.right;
            Vector3 worldDirection = transform.TransformDirection(localDirection);

            // Cast toward this segment so walls clip the visible cone shape.
            LayerMask blockingMask = obstacleMask.value == 0 ? Physics2D.DefaultRaycastLayers : obstacleMask;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, worldDirection, viewDistance, blockingMask);
            float segmentDistance = hit.collider != null ? hit.distance : viewDistance;

            vertices[i + 1] = localDirection * segmentDistance;

            if (i < segmentCount)
            {
                int triangleIndex = i * 6;

                // Front face.
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i + 2;

                // Back face, so the cone stays visible from both sides.
                triangles[triangleIndex + 3] = 0;
                triangles[triangleIndex + 4] = i + 2;
                triangles[triangleIndex + 5] = i + 1;
            }
        }

        coneMesh.Clear();
        coneMesh.vertices = vertices;
        coneMesh.triangles = triangles;
        coneMesh.RecalculateBounds();
        coneRuntimeMaterial.color = coneColor;
    }

    bool DetectPlayer() // returns whether or not the player is visible and alerts the movement script
    {
        if (enemyMovement == null)
        {
            return false;
        }

        // Search nearby colliders first so we only test likely targets.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewDistance);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (!hit.CompareTag("Player"))
            {
                continue;
            }

            // Make sure the player is inside the angle of the cone.
            Vector2 toPlayer = (Vector2)hit.transform.position - (Vector2)transform.position;
            float angleToPlayer = Vector2.Angle(transform.right, toPlayer);

            if (angleToPlayer > viewAngle * 0.5f)
            {
                continue;
            }

            // If a wall blocks the line to the player, the enemy should not see them.
            LayerMask blockingMask = obstacleMask.value == 0 ? Physics2D.DefaultRaycastLayers : obstacleMask;
            RaycastHit2D lineOfSight = Physics2D.Linecast(transform.position, hit.transform.position, blockingMask);
            if (lineOfSight.collider != null)
            {
                continue;
            }

            // The player is visible, so alert the enemy AI.
            enemyMovement.Alert();
            return true;
        }
        // the for loop finished and player was not detected in fov
        return false;
    }
}
