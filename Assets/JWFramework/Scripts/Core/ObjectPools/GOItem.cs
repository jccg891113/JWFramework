using UnityEngine;
using System.Collections;

namespace JWFramework.Resource.Pool
{
	public class GOItem : MonoBehaviour
	{
		[SerializeField]
		string resourceName;

		public void ResourceInit (string resourceName)
		{
			this.resourceName = resourceName;
		}

		public void Recovery ()
		{
			ObjectPools.Default.Recovery (resourceName, gameObject);
		}
	}
}