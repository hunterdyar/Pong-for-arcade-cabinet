using System;
using System.Collections;
using System.Collections.Generic;
using Pong.Powerups;
using UnityEngine;

namespace Pong
{
    public class ConfusePickUp : TimedPowerupComponent
    {
	    private PaddleMovement[] _otherPlayersMovements;
		protected override void Awake()
		{
			//Populate a list of this powerup component of other players.
			Player = GetComponentInParent<Paddle>().Player;
		}

		private void Start()
		{
			var playersData = Player.PongGameManager.GetOtherPlayers(Player);
			_otherPlayersMovements = new PaddleMovement[playersData.Length];
			for (int i = 0; i < playersData.Length; i++)
			{
				_otherPlayersMovements[i] = playersData[i].Paddle.GetComponent<PaddleMovement>();
			}

		}

		protected override bool IsPowerup(Powerup incoming)
		{
			return incoming == Powerup.Confused;
		}

		protected override void OnGain()
		{
			base.OnGain();
			foreach (var playersMovement in (_otherPlayersMovements))
			{
				playersMovement.IsConfused = true;
			}
		}

		protected override void OnLose()
		{
			foreach (var playersMovement in _otherPlayersMovements)
			{
				playersMovement.IsConfused = false;
			}
		}
    }
}
