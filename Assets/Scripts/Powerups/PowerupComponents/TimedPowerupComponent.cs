using System;
using UnityEngine;

namespace Pong.Powerups
{
	//Automatically lose a powerup after a certain amount of time.
	public abstract class TimedPowerupComponent : PowerupComponent
	{
		[SerializeField] private bool activateInstantly;
		[SerializeField] private float duration;
		private float _timer;
		private bool _active = false;
		protected float CountdownTimerNormalized;//1->0
		protected float CountupTimerNormalized;//0->1
		protected virtual void Awake()
		{
			//automatically get this, since we're part of the paddle hierarchy.
			//We could just assign in inspector if the powerup exists elsewhere.
			Player = GetComponentInParent<Paddle>().Player;
		}

		private void Update()
		{
			if (_active)
			{
				_timer += Time.deltaTime;
				CountupTimerNormalized = Mathf.Clamp01(_timer / duration);
				CountdownTimerNormalized = 1 - CountupTimerNormalized;
				//call activeTimer AFTER we check the tick.
				//I am close to not having "tick" be an update-like function, but instead just pass in the normalized value whenever it changes.
				ActiveTick();
				if (_timer >= duration)
				{
					OnLose();
				}
			}
		}

		protected virtual void ActiveTick()
		{
			
		}

		protected override void OnGain()
		{
			if (activateInstantly)
			{
				//Force the player to use it! This also makes them lose it from the stack.
				Player.ActivatePowerup();
			}
		}

		protected override void OnActivate()
		{
			_timer = 0;
			_active = true;
		}
	}
}