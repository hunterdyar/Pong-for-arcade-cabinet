using System;
using System.Collections.Generic;
using Pong.Powerups;
using UnityEngine;

namespace Pong
{
	[CreateAssetMenu(fileName = "PlayerData", menuName = "Pong/Player Data", order = 0)]
	public class PlayerData : ScriptableObject
	{
		//static events
		public static Action AnyPlayerScoreChange;

		//Events
		public Action<int> OnScoreChange;
		public Action<Powerup> OnGainedPowerup;
		public Action<Powerup> OnPowerupActivated;
		public Action<Powerup> OnCurrentPowerupChanged;//"active" being whichever powerup will fire when we press the button
		public Color Color => _color;
		[SerializeField] private Color _color;

		public Vector2 UpDirection => _upDirectionInWorldSpace.normalized;
		[Tooltip("Direction that cannon powerup shoots, etc")]
		[SerializeField] private Vector2 _upDirectionInWorldSpace;
		public Paddle Paddle => _paddle;
		private Paddle _paddle;//The  actual gameObject in the scene

		public PongGameManager PongGameManager => _pongGameManager;
		private PongGameManager _pongGameManager;

		//ternery operator
		public Powerups.Powerup CurrentPowerup => _powerups.Count > 0 ? _powerups.Peek() : Powerup.None;

		private Stack<Powerup> _powerups = new Stack<Powerup>(); 
		
		public int Score => _score;

		private int _score;

		public void SetPongGameManager(PongGameManager pongGameManager)
		{
			_pongGameManager = pongGameManager;
		}

		public void GetPoint(int delta = 1)
		{
			var score = _score + delta;
			score = Mathf.Max(score, 0);//min of 0.
			SetScore(score);
		}

		public void SetScore(int newScore)
		{
			_score = newScore;
			OnScoreChange?.Invoke(_score);
			AnyPlayerScoreChange?.Invoke();
		}

		public void ResetPlayerData()
		{
			SetScore(0);
		}
		public void RegisterPaddle(Paddle paddle)
		{
			_paddle = paddle;
		}

		public void GetPowerup(Powerup powerup)
		{
			_powerups.Push(powerup);
			OnGainedPowerup?.Invoke(CurrentPowerup);
			OnCurrentPowerupChanged?.Invoke(CurrentPowerup);
		}

		public void ActivatePowerup()
		{
			//if we have a powerup, use it
			if (_powerups.Count > 0)
			{
				var powerup = _powerups.Pop();//removes it from our stack.
				if (powerup != Powerup.None)
				{
					Debug.Log("Activating Powerup: "+powerup);
					OnPowerupActivated?.Invoke(powerup);
				}

				OnCurrentPowerupChanged?.Invoke(CurrentPowerup);
			}
		}

		// public void ActivatePowerup(Powerup powerup)

		// {

		// 	if (powerup == Powerup.None)

		// 	{

		// 		return;

		// 	}

		// 	//if we have a powerup, use it

		// 	if (_powerups.Contains(powerup))

		// 	{

		//remove from the list?

		// 		OnPowerupActivated?.Invoke(powerup);

		// 	}

		// }
	}
}