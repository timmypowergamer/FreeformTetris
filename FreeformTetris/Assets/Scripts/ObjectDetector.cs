using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
	private List<GrabbableObject> objectsInside = new List<GrabbableObject>();

	[SerializeField] private float WindForce = 500;

	private bool EndSequencePlaying = false;
	WallGenerator wall;

	private void Awake()
	{
		wall = GetComponentInParent<WallGenerator>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (EndSequencePlaying) return;
		var component = other.GetComponentInParent<GrabbableObject>();
		if (component == null) { return; }
		component.Placed = true;
		if(!objectsInside.Contains(component)) objectsInside.Add(component);
	}

	private void OnTriggerExit(Collider other)
	{
		if (EndSequencePlaying) return;
		var component = other.GetComponentInParent<GrabbableObject>();
		if (component == null) { return; }
		component.Placed = false;
		if (objectsInside.Contains(component)) objectsInside.Remove(component);
	}

	public void SetupFixedJoints()
	{
		EndSequencePlaying = true;
		foreach(GrabbableObject obj in objectsInside)
		{
			if (obj == null) continue;
			obj.RigidBody.constraints = RigidbodyConstraints.None;
			SpringJoint fixie = obj.gameObject.AddComponent<SpringJoint>();
			fixie.anchor = Vector3.zero;
			obj.RigidBody.useGravity = false;
			obj.RigidBody.mass = 0.5f;
			obj.RigidBody.drag = 3f;
			fixie.spring = 2000;
			fixie.damper = 200;
			ConstantForce force = obj.gameObject.AddComponent<ConstantForce>();
			Vector3 rand = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
			force.force = (transform.forward * WindForce) + rand;
		}		
	}

	public void BreakAll()
	{
		if (!wall.IsWinningPlayer)
		{
			foreach (GrabbableObject obj in objectsInside)
			{
				if (obj == null) continue;
				SpringJoint fixie = obj.gameObject.GetComponent<SpringJoint>();
				Destroy(fixie);
				obj.transform.SetLayerRecursively("ObjectInteraction");
			}
		}
		else
		{
			foreach (GrabbableObject obj in objectsInside)
			{
				if (obj == null) continue;
				ConstantForce force = obj.gameObject.GetComponent<ConstantForce>();
				Destroy(force);
			}
		}
	}
}
