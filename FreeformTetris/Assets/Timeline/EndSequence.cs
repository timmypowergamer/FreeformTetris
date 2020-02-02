using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class EndSequence : MonoBehaviour
{
	[SerializeField] private PlayableDirector director;
	public Transform cameraPoint;
	[SerializeField] private Rigidbody avatar;
	[SerializeField] private float WindForce;

	WallGenerator wall;

	private void Awake()
	{
		wall = GetComponentInParent<WallGenerator>();
	}

	private void OnEnable()
	{
		GameManager.OnGameFinished += GameFinished;
	}

	private void OnDisable()
	{
		GameManager.OnGameFinished -= GameFinished;
	}

	private void GameFinished()
	{
		if (wall.Owner == null) return;
		director.Play();
		wall.Owner.GetCamera().transform.position = cameraPoint.position;
		wall.Owner.GetCamera().transform.rotation = cameraPoint.rotation;
	}

	public void Eject()
	{
		if (!wall.IsWinningPlayer)
		{
			ConstantForce force = avatar.gameObject.AddComponent<ConstantForce>();
			force.force = cameraPoint.forward * WindForce;
			avatar.constraints = RigidbodyConstraints.None;
			avatar.AddRelativeTorque(new Vector3(0, 10, 0), ForceMode.Impulse);
		}
	}

	public void Kill()
	{
		//Debug.Log("kill");
		if(wall.IsWinningPlayer)
		{
			wall.Owner.SetWinningState(false);
		}
		else
		{
			if (GameManager.Instance.NumPlayers > 1)
			{
				wall.Owner.GetCamera().gameObject.SetActive(false);
			}
			else
			{
				wall.Owner.SetWinningState(true);
			}
		}
	}
}
