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

	[SerializeField] private GameObject _winGroup;
	[SerializeField] private GameObject _loseGroup;

	[SerializeField] private GameObject _fillGroup;
	[SerializeField] private Image _fillBar;
	[SerializeField] private TextMeshProUGUI _fillPct;

	[SerializeField] private Gradient _fillGradient;

	[SerializeField] private TextMeshProUGUI loseTextMesh;
	[SerializeField] private TextMeshProUGUI winTextMesh;

	private string[] winText = {
		"Nice Wall!",
		"Good Enough!",
		"You Did It!",
		"Adequate Work!",
		"You've Maintained Your Honour.",
		"You Should Work in Construction!"
	};

	private string[] loseText =
	{
		"Try Harder",
		"Maybe Next Time",
		"Not Good Enough",
		"Not Your Best Work",
		"Weak!",
		"Your Wall is a Source of Shame",
		"You Participated!",
		"Git Gud.",
		"You Have Been Consumed by an Unspeakable Horror.",
		"There are no words."
	};

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

	public void GameStarting()
	{
		_readyIndicatorGroup.SetActive(false);
	}

	public void Spawned()
	{
		_readyIndicatorGroup.SetActive(false);
		_readyInstructions.SetActive(false);
		_notReadyInstructions.SetActive(false);
	}

	public void GameOver(float score)
	{
		_fillGroup.SetActive(true);
		StartCoroutine(FillRoutine(score, 3));
	}

	private IEnumerator FillRoutine(float score, float totalTime)
	{
		float time = 0;
		float fill = 0;
		_fillBar.fillAmount = 0;
		_fillPct.text = (fill * 100).ToString("N0") + "%";
		_fillBar.color = _fillGradient.Evaluate(fill);

		while (time < totalTime && fill < score)
		{
			fill = Mathf.Lerp(0, 1, (time / totalTime));
			_fillBar.fillAmount = fill;
			_fillPct.text = (fill * 100).ToString("N0") + "%";
			_fillBar.color = _fillGradient.Evaluate(fill);
			yield return null;
			time += Time.deltaTime;
		}
		_fillBar.fillAmount = score;
		_fillPct.text = (score * 100).ToString("N0") + "%";
		_fillBar.color = _fillGradient.Evaluate(score);
	}

	public void SetWinner()
	{
		winTextMesh.text = this.winText[Random.Range(0, this.winText.Length)];
		_winGroup.gameObject.SetActive(true);
	}

	public void SetLoser()
	{
		loseTextMesh.text = this.loseText[Random.Range(0, this.loseText.Length)];
		_loseGroup.gameObject.SetActive(true);
	}
}
