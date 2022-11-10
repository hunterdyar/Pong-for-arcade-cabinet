using System;
using TMPro;
using UnityEngine;

namespace Pong
{
	/// <summary>
	/// Base class for easier "on player won" "on player lost" listening components
	/// </summary>
	public abstract class OnVictoryBase : MonoBehaviour
	{
		[SerializeField] protected PlayerData _winningPlayer;

		private void OnEnable()
		{
			PongGameManager.OnPlayerWon += OnPlayerWon;
		}

		private void OnDisable()
		{
			PongGameManager.OnPlayerWon -= OnPlayerWon;
		}

		private void OnPlayerWon(PlayerData player)
		{
			if (player == _winningPlayer)
			{
				//we won!
				OnMyPlayerWon();
			}
			else
			{
				//we lost!
				OnMyPlayerLost();
			}
		}

		protected virtual void OnMyPlayerWon()
		{
			//
		}

		protected virtual void OnMyPlayerLost()
		{
			//
		}
	}
}