
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HoleGeometry
{
	public static Vector2[] Create(float maxWidth, float maxHeight, float minRadius, float maxRadius, int numVertices)
	{
		var result = new Vector2[2 * numVertices];
		
		int numQuadrantVertices = (int)Mathf.CeilToInt(numVertices / 4.0f);
		
		float quadrantAngle = 2.0f * Mathf.Atan(maxHeight / maxWidth);
		float stepAngle = quadrantAngle / numQuadrantVertices;

		float curAngle = Mathf.Atan(maxHeight / maxWidth);
		for (int i = 0; i < numQuadrantVertices; i++)
		{
			float x = maxWidth / 2.0f;
			float y = x * Mathf.Tan(curAngle);
			
			result[2*i] = new Vector2(x, y);
            float radius = Random.Range(minRadius, maxRadius);
            result[2*i + 1] = new Vector2(radius * x, radius * y);
			
			curAngle -= stepAngle;
		}

		quadrantAngle = 2.0f * Mathf.Atan(maxWidth / maxHeight);
		stepAngle = quadrantAngle / numQuadrantVertices;
		for (int i = numQuadrantVertices; i < 2 * numQuadrantVertices; i++)
		{
			float y = -maxHeight / 2.0f;
			float x = y / Mathf.Tan(curAngle);
			
			result[2*i] = new Vector2(x, y);
            float radius = Random.Range(minRadius, maxRadius);
            result[2*i + 1] = new Vector2(radius * x, radius * y);
			
			curAngle -= stepAngle;
		}

		quadrantAngle = 2.0f * Mathf.Atan(maxHeight / maxWidth);
		stepAngle = quadrantAngle / numQuadrantVertices;
		for (int i = 2 * numQuadrantVertices; i < 3 * numQuadrantVertices; i++)
		{
			float x = -maxWidth / 2.0f;
			float y = x * Mathf.Tan(curAngle);
			
			result[2*i] = new Vector2(x, y);
            float radius = Random.Range(minRadius, maxRadius);
            result[2*i + 1] = new Vector2(radius * x, radius * y);
			
			curAngle -= stepAngle;
		}

		quadrantAngle = 2.0f * Mathf.Atan(maxWidth / maxHeight);
		stepAngle = quadrantAngle / numQuadrantVertices;
		for (int i = 3 * numQuadrantVertices; i < 4 * numQuadrantVertices; i++)
		{
			float y = maxHeight / 2.0f;
			float x = y / Mathf.Tan(curAngle);
			
			result[2*i] = new Vector2(x, y);
            float radius = Random.Range(minRadius, maxRadius);
            result[2*i + 1] = new Vector2(radius * x, radius * y);
			
			curAngle -= stepAngle;
		}
		
		return result;
	}
}
