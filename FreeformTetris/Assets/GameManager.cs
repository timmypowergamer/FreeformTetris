﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public delegate void GameEvent();
	public static GameEvent OnGameStarted;
	public static GameEvent OnGameFinished;
	public static GameEvent OnGameStarting;



	[SerializeField] private List<Transform> SpawnPoints;
	[SerializeField] private int MinimimPlayerCount = 2;
	[SerializeField] private PlayerHUDController _playerHUDPrefab;
	[SerializeField] private int MatchLength = 90;
	[SerializeField] private Transform _hudParent;
	[SerializeField] private Transform _hud34Parent;
	[SerializeField] public Color[] colors;
	[SerializeField] private GameObject _timerObject;
	[SerializeField] private TextMeshProUGUI _timerText;
	[SerializeField] private WallGenerator[] _walls;
	[SerializeField] private float minimumSPWinThreshold = 0.5f;
    [SerializeField] private AudioSource GameAudio;
	[SerializeField] private GameObject p3_camera;
	[SerializeField] private TextMeshProUGUI _countdownText;
	[SerializeField] private Transform _endHudParent;
    [SerializeField] private AudioSource ZapSource;

	private List<PlayerInput> activePlayers = new List<PlayerInput>();
	private List<PlayerInput> readyPlayers = new List<PlayerInput>();

	private Coroutine startRoutine;

	private int gameTimeRemaining;
	private DateTime gameTimeStart;

	private bool GameHasBeenStarted = false;

	public int NumPlayers { get { return activePlayers.Count; } }

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
		_countdownText.transform.parent.gameObject.SetActive(false);
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
		_walls[player.playerIndex].SetOwner(player.GetComponent<PlayerController>());
		_walls[player.playerIndex].SetColor(this.colors[player.playerIndex]);
		hud.SetActive(true);
		hud.SetPlayerNum(player.playerIndex);
		hud.SetReady(false);
		_hud34Parent.gameObject.SetActive(activePlayers.Count >= 3);
		p3_camera.SetActive(activePlayers.Count > 0);
		_countdownText.transform.parent.gameObject.SetActive(false);
	}

	public void PlayerLeft(PlayerInput player)
	{
		if(activePlayers.Contains(player))
		{
			if (readyPlayers.Contains(player)) ToggleReady(player);
			activePlayers.Remove(player);
			var hud = GetPlayerHUD(player);
			if (hud != null) hud.SetActive(false);
			_hud34Parent.gameObject.SetActive(activePlayers.Count >= 3);
			_walls[player.playerIndex].SetOwner(null);
			Debug.Log($"Player {player.playerIndex} left");
			if (GameHasBeenStarted) return;
			if (activePlayers.Count < PlayerInputManager.instance.playerCount && !PlayerInputManager.instance.joiningEnabled)
			{
				PlayerInputManager.instance.EnableJoining();
			}
			if (readyPlayers.Count == activePlayers.Count && readyPlayers.Count >= MinimimPlayerCount)
			{
				startRoutine = StartCoroutine(StartGame());
			}
			else if(startRoutine != null)
			{
				StopCoroutine(startRoutine);
				_countdownText.transform.parent.gameObject.SetActive(false);
			}
			p3_camera.SetActive(activePlayers.Count > 0);
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
					startRoutine = StartCoroutine(StartGame());
				}
			}
			else
			{
				readyPlayers.Remove(player);
				Debug.Log($"Player {player.playerIndex} is not ready");
				_countdownText.transform.parent.gameObject.SetActive(false);
				if (startRoutine != null)
				{
					StopCoroutine(startRoutine);
				}
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

	private IEnumerator StartGame()
	{
		if (readyPlayers.Count != activePlayers.Count) yield break;

		OnGameStarting?.Invoke();

		int countdown = 3;
		float timer = 1;

		_countdownText.transform.parent.gameObject.SetActive(true);
		while (countdown > 0)
		{
			timer = 1;
			Debug.Log($"Starting in {countdown}...");
			_countdownText.text = countdown.ToString();
			while(timer > 0)
			{
				yield return null;
				timer -= Time.deltaTime;
			}
			countdown--;
			if (activePlayers.Count != readyPlayers.Count) yield break;
		}

		_countdownText.text = "START!";
		gameTimeRemaining = MatchLength;
		gameTimeStart = DateTime.Now;
		Debug.Log("Game has started!");
		_timerObject.SetActive(true);
		GameHasBeenStarted = true;
        AudioManager.Instance?.EndMusic();
        GameAudio?.Play();
		OnGameStarted?.Invoke();
		GameRunningRoutine();
		timer = 1f;
		while (timer > 0)
		{
			yield return null;
			timer -= Time.deltaTime;
		}
		_countdownText.transform.parent.gameObject.SetActive(false);
	}

	private async void GameRunningRoutine()
	{
		while (gameTimeRemaining > 0 && Application.isPlaying)
		{
			foreach(WallGenerator wall in _walls)
			{				
				wall.UpdateScoreboard();
			}
			await Task.Delay(1000);
			gameTimeRemaining--;
			_timerText.text = TimeRemaining.ToString(@"mm\:ss");
		}
		GameEnded();
	}

	private void GameEnded()
	{
		WallGenerator winner = null;
		float bestScore = -1;
		foreach(WallGenerator wall in _walls)
		{
			float score = wall.GetScore();
			if(score > bestScore)
			{
				bestScore = score;
				winner = wall;
			}
		}
		if(activePlayers.Count > 1 || bestScore >= minimumSPWinThreshold) winner.IsWinningPlayer = true;
		OnGameFinished?.Invoke();
	}

	public void SetWinner(int playerNum)
	{
		for (int i = 0; i < playerHUDs.Count; i++)
		{
			if (i == playerNum)
			{
				playerHUDs[i].transform.SetParent(_endHudParent, false);
			}
			else
			{
				playerHUDs[i].gameObject.SetActive(false);
			}
		}
	}

    public void PlayZap()
    {
        ZapSource.Play();
    }
}
