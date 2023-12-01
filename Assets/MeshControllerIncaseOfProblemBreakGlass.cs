using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshControllerInCaseOfProblemBreakGlass : MonoBehaviour
{
    [Range(1.5f, 5f)]
    public float radius = 1f;

    [Range(0.005f, 0.05f)]
    public float deformationStrength = .02f;

    private Mesh mesh;
    private Vector3[] vertices, modifiedVerts;
    private float minX, maxX, minZ, maxZ;

    public float weight = 0.0f;
    public float density = 1.0f;
    public BottleController bottleController;

    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        vertices = mesh.vertices;
        modifiedVerts = mesh.vertices;

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

    void RecalculteMesh(){
        mesh.vertices = modifiedVerts;
        GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 localHitPoint = transform.InverseTransformPoint(hit.point);
            float volume = 0;

            for (int v = 0; v < modifiedVerts.Length; v++)
            {
                Vector3 distance = modifiedVerts[v] - localHitPoint;
                float smooothingFactor = 2f;
                float force = deformationStrength / (1f + localHitPoint.sqrMagnitude);

                if (distance.sqrMagnitude < radius)
                {
                    if (!IsEdgeVertex(v))
                    {
                        if(Input.GetMouseButton(0)){
                            modifiedVerts[v] = modifiedVerts[v] + (Vector3.up * force) / smooothingFactor;
                        } else if(Input.GetMouseButton(1)){
                            modifiedVerts[v] = modifiedVerts[v] + (Vector3.down * force) / smooothingFactor;
                        }
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

            Debug.Log("Volume: " + volume);
            weight = volume * density;
        } 

        RecalculteMesh();
    }
}
