using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public enum PlayerState
	{
		JOINED,
		READY,
		SPAWNING,
		PLAYING,
		PODIUM
	}

	public enum GroundState
	{
		GROUNDED,
		JUMPING,
		FALLING
	}

	public PlayerState CurrentPlayerState { get; private set; }
	public GroundState CurrentGroundState { get; private set; }

	[SerializeField] private CharacterController Controller;
	[SerializeField] private Camera Camera;
	[SerializeField] private Transform GroundCheckLocation;
	[SerializeField] private Transform GrabPoint;

	[SerializeField] private float MoveSpeed = 5;
	[SerializeField] private float LookSensitivity = 3;
	[SerializeField] private float MaxPositiveVerticalLook = 80f;
	[SerializeField] private float MaxNegativeVerticalLook = -80f;
	[SerializeField] private float Gravity = -9.81f;
	[SerializeField] private float GroundCheckDistance = 0.4f;
	[SerializeField] private LayerMask GroundMask;
	[SerializeField] private float JumpVelocity = 10f;
	[SerializeField] private float GrabRadius = 0.4f;
	[SerializeField] private LayerMask GrabMask;

	//movement
	private Vector2 MoveInputValue;
	private Vector2 LookInputValue;
	private Vector2 lookRotation;
	private Vector3 Velocity;
	private bool jumpPressed = false;
	private bool grabHeld = false;
	private bool grabPressed = false;


	private PlayerInput Input;
	private Transform spawnPoint;

	private Rigidbody heldObject;

	private void OnEnable()
	{
		Input = GetComponent<PlayerInput>();
		spawnPoint = GameManager.Instance.GetSpawnPoint(Input);
		GameManager.Instance.OnGameStarted += OnGameStart;
		CurrentPlayerState = PlayerState.JOINED;
		Controller.enabled = false;
		transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
		Camera.enabled = false;
	}

	private void OnDisable()
	{
		GameManager.Instance.OnGameStarted -= OnGameStart;
	}

	public void Move(InputAction.CallbackContext context)
	{
		if(CurrentPlayerState == PlayerState.PLAYING)
			MoveInputValue = context.ReadValue<Vector2>();		
	}
	
	public void Look(InputAction.CallbackContext context)
	{
		if (CurrentPlayerState == PlayerState.PLAYING)
			LookInputValue = context.ReadValue<Vector2>();
	}

	public void Grab(InputAction.CallbackContext context)
	{
		if (CurrentPlayerState == PlayerState.PLAYING)
		{
			grabPressed = context.phase == InputActionPhase.Performed;
			if (!grabHeld && context.phase == InputActionPhase.Started)
			{
				grabHeld = true;
			}
			if (grabHeld && context.phase == InputActionPhase.Canceled)
			{
				grabHeld = false;
			}
		}
	}

	public void Ready(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
		{
			if (CurrentPlayerState == PlayerState.JOINED)
			{
				CurrentPlayerState = PlayerState.READY;
				GameManager.Instance.ToggleReady(Input);
			}
			else if (CurrentPlayerState == PlayerState.READY)
			{
				CurrentPlayerState = PlayerState.JOINED;
				GameManager.Instance.ToggleReady(Input);
			}
			else if (CurrentPlayerState == PlayerState.PLAYING)
			{
				jumpPressed = true;
			}
		}
	}

	public void OnGameStart()
	{
		Respawn();		
	}

	public void Respawn()
	{
		CurrentPlayerState = PlayerState.PLAYING;
		Velocity = Vector3.zero;
		Controller.enabled = true;
		Camera.enabled = true;
		CurrentGroundState = GroundState.GROUNDED;
	}

	private void FixedUpdate()
	{
		if (CurrentPlayerState == PlayerState.PLAYING)
		{
			//Ground check
			if (CurrentGroundState != GroundState.JUMPING)
			{
				if (Physics.CheckSphere(GroundCheckLocation.position, GroundCheckDistance, GroundMask))
				{
					CurrentGroundState = GroundState.GROUNDED;
				}
				else
				{
					CurrentGroundState = GroundState.FALLING;
				}
			}
			else if(Velocity.y <= 0)
			{
				CurrentGroundState = GroundState.FALLING;
			}

			//look
			lookRotation.y += LookInputValue.x * LookSensitivity;
			lookRotation.x += LookInputValue.y * LookSensitivity;
			lookRotation.x = Mathf.Clamp(lookRotation.x, MaxNegativeVerticalLook, MaxPositiveVerticalLook);
			Controller.transform.localRotation = Quaternion.Euler(0, lookRotation.y, 0);
			Camera.transform.localRotation = Quaternion.Euler(lookRotation.x, 0, 0);

			//move
			Vector3 movement = (Controller.transform.right * MoveInputValue.x + Controller.transform.forward * MoveInputValue.y) * MoveSpeed;
			Velocity.x = movement.x;
			Velocity.z = movement.z;
			if (CurrentGroundState != GroundState.GROUNDED)
			{
				//gravity
				Velocity.y += Gravity * Time.fixedDeltaTime;
			}
			else
			{
				Velocity.y = Controller.velocity.y;
			}
			//jump
			if(CurrentGroundState == GroundState.GROUNDED && jumpPressed)
			{
				Velocity.y += JumpVelocity;
				CurrentGroundState = GroundState.JUMPING;
			}
			Controller.Move(Velocity * Time.fixedDeltaTime);
			jumpPressed = false;

			if (heldObject == null && grabPressed)
			{
				RaycastHit hitInfo;
				if (Physics.SphereCast(GrabPoint.position, GrabRadius, GrabPoint.forward, out hitInfo, 0.01f, GrabMask))
				{
					Grab(hitInfo.collider.attachedRigidbody);
				}
			}
			else if (heldObject != null && !grabHeld)
			{
				ReleaseItem();
			}
			grabPressed = false;
		}

		
	}

	private void Grab(Rigidbody item)
	{
		heldObject = item;
		heldObject.isKinematic = true;
		heldObject.transform.parent = GrabPoint;
		//heldObject.transform.localPosition = Vector3.zero;
	}

	private void ReleaseItem()
	{
		if(heldObject != null)
		{
			heldObject.isKinematic = false;
			heldObject.transform.parent = null;
		}
	}

}
