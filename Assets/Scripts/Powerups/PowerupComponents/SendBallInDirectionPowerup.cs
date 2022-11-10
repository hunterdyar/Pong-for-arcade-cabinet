using System;
using UnityEngine;
using UnityEngine.WSA;

namespace Pong.Powerups
{
	public class SendBallInDirectionPowerup : MonoBehaviour
	{
		private Ball _ball;
		private PlayerData _launchPlayer;
		[SerializeField] private float speed;
		private void Awake()
		{
			_ball = GetComponent<Ball>();
			_ball.OnLastPaddleTouchedChanged += OnLastPaddleTouchedChanged;
		}

		private void OnLastPaddleTouchedChanged(PlayerData player)
		{
			Subscribe(player);//this will automatically unsubscribe to previous
		}

		private void OnActivate(Powerup powerup)
		{
			if (powerup == Powerup.LaunchBall)
			{
				if (_launchPlayer != null)
				{
					_ball.LaunchBallInDirection(_launchPlayer.UpDirection,speed);
				}
			}
			
		}
		private void OnDestroy()
		{
			Unsubscribe();
		}

		private void Subscribe(PlayerData player)
		{
			if (_launchPlayer == player)
			{
				return;
			}
			
			if (_launchPlayer != null)
			{
				Unsubscribe();
			}
			player.OnPowerupActivated += OnActivate;
			_launchPlayer = player;

		}
		private void Unsubscribe()
		{
			if (_launchPlayer != null)
			{
				_launchPlayer.OnPowerupActivated -= OnActivate;
				_launchPlayer = null;
			}
		}
	}
}