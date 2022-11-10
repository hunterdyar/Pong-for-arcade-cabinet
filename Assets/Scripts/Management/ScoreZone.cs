using System;
using UnityEngine;

namespace Pong
{
	public class ScoreZone : MonoBehaviour
	{
		[SerializeField] private PlayerData _scoringPlayerData;

		private void Start()
		{
			GetComponent<Collider2D>().isTrigger = true;
		}

		public void SetScoringPlayer(PlayerData playerData)
		{
			_scoringPlayerData = playerData;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			//don't need to check for tag because we're checking for the component anyway
			//If we anticipate a lot of non-ball rigidbody things, then checking tag will optimize. 
			//we currently anticipate almost everything being a ball :p
			var ball = other.GetComponentInParent<Ball>();
			if (ball != null)
			{
				_scoringPlayerData.GetPoint();
				ball.BallEnteredScoreZone();
			}
		}
	}
}