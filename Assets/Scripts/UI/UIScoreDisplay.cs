using System;
using TMPro;
using UnityEngine;

namespace Pong.UI
{
	public class UIScoreDisplay : MonoBehaviour
	{
		private TMP_Text _text;
		
		[SerializeField]
		private PlayerData _playerData;
		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			if (_playerData == null)
			{
				Debug.LogError("No player data set for UIScoreDisplay",this);
			}
		}

		private void OnEnable()
		{
			_playerData.OnScoreChange += OnScoreChange;
		}

		private void OnDisable()
		{
			_playerData.OnScoreChange -= OnScoreChange;
		}

		private void OnScoreChange(int score)
		{
			_text.text = score.ToString("D");
		}
	}
}