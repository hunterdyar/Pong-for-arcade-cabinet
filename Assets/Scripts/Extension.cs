using UnityEngine;

namespace Pong
{
	public static class Extension
	{
		public static Vector2 Rotate(this Vector2 vector, float degrees)
		{
			var alpha = degrees * Mathf.Deg2Rad;
			return new Vector2(
				vector.x * Mathf.Cos(alpha) - vector.y * Mathf.Sin(alpha),
				vector.x * Mathf.Sin(alpha) + vector.y * Mathf.Cos(alpha)
			);
		}

	}
}