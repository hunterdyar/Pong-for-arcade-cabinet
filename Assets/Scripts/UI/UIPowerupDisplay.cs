using System;
using Pong.Powerups;
using TMPro;
using UnityEngine;

namespace Pong.UI
{
	public class UIPowerupDisplay : MonoBehaviour
	{
		private TMP_Text _text;

		[SerializeField] private PlayerData _playerData;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			if (_playerData == null)
			{
				Debug.LogError("No player data set for UIPowerupDisplay",this);
			}
		}

		private void Start()
		{
			//Probably NONE on start but idk 
			OnCurrentPowerupChanged(_playerData.CurrentPowerup);
		}

		private void OnEnable()
		{
			_playerData.OnCurrentPowerupChanged += OnCurrentPowerupChanged;
		}

		private void OnDisable()
		{
			_playerData.OnCurrentPowerupChanged -= OnCurrentPowerupChanged;
		}

		private void OnCurrentPowerupChanged(Powerup powerup)
		{
			if (powerup == Powerup.None)
			{
				_text.text = "";
			}
			else
			{
				_text.text = powerup.ToString();
			}
		}
	}
}