using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
	[SerializeField] private GameObject _readyIndicatorGroup;
	[SerializeField] private TextMeshProUGUI _readyIndicator;
	[SerializeField] private Color _readyColor;
	[SerializeField] private Color _notReadyColor;

	[SerializeField] private GameObject _readyInstructions;
	[SerializeField] private GameObject _notReadyInstructions;
	[SerializeField] private Image[] borders;

	public void SetPlayerNum(int playerNum)
	{
		foreach(Image i in borders)
		{
			i.color = GameManager.Instance.colors[playerNum];
		}
	}

	public void SetActive(bool value)
	{
		gameObject.SetActive(value);
	}

	public void SetReady(bool value)
	{
		_readyIndicatorGroup.SetActive(true);
		_readyIndicator.text = value ? "Ready!" : "NotReady";
		_readyIndicator.color = value ? _readyColor : _notReadyColor;
		_readyInstructions.SetActive(value);
		_notReadyInstructions.SetActive(!value);
	}

	public void Spawned()
	{
		_readyIndicatorGroup.SetActive(false);
		_readyInstructions.SetActive(false);
		_notReadyInstructions.SetActive(false);
	}
}
