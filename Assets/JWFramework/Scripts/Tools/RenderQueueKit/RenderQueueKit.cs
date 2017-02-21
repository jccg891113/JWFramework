using UnityEngine;
using System.Collections;

namespace Scene
{
	public class RenderQueueKit : MonoBehaviour
	{
		public int queue = 2850;
		
		void OnEnable ()
		{
			queue = GetQueueValue ();
			Change (gameObject, queue);
		}

		protected virtual int GetQueueValue ()
		{
			return queue;
		}

		public static void Change (GameObject obj, int queue)
		{
			var renders = obj.GetComponentsInChildren<Renderer> ();
			foreach (var render in renders) {
				if (render != null) {
					foreach (Material material in render.materials) {
						if (material != null) {
							material.renderQueue = queue;
						}
					}
				}
			}
		}
	}
}