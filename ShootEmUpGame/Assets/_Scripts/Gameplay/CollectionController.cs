using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
	public int gold
	{
		get
		{
			return _gold;
		}
		set
		{
			_gold = value;
			if (GamePlayManager.Instance.Collection != null
				&& GamePlayManager.Instance.Collection.gameObject.activeSelf)
			{
				GamePlayManager.Instance.Collection.RefreshGoldInfoPanel();
			}
		}
	}
	private int _gold;
	private int _goldIncrement;
	public int score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
			if (GamePlayManager.Instance.Collection != null
				&& GamePlayManager.Instance.Collection.gameObject.activeSelf)
			{
				GamePlayManager.Instance.Collection.RefreshScoreInfoPanel();
			}
		}
	}
	private int _score;
	private int _scoreIncrement;
	public int highScore;
	[SerializeField]
	private GoldParticleSystem _goldParticle;

	void FixedUpdate() {
		RefreshScoreInfoPanel();
		RefreshGoldInfoPanel();
	}

	public void Install()
    {
		_goldParticle.Install();
    }

	public void AddScore(int value)
	{
		_scoreIncrement += value;
	}
	public void AddGold(int value)
	{
		_goldIncrement += value;
		GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySFX("coin");
	}
	public void RefreshScoreInfoPanel()
	{
		GamePlayManager.Instance.UIManager.AddScore(_score, _score + _scoreIncrement);
		_score += _scoreIncrement;
		_scoreIncrement = 0;
	}

	public void RefreshGoldInfoPanel()
	{
		GamePlayManager.Instance.UIManager.AddGold(_gold, _gold + _goldIncrement);
		_gold += _goldIncrement;
		_goldIncrement = 0;
	}
}
