using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabBehavior : MonoBehaviour
{
	private SpringJoint joint;
	private Rigidbody grabbedItem;

	[SerializeField] private float BreakForce = 2000f;
	[SerializeField] private float SpringDamper = 50f;
	[SerializeField] private float SpringForce = 200f;
	[SerializeField] private float GrabDrag = 20f;
	[SerializeField] private float GrabAngularDrag = 5f;
	[SerializeField] private float GrabDistance = 2.5f;
	[SerializeField] private LayerMask mask;

	[SerializeField] private Transform crosshair;

	private float previousDrag;
	private float previousAngularDrag;

	public bool IsGrabbing { get { return grabbedItem != null; } }

	private void FixedUpdate()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, GrabDistance, mask))
		{
			crosshair.transform.position = hit.point;
		}
		else
		{
			crosshair.transform.localPosition = Vector3.zero;
		}
	}

	public void Grab()
	{
		if (grabbedItem == null)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, GrabDistance, mask))
			{
				grabbedItem = hit.rigidbody;

				var grabbedObject = grabbedItem.GetComponent<GrabbableObject>();
				if (grabbedObject != null && grabbedObject.Placed)
				{
					grabbedObject.Unlock();
				}

				previousDrag = grabbedItem.drag;
				previousAngularDrag = grabbedItem.angularDrag;
				grabbedItem.angularDrag = GrabAngularDrag;
				grabbedItem.drag = GrabDrag;
				grabbedItem.useGravity = false;
				joint = gameObject.AddComponent<SpringJoint>();
				joint.autoConfigureConnectedAnchor = false;
				joint.anchor = transform.InverseTransformPoint(hit.point);
				joint.connectedAnchor = grabbedItem.transform.InverseTransformPoint(hit.point);
				joint.spring = SpringForce;
				joint.damper = SpringDamper;
				joint.breakForce = BreakForce;
				joint.connectedBody = grabbedItem;
			}
		}
	}

	public void Release()
	{
		if (grabbedItem != null)
		{
			grabbedItem.useGravity = true;
			grabbedItem.drag = previousDrag;
			grabbedItem.angularDrag = previousAngularDrag;
			if(joint != null) Destroy(joint);

			var grabbedObject = grabbedItem.GetComponent<GrabbableObject>();
			if (grabbedObject != null && grabbedObject.Placed)
			{
				grabbedObject.Lock();
			}

			grabbedItem = null;
		}
	}

	private void OnJointBreak(float breakForce)
	{
		if(joint == null)
		{
			Release();
		}
	}
}
