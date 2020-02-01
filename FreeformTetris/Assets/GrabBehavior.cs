using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabBehavior : MonoBehaviour
{
	private Rigidbody collidingItem;
	private SpringJoint joint;
	private Rigidbody grabbedItem;

	[SerializeField] private float BreakForce = 2000f;
	[SerializeField] private float SpringDamper = 50f;
	[SerializeField] private float SpringForce = 200f;
	[SerializeField] private float GrabDrag = 20f;
	[SerializeField] private float GrabAngularDrag = 5f;

	private float previousDrag;
	private float previousAngularDrag;

	private void OnTriggerEnter(Collider other)
	{
		if(other.attachedRigidbody != null && collidingItem == null)
		{
			collidingItem = other.attachedRigidbody;
			Debug.Log($"colliding with {collidingItem.name}");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(collidingItem == other.attachedRigidbody)
		{
			Debug.Log($"{collidingItem.name} left");
			collidingItem = null;
		}
	}

	public bool IsGrabbing { get { return grabbedItem != null; } }

	public void Grab()
	{
		if(collidingItem != null)
		{
			Debug.Log($"Grabbing {collidingItem.name}");
			grabbedItem = collidingItem;
			previousDrag = grabbedItem.drag;
			previousAngularDrag = grabbedItem.angularDrag;
			grabbedItem.angularDrag = GrabAngularDrag;
			grabbedItem.drag = GrabDrag;
			//grabbedItem.useGravity = false;
			joint = gameObject.AddComponent<SpringJoint>();
			joint.autoConfigureConnectedAnchor = false;
			//joint.
			joint.anchor = Vector3.zero;
			joint.spring = SpringForce;
			joint.damper = SpringDamper;
			joint.breakForce = BreakForce;
			joint.connectedBody = grabbedItem;
			joint.connectedAnchor = Vector3.zero;
		}
	}

	public void Release()
	{
		if (grabbedItem != null)
		{
			//grabbedItem.useGravity = true;
			grabbedItem.drag = previousDrag;
			grabbedItem.angularDrag = previousAngularDrag;
			if(joint != null) Destroy(joint);
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
