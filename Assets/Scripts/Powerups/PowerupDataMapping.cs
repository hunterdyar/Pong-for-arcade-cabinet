using System;
using UnityEngine;

namespace Pong.Powerups
{
	[Serializable]
	public struct PowerupDataMapping
	{
		public Powerup Powerup;
		public Sprite Sprite;
		[Min(0)][Tooltip("Number of beans in the bag. If all weights are the same number, random chance of selection is identical.")]
		public float Weight;
	}
}