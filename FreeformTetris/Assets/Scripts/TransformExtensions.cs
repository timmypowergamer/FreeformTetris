using UnityEngine;
using System.Collections.Generic;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform transform)
    {
        if (transform == null) return;

		List<Transform> children = new List<Transform>();
		foreach(Transform t in transform)
		{
			children.Add(t);
		}

		foreach(Transform t in children)
		{
			Object.Destroy(t.gameObject);
		}
	}

	public static void SetLayerRecursively(this Transform transform, string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);

		foreach (Transform t in transform)
		{
			t.gameObject.layer = layer;
		}
	}

	public static string PathInHierarchy(this Transform transform)
	{
		if (transform.parent == null) return transform.gameObject.name;
		else return transform.parent.PathInHierarchy() + "/" + transform.gameObject.name;
	}
}

public enum AnchorPresets
{
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottonCenter,
	BottomRight,
	BottomStretch,

	VertStretchLeft,
	VertStretchRight,
	VertStretchCenter,

	HorStretchTop,
	HorStretchMiddle,
	HorStretchBottom,

	StretchAll
}

public enum PivotPresets
{
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottomCenter,
	BottomRight,
}

public static class RectTransformExtensions
{
	public static void CopyFrom(this RectTransform rect, RectTransform otherRect, RectOffset padding = null)
	{
		if (padding == null) padding = new RectOffset(0, 0, 0, 0);
		rect.pivot = otherRect.pivot;
		rect.anchorMin = otherRect.anchorMin;
		rect.anchorMax = otherRect.anchorMax;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, otherRect.rect.width + padding.horizontal);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, otherRect.rect.height + padding.vertical);
	}

	public static void SetAnchor(this RectTransform source, AnchorPresets allign, float offsetX = 0, float offsetY = 0)
	{
		source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

		switch (allign)
		{
			case (AnchorPresets.TopLeft):
				{
					source.anchorMin = new Vector2(0, 1);
					source.anchorMax = new Vector2(0, 1);
					break;
				}
			case (AnchorPresets.TopCenter):
				{
					source.anchorMin = new Vector2(0.5f, 1);
					source.anchorMax = new Vector2(0.5f, 1);
					break;
				}
			case (AnchorPresets.TopRight):
				{
					source.anchorMin = new Vector2(1, 1);
					source.anchorMax = new Vector2(1, 1);
					break;
				}

			case (AnchorPresets.MiddleLeft):
				{
					source.anchorMin = new Vector2(0, 0.5f);
					source.anchorMax = new Vector2(0, 0.5f);
					break;
				}
			case (AnchorPresets.MiddleCenter):
				{
					source.anchorMin = new Vector2(0.5f, 0.5f);
					source.anchorMax = new Vector2(0.5f, 0.5f);
					break;
				}
			case (AnchorPresets.MiddleRight):
				{
					source.anchorMin = new Vector2(1, 0.5f);
					source.anchorMax = new Vector2(1, 0.5f);
					break;
				}

			case (AnchorPresets.BottomLeft):
				{
					source.anchorMin = new Vector2(0, 0);
					source.anchorMax = new Vector2(0, 0);
					break;
				}
			case (AnchorPresets.BottonCenter):
				{
					source.anchorMin = new Vector2(0.5f, 0);
					source.anchorMax = new Vector2(0.5f, 0);
					break;
				}
			case (AnchorPresets.BottomRight):
				{
					source.anchorMin = new Vector2(1, 0);
					source.anchorMax = new Vector2(1, 0);
					break;
				}

			case (AnchorPresets.HorStretchTop):
				{
					source.anchorMin = new Vector2(0, 1);
					source.anchorMax = new Vector2(1, 1);
					break;
				}
			case (AnchorPresets.HorStretchMiddle):
				{
					source.anchorMin = new Vector2(0, 0.5f);
					source.anchorMax = new Vector2(1, 0.5f);
					break;
				}
			case (AnchorPresets.HorStretchBottom):
				{
					source.anchorMin = new Vector2(0, 0);
					source.anchorMax = new Vector2(1, 0);
					break;
				}

			case (AnchorPresets.VertStretchLeft):
				{
					source.anchorMin = new Vector2(0, 0);
					source.anchorMax = new Vector2(0, 1);
					break;
				}
			case (AnchorPresets.VertStretchCenter):
				{
					source.anchorMin = new Vector2(0.5f, 0);
					source.anchorMax = new Vector2(0.5f, 1);
					break;
				}
			case (AnchorPresets.VertStretchRight):
				{
					source.anchorMin = new Vector2(1, 0);
					source.anchorMax = new Vector2(1, 1);
					break;
				}

			case (AnchorPresets.StretchAll):
				{
					source.anchorMin = new Vector2(0, 0);
					source.anchorMax = new Vector2(1, 1);
					break;
				}
		}
	}

	public static void SetPivot(this RectTransform source, PivotPresets preset)
	{

		switch (preset)
		{
			case (PivotPresets.TopLeft):
				{
					source.pivot = new Vector2(0, 1);
					break;
				}
			case (PivotPresets.TopCenter):
				{
					source.pivot = new Vector2(0.5f, 1);
					break;
				}
			case (PivotPresets.TopRight):
				{
					source.pivot = new Vector2(1, 1);
					break;
				}

			case (PivotPresets.MiddleLeft):
				{
					source.pivot = new Vector2(0, 0.5f);
					break;
				}
			case (PivotPresets.MiddleCenter):
				{
					source.pivot = new Vector2(0.5f, 0.5f);
					break;
				}
			case (PivotPresets.MiddleRight):
				{
					source.pivot = new Vector2(1, 0.5f);
					break;
				}

			case (PivotPresets.BottomLeft):
				{
					source.pivot = new Vector2(0, 0);
					break;
				}
			case (PivotPresets.BottomCenter):
				{
					source.pivot = new Vector2(0.5f, 0);
					break;
				}
			case (PivotPresets.BottomRight):
				{
					source.pivot = new Vector2(1, 0);
					break;
				}
		}
	}
}