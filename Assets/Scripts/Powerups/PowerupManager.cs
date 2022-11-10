using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pong.Powerups
{
	[CreateAssetMenu(fileName = "PowerupManager", menuName = "Pong/PowerupManager", order = 0)]
	public class PowerupManager : ScriptableObject
	{
		[Header("Spawn Config")]
		[SerializeField] private bool _spawningEnabled;
		[SerializeField] private int _spawnCountAtStartRound;
		[SerializeField] private float _averageDelayBetweenSpawns;
		[SerializeField] [Range(0,1)] private float _randomDelayRange; 
		[SerializeField] private int _maxUncollectedOnScreen;
		[SerializeField] private int _minUncollectedOnScreen;
		[SerializeField] private Vector2 _minDistanceToEdges;
		[SerializeField] private GameObject _powerupPickupPrefab;
		private float _timeSinceLastPowerupSpawn;
		private float _spawnNextPickupTime;
		private List<PowerupPickup> _activePowerupCollectibles;
		
		[Header("Powerip Config")]
		[SerializeField] private PowerupDataMapping[] _dataMappings;
		private Dictionary<Powerup, PowerupDataMapping> _dataMap;
		
		public void Init()
		{
			_activePowerupCollectibles = new List<PowerupPickup>();
			_dataMap = new Dictionary<Powerup, PowerupDataMapping>();
			foreach (var pim in _dataMappings)
			{
				_dataMap.Add(pim.Powerup,pim);
			}
		}

		public void StartGame()
		{
			if (_spawningEnabled)
			{
				for (int i = 0; i < _spawnCountAtStartRound; i++)
				{
					SpawnPowerupPickup();
				}
			}
		}

		public void Tick()
		{
			if (_spawningEnabled)
			{
				_timeSinceLastPowerupSpawn += Time.deltaTime;
				if (_timeSinceLastPowerupSpawn >= _spawnNextPickupTime || _activePowerupCollectibles.Count < _minUncollectedOnScreen)
				{
					SpawnPowerupPickup();
				}
			}
		}

		private void SpawnPowerupPickup()
		{
			if (_activePowerupCollectibles.Count >= _maxUncollectedOnScreen)
			{
				return;
			}

			var bounds = Utility.GetXYScreenBoundsInWorldSpace();
			float x = Random.Range(bounds.min.x + _minDistanceToEdges.x,bounds.max.x - _minDistanceToEdges.x);
			float y = Random.Range(bounds.min.y + _minDistanceToEdges.y,bounds.max.y - _minDistanceToEdges.y);
			var pos = new Vector3(x, y, 0);
			var powerupPickup = Instantiate(_powerupPickupPrefab, pos, Quaternion.Euler(0, 0, Random.Range(0f, 360f))).GetComponent<PowerupPickup>();
			if (powerupPickup != null)
			{
				powerupPickup.SetPickup(GetRandomPowerup());
				_activePowerupCollectibles.Add(powerupPickup);
			}
			else
			{
				Debug.LogWarning("no powerupPickup component on collectible prefab?", _powerupPickupPrefab);
			}
			ResetPickupSpawnTimer();
			
		}

		private void ResetPickupSpawnTimer()
		{
			_timeSinceLastPowerupSpawn = 0;
			//random delay range is between 0 and 1, which will delay between 0 and 100% variation to the spawn time.
			float randomHalfOffset = _randomDelayRange * _averageDelayBetweenSpawns / 2;
			_spawnNextPickupTime = _averageDelayBetweenSpawns + Random.Range(-randomHalfOffset, randomHalfOffset);
		}

		public Powerup GetRandomPowerup()
		{
			//todo: if we want some powerups to appear more often than others, this would be the place for that.
			//We would do that with some weighting and use the sprite mppings to make an array with each item in the array x times, pluck a random one
			
			var asArray = Enum.GetValues(typeof(Powerup));
			int max = asArray.Length;
			//The first element of this array is "none", so we skip it. Range starts at 1 not 0.
			return (Powerup)asArray.GetValue(Random.Range(1, max));
		}

		public Sprite GetSprite(Powerup powerup)
		{
			if (_dataMap.TryGetValue(powerup, out var data))
			{
				return data.Sprite;
			}
			else
			{
				return null;
			}
		}

		public void OnPickup(PowerupPickup powerupPickup)
		{
			_activePowerupCollectibles.Remove(powerupPickup);
		}
	}
}