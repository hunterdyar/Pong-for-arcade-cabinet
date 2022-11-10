using System;
using UnityEngine;

namespace Pong.Powerups
{
	public class CannonPowerup : PowerupComponent
	{
		[SerializeField] private Transform _ballSpawnLocation;
		[Min(0)]
		[SerializeField] private float _ballSpeed;

		private int ammoCount;
		
		private SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.enabled = false;
			Player = GetComponentInParent<Paddle>().Player;
			//
			_spriteRenderer.color = Player.Color;
			ammoCount = 0;
		}

		protected override void OnActivate()
		{
			var ball = Player.PongGameManager.CreateNewBall(false);
			ball.transform.position = _ballSpawnLocation.position;
			var direction = Player.UpDirection + Player.Paddle.CurrentVelocity;
			ball.LaunchBallInDirection(direction,_ballSpeed);
			ball.SetPlayerLastHit(Player);
			ammoCount--;
			
			//todo: play some kind of fire animation and THEN call OnLose.
			if (ammoCount <= 0)
			{
				OnLose();
			}
		}

		protected override void OnGain()
		{
			ammoCount++;//if we get it again while already having it, get more ammo!
			
			//play some kind of animation to make the powerup emerge.
			_spriteRenderer.enabled = true;
		}

		protected override void OnLose()
		{
			_spriteRenderer.enabled = false;
		}

		protected override bool IsPowerup(Powerup incoming)
		{
			return incoming == Powerup.Cannon;
		}
	}
}