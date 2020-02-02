using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var component = other.GetComponentInParent<GrabbableObject>();
		if (component == null) { return; }
		component.Placed = true;
	}

	private void OnTriggerExit(Collider other)
	{
		var component = other.GetComponentInParent<GrabbableObject>();
		if (component == null) { return; }
		component.Placed = false;
	}
}
