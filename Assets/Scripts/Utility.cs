
	using UnityEngine;

	public static class Utility
	{
		public static Bounds GetXYScreenBoundsInWorldSpace(Camera camera, float padding = 0)
		{
			var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
			var topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, camera.nearClipPlane));
			var center = Vector3.Lerp(bottomLeft, topRight, 0.5f);
			var size = (topRight - bottomLeft);
			//todo: fix z
			return new Bounds(center, size - new Vector3(padding,padding,0));
		}

		public static Bounds GetXYScreenBoundsInWorldSpace(float padding = 0)
		{
			if (Camera.main != null)
			{
				return GetXYScreenBoundsInWorldSpace(Camera.main, padding);
			}

			Debug.LogError("Can't get world bounds without camera.");
			return new Bounds(Vector3.zero, Vector3.zero);
		}
	}
