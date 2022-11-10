using System;
using UnityEngine;

namespace Pong.Powerups
{
	public class HideOpponentPaddlePowerupComponent : TimedPowerupComponent
	{
		[SerializeField] private SpriteRenderer[] _spriteRenderers;
		private Color _originalColor;
		private HideOpponentPaddlePowerupComponent[] _otherPlayers;
		protected override void Awake()
		{
			//Populate a list of this powerup component of other players.
			Player = GetComponentInParent<Paddle>().Player;
		}

		private void Start()
		{
			var playersData = Player.PongGameManager.GetOtherPlayers(Player);
			_otherPlayers = new HideOpponentPaddlePowerupComponent[playersData.Length];
			for (int i = 0; i < playersData.Length; i++)
			{
				_otherPlayers[i] = playersData[i].Paddle.GetComponentInChildren<HideOpponentPaddlePowerupComponent>();
			}
			// Player = Player.PongGameManager.GetOtherPlayers(Player);
			_originalColor = Player.Color;
		}

		protected override bool IsPowerup(Powerup incoming)
		{
			return incoming == Powerup.HideOpponent;
		}

		protected override void OnGain()
		{
			base.OnGain();
			foreach (var player in (_otherPlayers))
			{
				player.Hide();
			}
		}

		protected override void OnLose()
		{
			foreach (var player in _otherPlayers)
			{
				player.Show();
			}
		}

		private void Hide()
		{
			foreach (var sr in _spriteRenderers)
			{
				sr.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
			}
		}

		private void Show()
		{
			foreach (var sr in _spriteRenderers)
			{
				sr.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 1);
			}
		}
	}
}