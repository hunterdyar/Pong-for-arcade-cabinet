using System;
using UnityEngine;

namespace Pong.Powerups
{
	public abstract class PowerupComponent : MonoBehaviour
	{
		protected PlayerData Player;
		private void OnEnable()
		{
			Player.OnGainedPowerup += OnAnyGainedPowerup;
			Player.OnPowerupActivated += OnAnyPowerupActivated;
		}
		
		private void OnDisable()
		{
			Player.OnGainedPowerup -= OnAnyGainedPowerup;
			Player.OnPowerupActivated -= OnAnyPowerupActivated;
		}

		private void OnAnyPowerupActivated(Powerup p)
		{
			if (IsPowerup(p))
			{
				OnActivate();
			}
		}

		private void OnAnyGainedPowerup(Powerup p)
		{
			if (IsPowerup(p))
			{
				OnGain();
			}
		}

		protected abstract bool IsPowerup(Powerup incoming);

		protected abstract void OnGain();

		protected abstract void OnActivate();

		protected abstract void OnLose();
	}

}