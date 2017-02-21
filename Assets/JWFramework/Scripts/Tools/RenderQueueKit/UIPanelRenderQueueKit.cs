using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Scene
{
	public class UIPanelRenderQueueKit : MonoBehaviour
	{
//		public bool needAllChildPanelDepth = true;
//		[SerializeField]
//		int defaultQueue = 3000;
//		[SerializeField]
//		List<UIPanel> parentPanels = new List<UIPanel> ();
//
//		// Use this for initialization
//		void Start ()
//		{
//			ResetScale ();
//			ResetParentPanel ();
//			if (parentPanels != null && parentPanels.Count > 0) {
//				defaultQueue = GetPanelMaxRender (parentPanels);
//				RenderQueueKit.Change (gameObject, defaultQueue);
//			}
//		}
//
//		private void ResetScale ()
//		{
//			float wph = (float)Screen.width / (float)Screen.height;
//			float baseWph = 16f / 9f;
//			if (wph < baseWph) {
//				float baseHeight = Screen.width * 9f / 16f;
//				float percent = baseHeight / (float)Screen.height;
//				var par = GetComponentsInChildren<ParticleSystem> ();
//				if (par.Length > 0) {
//					var trans = GetComponentsInChildren<Transform> ();
//					foreach (var item in trans) {
//						var tp = item.GetComponent<ParticleSystem> ();
//						if (tp != null) {
//							tp.startSize = tp.startSize * percent;
//						} else {
//							item.localScale = item.localScale * percent;
//						}
//					}
//				}
//			}
//			var particleRoot = GetComponent<ParticleSystem> ();
//			if (particleRoot != null) {
//				particleRoot.Stop (true);
//				particleRoot.Play (true);
//			}
//		}
//
//		private void ResetParentPanel ()
//		{
//			UIPanel parentPanel = gameObject.GetComponentInParent<UIPanel> ();
//			if (needAllChildPanelDepth) {
//				parentPanels = new List<UIPanel> (parentPanel.GetComponentsInChildren<UIPanel> ());
//			} else {
//				parentPanels.Clear ();
//				parentPanels.Add (parentPanel);
//			}
//		}
//	
//		// Update is called once per frame
//		void Update ()
//		{
//			if (parentPanels != null && parentPanels.Count > 0) {
//				int tempQueue = GetPanelMaxRender (parentPanels);
//				if (tempQueue != defaultQueue) {
//					RenderQueueKit.Change (gameObject, tempQueue);
//					defaultQueue = tempQueue;
//				}
//			} else {
//				ResetParentPanel ();
//			}
//		}
//
//		private int GetPanelMaxRender (List<UIPanel> panels)
//		{
//			if (panels != null && panels.Count > 0) {
//				int tempQueue = parentPanels [0].startingRenderQueue + parentPanels [0].drawCalls.Count;
//				for (int i = 1, imax = parentPanels.Count; i < imax; i++) {
//					tempQueue = System.Math.Max (tempQueue, parentPanels [i].startingRenderQueue + parentPanels [0].drawCalls.Count);
//				}
//				return tempQueue;
//			} else {
//				return 3000;
//			}
//		}
//
//		public void SetNeedAllChildPanel (bool newValue)
//		{
//			if (needAllChildPanelDepth != newValue) {
//				ResetParentPanel ();
//			}
//			needAllChildPanelDepth = newValue;
//		}
	}
}