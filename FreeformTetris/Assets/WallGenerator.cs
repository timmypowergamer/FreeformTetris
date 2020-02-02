using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    static Mesh mesh = null;

    MeshFilter mf;
    float totalWidth = 20;
    float width = 15;
    float height = 5;
    float depth = 1f;
    float minRadius = 0.25f;
    float maxRadius = 0.75f;
    int numVertices = 32;
    float checkDiameter = 0.1f;

	private float lastComputedScore = 0;
	private List<Vector3> checkCenters;

    [SerializeField] private Color color;
	[SerializeField] private ScoreboardScript Scoreboard;
	[SerializeField] private bool DrawHitboxGizmos;

	public bool IsWinningPlayer = false;
	public PlayerController Owner;

    // Start is called before the first frame update
    async void Start()
    {
        var triggerVolume = GetComponent<Collider>();
        triggerVolume.bounds.size.Set(20, 5, 1);
        if (mesh == null)
        {
            var holeVertices = HoleGeometry.Create(width, height, minRadius, maxRadius, numVertices);
            int numHoleVertices = holeVertices.Length;

            var vertices = new Vector3[2 * numHoleVertices + 8];
            var uv = new Vector2[2 * numHoleVertices + 8];
            var triangles = new int[][] { new int[3 * 4 * numVertices + 3 * 4], new int[3 * 2 * numVertices] };
            for (int i = 0; i < numHoleVertices; i += 2)
            {
                int hv0 = i, hv1 = i + 1, hv2 = (i + 2) % numHoleVertices, hv3 = (i + 3) % numHoleVertices;
                int v0 = hv0, v1 = hv1, v2 = hv2, v3 = hv3;
                int v0p = v0 + numHoleVertices, v1p = v1 + numHoleVertices, v2p = v2 + numHoleVertices, v3p = v3 + numHoleVertices;

                vertices[v0] = new Vector3(holeVertices[hv0].x, holeVertices[hv0].y);
                vertices[v1] = new Vector3(holeVertices[hv1].x, holeVertices[hv1].y);
                vertices[v0p] = new Vector3(holeVertices[hv0].x, holeVertices[hv0].y, depth);
                vertices[v1p] = new Vector3(holeVertices[hv1].x, holeVertices[hv1].y, depth);

                uv[v0] = new Vector2(holeVertices[hv0].x / totalWidth + 0.5f, holeVertices[hv0].y / height + 0.5f);
                uv[v1] = new Vector2(holeVertices[hv1].x / totalWidth + 0.5f, holeVertices[hv1].y / height + 0.5f);
                uv[v0p] = new Vector2(holeVertices[hv0].x / totalWidth + 0.5f, holeVertices[hv0].y / height + 0.5f);
                uv[v1p] = new Vector2(holeVertices[hv1].x / totalWidth + 0.5f, holeVertices[hv1].y / height + 0.5f);

                triangles[0][6 * i] = v2;
                triangles[0][6 * i + 1] = v1;
                triangles[0][6 * i + 2] = v0;

                triangles[0][6 * i + 3] = v1;
                triangles[0][6 * i + 4] = v2;
                triangles[0][6 * i + 5] = v3;

                triangles[0][6 * i + 6] = v0p;
                triangles[0][6 * i + 7] = v1p;
                triangles[0][6 * i + 8] = v2p;

                triangles[0][6 * i + 9] = v3p;
                triangles[0][6 * i + 10] = v2p;
                triangles[0][6 * i + 11] = v1p;

                triangles[1][3 * i] = v1;
                triangles[1][3 * i + 1] = v3p;
                triangles[1][3 * i + 2] = v1p;

                triangles[1][3 * i + 3] = v3;
                triangles[1][3 * i + 4] = v3p;
                triangles[1][3 * i + 5] = v1;
            }

            vertices[2 * numHoleVertices] = new Vector3(-totalWidth / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 1] = new Vector3(-width / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 2] = new Vector3(-width / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 3] = new Vector3(-totalWidth / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 4] = new Vector3(totalWidth / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 5] = new Vector3(width / 2.0f, height / 2.0f);
            vertices[2 * numHoleVertices + 6] = new Vector3(width / 2.0f, -height / 2.0f);
            vertices[2 * numHoleVertices + 7] = new Vector3(totalWidth / 2.0f, -height / 2.0f);

            uv[2 * numHoleVertices] = new Vector2(0, 1);
            uv[2 * numHoleVertices + 1] = new Vector2(0.5f - width / 2.0f / totalWidth, 1);
            uv[2 * numHoleVertices + 2] = new Vector2(0.5f - width / 2.0f / totalWidth, 0);
            uv[2 * numHoleVertices + 3] = new Vector2(0, 0);
            uv[2 * numHoleVertices + 4] = new Vector2(1, 1);
            uv[2 * numHoleVertices + 5] = new Vector2(0.5f + width / 2.0f / totalWidth, 1);
            uv[2 * numHoleVertices + 6] = new Vector2(0.5f + width / 2.0f / totalWidth, 0);
            uv[2 * numHoleVertices + 7] = new Vector2(1, 0);

            triangles[0][3 * 4 * numVertices] = 2 * numHoleVertices;
            triangles[0][3 * 4 * numVertices + 1] = 2 * numHoleVertices + 1;
            triangles[0][3 * 4 * numVertices + 2] = 2 * numHoleVertices + 2;

            triangles[0][3 * 4 * numVertices + 3] = 2 * numHoleVertices + 2;
            triangles[0][3 * 4 * numVertices + 4] = 2 * numHoleVertices + 3;
            triangles[0][3 * 4 * numVertices + 5] = 2 * numHoleVertices;

            triangles[0][3 * 4 * numVertices + 6] = 2 * numHoleVertices + 4;
            triangles[0][3 * 4 * numVertices + 7] = 2 * numHoleVertices + 6;
            triangles[0][3 * 4 * numVertices + 8] = 2 * numHoleVertices + 5;

            triangles[0][3 * 4 * numVertices + 9] = 2 * numHoleVertices + 6;
            triangles[0][3 * 4 * numVertices + 10] = 2 * numHoleVertices + 4;
            triangles[0][3 * 4 * numVertices + 11] = 2 * numHoleVertices + 7;

            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.subMeshCount = 2;
            mesh.SetTriangles(triangles[0], 0);
            mesh.SetTriangles(triangles[1], 1);
            mesh.uv = uv;
            mesh.RecalculateNormals();
        }

        mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;
	}

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(new Vector3(0, 0, depth / 2), new Vector3(totalWidth, height, depth));
        Gizmos.DrawWireCube(new Vector3(0, 0, depth / 2), new Vector3(width, height, depth));
		if (DrawHitboxGizmos)
		{
			computeCoverage("Objects", DrawHitboxGizmos);
		}
	}

	public void UpdateScoreboard()
	{
		lastComputedScore = computeScore();
		Scoreboard.score = Mathf.RoundToInt(lastComputedScore * 100f);
	}

	public void SetOwner(PlayerController newOwner)
	{
		Owner = newOwner;
		if (newOwner != null)
		{
			newOwner.SetWall(this);
		}
	}

    public void SetColor(Color color)
    {
        this.color = color;
        this.Scoreboard.GetComponent<MeshRenderer>().material.color = color;
    }

    float computeScore()
	{
		return computeCoverage("Objects", false);
	}

	float computeCoverage(string layer, bool drawGizmos)
	{
		if (checkCenters == null) computeInitialCoverage("Default");
		var localHalfExtents = new Vector3(0.5f * checkDiameter, 0.5f * checkDiameter, 0.6f * depth);
		var objectsMask = LayerMask.GetMask(layer);

		int count = 0;
		foreach(Vector3 globalCenter in checkCenters)
		{
			if (drawGizmos) Gizmos.color = Color.white;
			if (Physics.CheckBox(globalCenter, localHalfExtents, transform.rotation, objectsMask))
			{
				count++;
				if (drawGizmos) Gizmos.color = Color.red;
			}
			if (drawGizmos) Gizmos.DrawWireCube(transform.InverseTransformPoint(globalCenter), localHalfExtents * 2);
		}

		return (float)count / checkCenters.Count;
	}

    void computeInitialCoverage(string layer)
    {
		checkCenters = new List<Vector3>();

        int numChecksHoriz = Mathf.FloorToInt(width / checkDiameter);
        int numChecksVert = Mathf.FloorToInt(height / checkDiameter);


        var localHalfExtents = new Vector3(0.5f * checkDiameter, 0.5f * checkDiameter, 0.6f * depth);
        float zLocal = 0.5f * depth;
        var objectsMask = LayerMask.GetMask(layer);

        for (int row = 0; row < numChecksVert; row++)
        {
            float yLocal = (row + 0.5f) * checkDiameter - 0.5f * height;
            for (int col = 0; col < numChecksHoriz; col++)
            {
                float xLocal = (col + 0.5f) * checkDiameter - 0.5f * width;
                var localCenter = new Vector3(xLocal, yLocal, zLocal);
                var globalCenter = transform.TransformPoint(localCenter);

				if (!Physics.CheckBox(globalCenter, localHalfExtents, transform.rotation, objectsMask))
                {
					checkCenters.Add(globalCenter);
				}
            }
        }
    }

	public float GetScore()
	{
		return lastComputedScore;
	}
}
