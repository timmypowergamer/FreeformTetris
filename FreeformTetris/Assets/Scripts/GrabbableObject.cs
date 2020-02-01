﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
	public Rigidbody RigidBody { get; private set; } 

	private void Reset()
	{
		RigidBody = GetComponent<Rigidbody>();
		RigidBody.drag = 1;
		RigidBody.angularDrag = 1;
		RigidBody.transform.SetLayerRecursively("Objects");

		foreach(MeshFilter mesh in GetComponentsInChildren<MeshFilter>())
		{
			if(mesh.name.ToLower().Contains("collider") || mesh.name.ToLower().Contains("collision"))
			{
				MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
				DestroyImmediate(renderer);
				MeshCollider collider = mesh.gameObject.AddComponent<MeshCollider>();
				collider.convex = true;
			}
		}
	}
}