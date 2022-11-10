using System.Xml;
using UnityEngine;

namespace Pong.Powerups
{
	public class LongPaddlePowerup : TimedPowerupComponent
	{
		[SerializeField] private Vector3 newLocalScale;
		private Vector3 _defaultLocalScale;

		protected override void ActiveTick()
		{
			transform.localScale = Vector3.Lerp(newLocalScale, _defaultLocalScale, CountupTimerNormalized);
		}

		protected override void Awake()
		{
			_defaultLocalScale = transform.localScale;
			base.Awake();
		}

		protected override void OnActivate()
		{
			//Activates Instantly?
			transform.localScale = newLocalScale;
			//not sure how this one should work
			base.OnActivate();
		}

		protected override void OnLose()
		{
			//todo: flicker off the paddle sprite for a bit before losing the collider.
			transform.localScale = _defaultLocalScale;
		}

		protected override bool IsPowerup(Powerup incoming)
		{
			return incoming == Powerup.LongPaddle;
		}
	}
}