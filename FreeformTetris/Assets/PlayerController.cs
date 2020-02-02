using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public enum PlayerState
	{
		NONE,
		JOINED,
		READY,
		SPAWNING,
		PLAYING,
		PODIUM,
		WINNER
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
	[SerializeField] private GrabBehavior GrabPoint;

	[SerializeField] private float MoveSpeed = 5;
	[SerializeField] private float LookSensitivity = 3;
	[SerializeField] private float MaxPositiveVerticalLook = 80f;
	[SerializeField] private float MaxNegativeVerticalLook = -80f;
	[SerializeField] private float Gravity = -9.81f;
	[SerializeField] private float GroundCheckDistance = 0.4f;
	[SerializeField] private LayerMask GroundMask;
	[SerializeField] private float JumpVelocity = 10f;

	private PlayerHUDController HUD;
	private WallGenerator wall;

	//movement
	private Vector2 MoveInputValue;
	private Vector2 LookInputValue;
	private Vector2 lookRotation;
	private Vector3 Velocity;
	private bool jumpPressed = false;
	private bool grabHeld = false;
	private bool grabPressed = false;


	public PlayerInput Input { get; private set; }
	private Transform spawnPoint;

    private Animator animator;

	private void OnEnable()
	{
		Input = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
		spawnPoint = GameManager.Instance.GetSpawnPoint(Input);
		GameManager.OnGameStarted += OnGameStart;
		GameManager.OnGameFinished += OnGameFinished;

		CurrentPlayerState = PlayerState.JOINED;
		Controller.enabled = false;
		transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.localRotation);
		Camera.enabled = false;
		HUD = GameManager.Instance.GetPlayerHUD(Input);
		HUD.SetReady(false);
        animator.SetFloat("Forward", 0);
    }

    private void OnDisable()
	{
		GameManager.OnGameStarted -= OnGameStart;
		GameManager.OnGameFinished -= OnGameFinished;
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
				HUD.SetReady(true);
			}
			else if (CurrentPlayerState == PlayerState.WINNER)
			{
				SceneManager.LoadScene("TitleScene");
			}
			else if (CurrentPlayerState == PlayerState.PLAYING)
			{
				jumpPressed = true;
			}
		}
	}

	public void Leave(InputAction.CallbackContext context)
	{
		//Debug.Log("ready");
		if (context.phase == InputActionPhase.Performed)
		{
			if(CurrentPlayerState == PlayerState.JOINED)
			{				
				Destroy(gameObject);
			}
			else if (CurrentPlayerState == PlayerState.READY)
			{
				CurrentPlayerState = PlayerState.JOINED;
				GameManager.Instance.ToggleReady(Input);
				HUD.SetReady(false);
			}
		}
	}

	public void OnGameStart()
	{
		Respawn();		
	}

	public void OnGameFinished()
	{
		CurrentPlayerState = PlayerState.PODIUM;
		Velocity = Vector3.zero;
		Controller.enabled = false;
		HUD.GameOver(wall.GetScore());
		animator.gameObject.SetActive(false);
	}

	public void Respawn()
	{
		HUD.Spawned();
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
					//Debug.Log("Grounded");
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
			//gravity
			Velocity.y += Gravity * Time.fixedDeltaTime;

			//jump
			if(CurrentGroundState == GroundState.GROUNDED && jumpPressed)
			{
				Velocity.y = JumpVelocity;
				//Velocity.y += JumpVelocity;
				CurrentGroundState = GroundState.JUMPING;
			}
			Controller.Move(Velocity * Time.fixedDeltaTime);
			jumpPressed = false;

			if (grabPressed && !GrabPoint.IsGrabbing)
			{
				GrabPoint.Grab();
			}
			else if (!grabHeld && GrabPoint.IsGrabbing)
			{
				GrabPoint.Release();
			}
			grabPressed = false;

            animator.SetFloat("Forward", MoveInputValue.y);
		}		
	}

	public void SetWinningState(bool youActuallyLost)
	{
		CurrentPlayerState = PlayerState.WINNER;
		if (youActuallyLost)
		{
			HUD.SetLoser();
		}
		else
		{
			HUD.SetWinner();
		}
	}

	public Camera GetCamera()
	{
		return Camera;
	}

	public void SetWall(WallGenerator wall)
	{
		this.wall = wall;
	}
}
