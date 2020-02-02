using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public delegate void GameStartEvent();
	public GameStartEvent OnGameStarted;



	[SerializeField] private List<Transform> SpawnPoints;
	[SerializeField] private int MinimimPlayerCount = 2;
	[SerializeField] private PlayerHUDController _playerHUDPrefab;
	[SerializeField] private int MatchLength = 90;
	[SerializeField] private Transform _hudParent;
	[SerializeField] public Color[] colors;
	[SerializeField] private GameObject _timerObject;
	[SerializeField] private TextMeshProUGUI _timerText;

	private List<PlayerInput> activePlayers = new List<PlayerInput>();
	private List<PlayerInput> readyPlayers = new List<PlayerInput>();

	private int gameTimeRemaining;
	private DateTime gameTimeStart;

	private TimeSpan TimeRemaining { get { return new TimeSpan(0, 0, gameTimeRemaining); } }

	[SerializeField] private List<PlayerHUDController> playerHUDs = new List<PlayerHUDController>();

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError($"Detected more than one GameManager in the scene! : {gameObject.name}");
			return;
		}
		Instance = this;
		_timerObject.SetActive(false);
	}

	public void PlayerJoined(PlayerInput player)
	{
		activePlayers.Add(player);
		Debug.Log($"Player {player.playerIndex} Joined");
		if(activePlayers.Count >= PlayerInputManager.instance.maxPlayerCount)
		{
			PlayerInputManager.instance.DisableJoining();
		}

		PlayerHUDController hud = GetPlayerHUD(player);
		hud.SetActive(true);
		hud.SetPlayerNum(player.playerIndex);
		hud.SetReady(false);
	}

	public void PlayerLeft(PlayerInput player)
	{
		if(activePlayers.Contains(player))
		{
			if (readyPlayers.Contains(player)) ToggleReady(player);
			activePlayers.Remove(player);
			var hud = GetPlayerHUD(player);
			if (hud != null) hud.SetActive(false);
			Debug.Log($"Player {player.playerIndex} left");
			if (activePlayers.Count < PlayerInputManager.instance.playerCount && !PlayerInputManager.instance.joiningEnabled)
			{
				PlayerInputManager.instance.EnableJoining();
			}
			if (readyPlayers.Count == activePlayers.Count && readyPlayers.Count >= MinimimPlayerCount)
			{
				StartGame();
			}
		}
	}

	public void ToggleReady(PlayerInput player)
	{
		if(activePlayers.Contains(player))
		{
			if (!readyPlayers.Contains(player))
			{
				readyPlayers.Add(player);
				Debug.Log($"Player {player.playerIndex} is ready");
				if(readyPlayers.Count == activePlayers.Count && readyPlayers.Count >= MinimimPlayerCount)
				{
					StartGame();
				}
			}
			else
			{
				readyPlayers.Remove(player);
				Debug.Log($"Player {player.playerIndex} is not ready");
			}
		}
	}

	public Transform GetSpawnPoint(PlayerInput player)
	{
		Transform spawnpoint = SpawnPoints[0];
		if (SpawnPoints.Count >= PlayerInputManager.instance.playerCount)
		{
			spawnpoint = SpawnPoints[player.playerIndex];
		}
		return spawnpoint;
	}

	public PlayerHUDController GetPlayerHUD(PlayerInput player)
	{
		if(playerHUDs.Count > player.playerIndex)
		{
			return playerHUDs[player.playerIndex];
		}
		return null;
	}

	private async void StartGame()
	{
		if (readyPlayers.Count != activePlayers.Count) return;

		Debug.Log("Starting in 3...");
		await Task.Delay(1000);
		if (readyPlayers.Count != activePlayers.Count) return;
		Debug.Log("2...");
		await Task.Delay(1000);
		if (readyPlayers.Count != activePlayers.Count) return;
		Debug.Log("1...");
		await Task.Delay(1000);
		if (readyPlayers.Count != activePlayers.Count) return;

		gameTimeRemaining = MatchLength;
		gameTimeStart = DateTime.Now;
		Debug.Log("Game has started!");
		_timerObject.SetActive(true);
		OnGameStarted?.Invoke();
		GameRunningRoutine();
	}

	private async void GameRunningRoutine()
	{
		while (gameTimeRemaining > 0)
		{
			await Task.Delay(1000);
			gameTimeRemaining--;
			_timerText.text = TimeRemaining.ToString(@"mm\:ss");
		}
	}

	public void UpdateScore()
	{

	}
}
