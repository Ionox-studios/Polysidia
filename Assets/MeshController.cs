using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour
{
    [Range(1.5f, 5f)]
    public float radius = 1f;

    [Range(0f, 0.05f)]
    public float deformationStrength = 0.02f;

    public BottleController bottleController; // Assign in the Inspector
    public vacuumController vacuumController; // Assign in the Inspector
    public float density = 1.0f;

    private Mesh mesh;
    private Vector3[] vertices, modifiedVerts;
    private float minX, maxX, minZ, maxZ;
    public float weight = 0.0f;

void Start()
{
    mesh = GetComponentInChildren<MeshFilter>().mesh;
    vertices = mesh.vertices;
    modifiedVerts = (Vector3[])vertices.Clone(); // Clone the vertices array
    CalculateBounds();
}

    void CalculateBounds()
    {
        minX = maxX = vertices[0].x;
        minZ = maxZ = vertices[0].z;

        foreach (Vector3 vertex in vertices)
        {
            if (vertex.x < minX) minX = vertex.x;
            if (vertex.x > maxX) maxX = vertex.x;
            if (vertex.z < minZ) minZ = vertex.z;
            if (vertex.z > maxZ) maxZ = vertex.z;
        }
    }

    bool IsEdgeVertex(int vertexIndex)
    {
        Vector3 vertex = modifiedVerts[vertexIndex];
        return (vertex.x == minX || vertex.x == maxX || vertex.z == minZ || vertex.z == maxZ);
    }
    void RecalculateMesh()
    {
        mesh.vertices = modifiedVerts;
        GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }
    void Update()
    {
        if (bottleController != null)
        {
            ApplyBottleDeformation();
        }

        if (vacuumController != null && vacuumController.IsSucking())
        {
            ApplyVacuumDeformation();
        }

        RecalculateMesh();
    }
    void ApplyBottleDeformation()
    {
        if (bottleController != null)
        {
             Vector3 worldEmissionPoint = bottleController.GetEmissionPoint();
             Vector3 localEmissionPoint = transform.InverseTransformPoint(worldEmissionPoint);
             float middleZ = GetMiddleZ();
            localEmissionPoint.z = new Vector3(0, 0, middleZ).z;
            float middleY = GetMiddleY();
            localEmissionPoint.y = new Vector3(0, middleY, 0).y;

            float pourIntensity = bottleController.GetPourIntensity();
            //Debug.Log("Pour intensity: " + pourIntensity);
            //Debug.Log("Emission point: " + localEmissionPoint);
            // Transform the emission point to local space of the mesh


            ApplyDeformation(localEmissionPoint, pourIntensity);
        }

    }
void ApplyVacuumDeformation()
{
    //Run this through apply deformation to make it more gradual later
    Vector3 suctionPoint = vacuumController.GetSuctionPoint();
    float middleZ = GetMiddleZ();

    // Adjust the suction point's Z-coordinate to match the middle Z of the mesh
    suctionPoint.z = transform.TransformPoint(new Vector3(0, 0, middleZ)).z;

    if (vacuumController.isSucking)
    {
        for (int v = 0; v < modifiedVerts.Length; v++)
        {
            if (!IsEdgeVertex(v))
            {
                Vector3 worldVertex = transform.TransformPoint(modifiedVerts[v]);
                
                // Check if the vertex is within a certain distance in the X-Y plane from the suction point
                float distanceXY = Vector2.Distance(new Vector2(worldVertex.x, worldVertex.y), new Vector2(suctionPoint.x, suctionPoint.y));


                if (distanceXY <= vacuumController.suctionRadius)  // Check if within suction radius in X-Y plane
                {
                    float suctionEffect = vacuumController.CalculateSuctionEffect(worldVertex);
                    Debug.Log("Suction effect: " + suctionEffect);
                    Vector3 directionToSuction = (suctionPoint - worldVertex).normalized;
                    directionToSuction = transform.InverseTransformDirection(directionToSuction);
                    directionToSuction.y = Mathf.Min(directionToSuction.y, 0);
                    Debug.Log("Direction to suction: " + directionToSuction);
                    Debug.Log("Vertex: " + Time.deltaTime); 
                    modifiedVerts[v] += directionToSuction * suctionEffect * Time.deltaTime;
                    // set y to be the average of the original and the modified
                    modifiedVerts[v].y = modifiedVerts[v].y*0.8f+ vertices[v].y*0.2f;

                    //modifiedVerts[v].y = Mathf.Min(modifiedVerts[v].y, vertices[v].y);
                    if (modifiedVerts[v].y < 0)
                    {
                        modifiedVerts[v].y = 0;
                    }
                }
            }
        }
    }
}




void ApplyDeformation(Vector3 localEmissionPoint, float intensity)
{
    float volume = 0f;
    float adjustedDeformationStrength = deformationStrength * intensity;

    for (int v = 0; v < modifiedVerts.Length; v++)
    {
        Vector3 distance = modifiedVerts[v] - localEmissionPoint;
        // Consider only X and Z components for radius check
        Vector3 distanceXZ = new Vector3(distance.x, 0f, distance.z);

        if (distanceXZ.sqrMagnitude < radius * radius)
        {
            if (!IsEdgeVertex(v))
            {
                float force = adjustedDeformationStrength / (1f + distanceXZ.sqrMagnitude);
                modifiedVerts[v] += Vector3.up * force;
            }

            if (modifiedVerts[v].y < 0)
            {
                modifiedVerts[v].y = 0;
            }
        }

        if (modifiedVerts[v].y > 0)
        {
            volume += modifiedVerts[v].y;
        }
    }

    //Debug.Log("Volume: " + volume);
    weight = volume * density;
}

    public float GetMiddleZ()
    {
        // Assuming this GameObject's mesh represents its bounds
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null)
        {
            Bounds bounds = meshFilter.mesh.bounds;
            float middleZ = bounds.center.z; // Local Z value
            return middleZ;
        }
        else
        {
            Debug.LogWarning("MeshFilter not found in MeshController or its children.");
            return 0f;
        }
    }
    
    public float GetMiddleY()
    {
        // Assuming this GameObject's mesh represents its bounds
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null)
        {
            Bounds bounds = meshFilter.mesh.bounds;
            float middleY = bounds.center.y; // Local Z value
            return middleY;
        }
        else
        {
            Debug.LogWarning("MeshFilter not found in MeshController or its children.");
            return 0f;
        }
    }
    public void ResetMesh()
{
    modifiedVerts = (Vector3[])vertices.Clone();
    RecalculateMesh();
    weight = 0.0f;
}
    public void SetBottleController(BottleController newBottleController)
    {
        bottleController = newBottleController;
    }
}
