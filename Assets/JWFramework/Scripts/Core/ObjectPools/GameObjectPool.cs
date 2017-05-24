using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Tools;

namespace JWFramework.Resource.Pool.Private
{
	public class GameObjectPool : GOPoolBase
	{
		public string resName;
		private ObjectPools recoveryRoot;

		public GameObjectPool (string resName, Object resource, int count, ObjectPools recoveryRoot) : base (recoveryRoot.transform)
		{
			this.resName = resName;
			this.prefab = resource;
			this.minCount = count;
			this.recoveryRoot = recoveryRoot;
			if (prefab != null) {
				for (int i = 0; i < count; i++) {
					GameObject res = InstantiatePrefab ();
					resPool.Add (res);
				}
				totalCound = count;
			}
		}

		protected override void WhenInstantiatePrefab (GameObject res)
		{
			var comp = res.AddMissingComponent<GOItem> ();
			comp.ResourceInit (resName, recoveryRoot);
		}
	}
}