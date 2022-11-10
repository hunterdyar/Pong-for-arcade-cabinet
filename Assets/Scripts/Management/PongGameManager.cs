using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pong;
using Pong.Powerups;
using UnityEngine;

namespace Pong
{
	public class PongGameManager : MonoBehaviour
	{
		//This static action is the only real "singleton-ey" thing we have going on right now. It would be easy to move this into a scriptable-object-architecture style event system for game state.
		//I prefer that, but the project just doesn't have the scope to warrant it. keeping it simple until I need to change.
		public static Action<GameState> OnGameStateChange;
		public static Action<PlayerData> OnPlayerWon; 
			
		[Header("Scene Config")]
		[SerializeField] private Transform ballSpawnPosition;
		[SerializeField] private Countdown _countdown;
		
		//State machine should always go in order. Setup->countdown->gameplay->game over. It won't loop back to the start... because we will just reload the scene instead!
		public GameState GameState => _gameState;
		private GameState _gameState;

		[Header("Game Config")]
		[SerializeField]
		private float _delayBeforeBallLaunch;
		[Min(1)] [SerializeField]
		private int _startingBallCount;

		[SerializeField] private int scoreToWin;
		
		[Header("Data Setup")]
		[SerializeField] private GameObject ballPrefab;
		[SerializeField] private PowerupManager _powerupManager;
		
		private readonly List<Ball> _ballsInPlay = new List<Ball>();

		[SerializeField] private PlayerData[] _players;
		private void Awake()
		{
			//Idle state. This will probably not even bother to fire the event: there are no listeners yet. Things can set themselves up on awake/start, we reload scenes to restart.
			EnterGameState(GameState.Setup);
			_powerupManager.Init();

			foreach (var player in _players)
			{
				player.SetPongGameManager(this);
			}
		}

		private void OnEnable()
		{
			Countdown.OnCountdownGo += OnCountdownGo;
			PlayerData.AnyPlayerScoreChange += CheckForVictory;
		}

		private void OnDisable()
		{
			Countdown.OnCountdownGo -= OnCountdownGo;
			PlayerData.AnyPlayerScoreChange -= CheckForVictory;
		}

		private void Start()
		{
			for (int i = 0; i < _startingBallCount; i++)
			{
				CreateNewBall(false);
			}
			
			//initialize score to 0, etc.
			foreach (var player in _players)
			{
				player.ResetPlayerData();
			}
			
			EnterGameState(GameState.Countdown);
			_countdown.StartCountdown();
			_powerupManager.StartGame();
		}

		private void EnterGameState(GameState state)
		{
			if (_gameState != state)
			{
				_gameState = state;
				OnGameStateChange?.Invoke(_gameState);
			}
		}

		void Update()
		{
			if (_gameState == GameState.Gameplay)
			{
				_powerupManager.Tick();
			}
		}

		//Todo: Refactor this so a Countdown object handles everything and then calls back to the game manager
		private void OnCountdownGo()
		{
			EnterGameState(GameState.Gameplay);
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
			//If it ever becomes possible to get points without the ball going into the zone, doign the check there breaks.
			// Since that's an idea that we had (like coin/score pickups or such), we instead subscribe and do checkForVictory to a playeraction to check as a result of the player score changing. So it will just always work.
			// CheckForVictory();
			if (replaceBall && _gameState == GameState.Gameplay)
			{
				var newBall = CreateNewBall(false);
				newBall.FireBall(_delayBeforeBallLaunch);
			}
		}

		//This function does not assume we know who called it or why. Most of the time it's a player score change, and we could pass that in instead of looping through all players.
		//but a) there are only two players, it's whatever and b) rather just have one bit of logic for the code and not deal with edge-cases.
		private void CheckForVictory()
		{
			if (_gameState != GameState.Gameplay)
			{
				return;
			}
			
			foreach (var player in _players)
			{
				if (player.Score >= scoreToWin)
				{
					//Interestingly, we don't keep track of who won or save that anywhere. We shouldn't need to! The next thing that will happen is the game restarts.
					OnPlayerWon?.Invoke(player);
					EnterGameState(GameState.PlayerWon);
					break;
				}
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