using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public delegate void GameStartEvent();
	public GameStartEvent OnGameStarted;

	[SerializeField] private List<Transform> SpawnPoints;
	[SerializeField] private int MinimimPlayerCount = 2;

	private List<PlayerInput> activePlayers = new List<PlayerInput>();
	private List<PlayerInput> readyPlayers = new List<PlayerInput>();

	private Task StartGameTask;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError($"Detected more than one GameManager in the scene! : {gameObject.name}");
			return;
		}
		Instance = this;
	}

	public void PlayerJoined(PlayerInput player)
	{
		activePlayers.Add(player);
		Debug.Log($"Player {player.playerIndex} Joined");
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
		Debug.Log("Game has started!");
		OnGameStarted?.Invoke();
	}
}
