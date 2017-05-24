using UnityEngine;
using System.Collections;

namespace JWFramework.Resource.Pool
{
	public class GOItem : MonoBehaviour
	{
		[SerializeField]
		public string resourceName;
		public ObjectPools recoveryRoot;

		public void ResourceInit (string resourceName, ObjectPools recoveryRoot)
		{
			this.resourceName = resourceName;
			this.recoveryRoot = recoveryRoot;
		}

		public void Recovery ()
		{
			recoveryRoot.Recovery (resourceName, gameObject);
		}
	}
}