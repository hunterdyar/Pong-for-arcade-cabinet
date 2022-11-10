using System;
using UnityEngine;

namespace Pong.Powerups
{
	public class PowerupPickup : MonoBehaviour
	{
		//Powerups could be scriptableObjects or monobehaviours or just classes, who knows!
		[SerializeField] private PowerupManager _powerupManager;
		private Powerup _powerup= Powerup.None;

		private SpriteRenderer _spriteRenderer;
		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		//Called by spawner
		public void SetPickup(Powerup powerup)
		{
			this._powerup = powerup;
		}

		private void Start()
		{
			if (_powerup == Powerup.None)
			{
				//Created improperly. Probably left in a scene.
				Destroy(gameObject);
				return;
			}
			
			var s = _powerupManager.GetSprite(_powerup);
			if (s != null)
			{
				_spriteRenderer.sprite = s;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			var ball = other.GetComponentInParent<Ball>();
			if (ball != null)//we could have been spawned or moved into another powerup.
			{
				var lastPlayer = ball.GetLastPlayer();
				if (lastPlayer != null)
				{
					OnPickup(lastPlayer);
				}
			}
		}

		public virtual void OnPickup(PlayerData player)
		{
			_powerupManager.OnPickup(this);
			player.GetPowerup(_powerup);
			//todo: JUICE
			Destroy(gameObject);
		}

		
	}
}