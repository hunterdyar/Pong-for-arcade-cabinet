using UnityEngine;

namespace Pong.Pooling
{
	public abstract class PooledObject : MonoBehaviour
	{
		public GameObjectPool.PoolEvent ReturnToPool;

		public virtual void ResetAsNew(Vector3 position, Quaternion rotation, Transform parent)
		{
			transform.position = position;
			transform.rotation = rotation;
			transform.SetParent(parent);
		}
	}
}