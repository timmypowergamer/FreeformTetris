﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    static Mesh mesh = null;

    MeshFilter mf;
    public float totalWidth = 20;
    public float width = 5;
    public float height = 5;
    public float depth = 1;
    public float minRadius = 0.25f;
    public float maxRadius = 0.75f;
    public int numVertices = 16;
    public float checkDiameter = 0.1f;
	
	float holeCoverage;

    // Start is called before the first frame update
    void Start()
    {
        var triggerVolume = GetComponent<Collider>();
        triggerVolume.bounds.size.Set(20, 5, 1);
        if (mesh == null)
        {
            var holeVertices = HoleGeometry.Create(width, height, minRadius, maxRadius, numVertices);
            int numHoleVertices = holeVertices.Length;

            var vertices = new Vector3[2 * numHoleVertices + 8];
            var triangles = new int[3 * 6 * numVertices + 3 * 4];
            for (int i = 0; i < numHoleVertices; i += 2)
            {
                int hv0 = i, hv1 = i + 1, hv2 = (i + 2) % numHoleVertices, hv3 = (i + 3) % numHoleVertices;
                int v0 = hv0, v1 = hv1, v2 = hv2, v3 = hv3;
                int v0p = v0 + numHoleVertices, v1p = v1 + numHoleVertices, v2p = v2 + numHoleVertices, v3p = v3 + numHoleVertices;

                vertices[v0] = new Vector3(holeVertices[hv0].x, holeVertices[hv0].y);
                vertices[v1] = new Vector3(holeVertices[hv1].x, holeVertices[hv1].y);
                vertices[v0p] = new Vector3(holeVertices[hv0].x, holeVertices[hv0].y, depth);
                vertices[v1p] = new Vector3(holeVertices[hv1].x, holeVertices[hv1].y, depth);

                triangles[9 * i] = v2;
                triangles[9 * i + 1] = v1;
                triangles[9 * i + 2] = v0;

                triangles[9 * i + 3] = v1;
                triangles[9 * i + 4] = v2;
                triangles[9 * i + 5] = v3;

                triangles[9 * i + 6] = v0p;
                triangles[9 * i + 7] = v1p;
                triangles[9 * i + 8] = v2p;

                triangles[9 * i + 9] = v3p;
                triangles[9 * i + 10] = v2p;
                triangles[9 * i + 11] = v1p;

                triangles[9 * i + 12] = v1;
                triangles[9 * i + 13] = v3p;
                triangles[9 * i + 14] = v1p;

                triangles[9 * i + 15] = v3;
                triangles[9 * i + 16] = v3p;
                triangles[9 * i + 17] = v1;
            }

            vertices[2 * numHoleVertices] = new Vector3(-totalWidth / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 1] = new Vector3(-width / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 2] = new Vector3(-width / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 3] = new Vector3(-totalWidth / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 4] = new Vector3(totalWidth / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 5] = new Vector3(width / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 6] = new Vector3(width / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 7] = new Vector3(totalWidth / 2.0f, -height / 2.0f);

            triangles[3 * 6 * numVertices] = 2 * numHoleVertices;
            triangles[3 * 6 * numVertices + 1] = 2 * numHoleVertices + 1;
            triangles[3 * 6 * numVertices + 2] = 2 * numHoleVertices + 2;

            triangles[3 * 6 * numVertices + 3] = 2 * numHoleVertices + 2;
            triangles[3 * 6 * numVertices + 4] = 2 * numHoleVertices + 3;
            triangles[3 * 6 * numVertices + 5] = 2 * numHoleVertices;

            triangles[3 * 6 * numVertices + 6] = 2 * numHoleVertices + 4;
            triangles[3 * 6 * numVertices + 7] = 2 * numHoleVertices + 6;
            triangles[3 * 6 * numVertices + 8] = 2 * numHoleVertices + 5;

            triangles[3 * 6 * numVertices + 9] = 2 * numHoleVertices + 6;
            triangles[3 * 6 * numVertices + 10] = 2 * numHoleVertices + 4;
            triangles[3 * 6 * numVertices + 11] = 2 * numHoleVertices + 7;

            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            //mesh.uv = uv;
            mesh.RecalculateNormals();
        }

        mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;
		
		holeCoverage = 1.0f - computeCoverage("Default");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //var score = computeScore();
        //Debug.Log($"{score}");
    }

    private void OnTriggerEnter(Collider other)
    {
        var component = other.GetComponentInParent<GrabbableObject>();
        if(component == null) { return; }
        component.Placed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var component = other.GetComponentInParent<GrabbableObject>();
        if (component == null) { return; }
        component.Placed = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(new Vector3(0, 0, depth / 2), new Vector3(width, height, depth));
        Gizmos.DrawWireCube(new Vector3(0, 0, depth / 2), new Vector3(totalWidth, height, depth));
    }

    float computeScore()
	{
		return computeCoverage("Objects") / holeCoverage;
	}

    float computeCoverage(string layer)
    {
        int numChecksHoriz = Mathf.FloorToInt(width / checkDiameter);
        int numChecksVert = Mathf.FloorToInt(height / checkDiameter);

        var localHalfExtents = new Vector3(0.5f * checkDiameter, 0.5f * checkDiameter, 0.5f * depth);
        var globalHalfExtents = transform.TransformVector(localHalfExtents);
        var quat = Quaternion.LookRotation(transform.forward, transform.up);
        float zLocal = 0.5f * depth;
        var objectsMask = LayerMask.GetMask(layer);

        int count = 0;
        for (int row = 0; row < numChecksVert; row++)
        {
            float yLocal = (row + 0.5f) * checkDiameter - 0.5f * height;
            for (int col = 0; col < numChecksHoriz; col++)
            {
                float xLocal = (col + 0.5f) * checkDiameter - 0.5f * width;
                var localCenter = new Vector3(xLocal, yLocal, zLocal);
                var globalCenter = transform.TransformPoint(localCenter);
                if (Physics.CheckBox(globalCenter, globalHalfExtents, quat, objectsMask))
                {
                    count++;
                }

            }
        }

        return (float)count / numChecksHoriz / numChecksVert;
    }
}
