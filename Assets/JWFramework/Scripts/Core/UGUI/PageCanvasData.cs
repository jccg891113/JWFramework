using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	[System.Serializable]
	public class PageCanvasData
	{
		Dictionary<Canvas, int> allCanvasBaseOrder;
		int additionalOrder;

		public int minOrder{ get; private set; }

		public int maxOrder{ get; private set; }

		public int minAddOrder{ get { return additionalOrder + minOrder; } }

		public int MaxAddOrder{ get { return additionalOrder + maxOrder; } }

		public int deltaOrder{ get; private set; }

		public PageCanvasData (Canvas[] allCanvas)
		{
			minOrder = int.MaxValue;
			maxOrder = int.MinValue;
			allCanvasBaseOrder = new Dictionary<Canvas, int> ();
			for (int i = 0, imax = allCanvas.Length; i < imax; i++) {
				var canvas = allCanvas [i];
				int baseOrder = canvas.sortingOrder;
				allCanvasBaseOrder [canvas] = baseOrder;
				minOrder = System.Math.Min (minOrder, baseOrder);
				maxOrder = System.Math.Max (maxOrder, baseOrder);
			}
			deltaOrder = maxOrder - minOrder;
			additionalOrder = 0;
		}

		public void ResetAddtionalOrder (int newAdditionalOrder)
		{
			additionalOrder = newAdditionalOrder;
			foreach (var pair in allCanvasBaseOrder) {
				var canvas = pair.Key;
				int baseOrder = pair.Value;
				canvas.sortingOrder = baseOrder + additionalOrder;
			}
		}
	}
}