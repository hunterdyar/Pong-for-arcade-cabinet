using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pong;
using Pong.Powerups;
using Unity.VisualScripting;
using UnityEngine;

namespace Pong
{
	public class PongGameManager : MonoBehaviour
	{
		[Header("Scene Config")]
		[SerializeField] private Transform ballSpawnPosition;
		[SerializeField] private Countdown _countdown;

		[Header("Game Config")]
		[SerializeField]
		private float _delayBeforeBallLaunch;
		[Min(1)] [SerializeField]
		private int _startingBallCount;
		
		[Header("Data Setup")]
		[SerializeField] private GameObject ballPrefab;
		[SerializeField] private PowerupManager _powerupManager;
		
		private readonly List<Ball> _ballsInPlay = new List<Ball>();

		[SerializeField] private PlayerData[] _players;

		private void Awake()
		{
			_powerupManager.Init();

			foreach (var player in _players)
			{
				player.SetPongGameManager(this);
			}
		}

		private void OnEnable()
		{
			Countdown.OnCountdownGo += OnCountdownGo;
		}

		private void OnDisable()
		{
			Countdown.OnCountdownGo -= OnCountdownGo;
		}

		private void Start()
		{
			foreach (var player in _players)
			{
				player.ResetPlayerData();
				player.SetInputActive(true);
			}
			
			for (int i = 0; i < _startingBallCount; i++)
			{
				CreateNewBall(false);
			}
			
			//initial state
			foreach (var player in _players)
			{
				player.SetInputActive(false);
				player.ResetPosition();
			}

			_countdown.StartCountdown();
			_powerupManager.StartGame();
		}

		void Update()
		{
			_powerupManager.Tick();
		}

		//Todo: Refactor this so a Countdown object handles everything and then calls back to the game manager
		private void OnCountdownGo()
		{
			foreach(var player in _players)
			{
				player.SetInputActive(true);
			}

			//todo: who determines ball speed?
			foreach (var ball in _ballsInPlay)
			{
				ball.FireBall();
			}
		}

		private Vector3 GetValidSpawnPosition()
		{
			//Todo: Sample for collisions and pick different spots. 
			//Either loop through a list of available spots, or keep searching in an increasing radius around the spawn position for a valid spot. 
			//I prefer list, and the lack of randomness, which will look nice if we decide to spawn in 5 at once.
			int attempt = 0;
			while (attempt < 100)//we could calculate max attempts to make depending on, like, the width of the screen and how many balls we could fit.
			{
				float r = 0.1f;//distance left/right
				int sign = (attempt % 2 == 0) ? 1 : -1;//x%2==0 is even/odd check using "modulo" operator to see if evenly divisible by 2. the ? 1 : -1 is a "ternery" operator which will evaluate to 1 when whats before the ? is true, -1 when false.
				float extra = (attempt % 2 == 0) ? 0 : r;
				//in effect, sign should toggle between 1 and -1 as attempt increments.
				var pos = new Vector2(ballSpawnPosition.position.x + (attempt/2 * r * sign),ballSpawnPosition.position.y);
				if (ValidBallSpawnLocation(pos))
				{
					return pos;
					//return statement breaks the loop.
				}
				attempt++;
			}
			return ballSpawnPosition.position;
		}

		private bool ValidBallSpawnLocation(Vector2 point)
		{
			var col = Physics2D.OverlapCircle(point, 0.199f);
			return col == null;
		}

		public Ball CreateNewBall(bool destroyAllCurrent = true)
		{
			//Destroy current Ball
			if (destroyAllCurrent && _ballsInPlay.Count >0)
			{
				foreach (var ball in _ballsInPlay)
				{
					ball.RemoveBall();
				}
				_ballsInPlay.Clear();
			}
			
			
			var ballObject = GameObject.Instantiate(ballPrefab, GetValidSpawnPosition(), Quaternion.identity);
			var newBall = ballObject.GetComponent<Ball>();
			newBall.SetPongGameManager(this);//Dependency injection pattern
			_ballsInPlay.Add(newBall);
			return newBall;
		}

		//Called by the ball when it gets scored.
		//Score updated by the scorezone before this gets called
		//Some balls may or may not get replaced.
		public void OnBallRemovedFromPlay(bool replaceBall = true)
		{
			//check if the game is over
			if (replaceBall)
			{
				var newBall = CreateNewBall(false);
				newBall.FireBall(_delayBeforeBallLaunch);
			}
		}

		//On the principle of the thing, the game, architecturally, should work for any number of player components.
		//This sort of principle ends up being more useful then design flexibility, but helps with doing things like multiplayer, AI, or like cutscenes. We can't assume all the players exist at one point in time.
		//So, instead of "if playerA return B" and "if playerB return A", we do the linq thing.
		public PlayerData[] GetOtherPlayers(PlayerData player)
		{
			return _players.Where(x => x != player).ToArray();
		}
	}
}