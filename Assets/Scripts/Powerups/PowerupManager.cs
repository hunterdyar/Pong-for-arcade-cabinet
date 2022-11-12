using System;
using System.Collections.Generic;
using System.Linq;
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
		
		[Header("Powerup Config")]
		[InspectorName("Powerup Data")]
		[SerializeField] private PowerupDataMapping[] _dataMappings;
		private Dictionary<Powerup, PowerupDataMapping> _dataMap;
		private float _randomWeightTotal;
		public void Init()
		{
			_activePowerupCollectibles = new List<PowerupPickup>();
			_dataMap = new Dictionary<Powerup, PowerupDataMapping>();
			_randomWeightTotal = 0;
			foreach (var pim in _dataMappings)
			{
				_dataMap.Add(pim.Powerup,pim);
				_randomWeightTotal = _randomWeightTotal + pim.Weight;
				if (pim.Weight == 0)
				{
					Debug.LogWarning($"{pim.Powerup} has a weight of 0. Will not be chosen.");
				}
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

		[ContextMenu("Get Random Powerup")]
		public Powerup GetRandomPowerup()
		{
			//this doesnt work of the weights change during runtime. I have a feeling that having weights change would be nice, as we go from early to late-game.
			//if so, we have to recalculate the total here:
			// _randomWeightTotal = _dataMappings.Sum(x => x.Weight);
			
			var choice = Random.Range(0f,_randomWeightTotal);
			
			foreach (var p in _dataMappings)
			{
				choice = choice - p.Weight;
				if (choice <= 0)
				{
					return p.Powerup;
				}
			}

			// Guessing Fence post problem??
			Debug.LogError("Random powerup selection failed? Are weights negative? Did things change during runtime?");
			return _dataMappings[_dataMappings.Length-1].Powerup;//eh just give us the last one. 
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