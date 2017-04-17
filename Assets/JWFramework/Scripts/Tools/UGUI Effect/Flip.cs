using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace JWFramework.UGUI
{
	public enum FlipMode
	{
		None,
		Horizontal,
		Vertical,
		Both,
	}

	[AddComponentMenu ("UI/Effects/Flip")]
	public class Flip : BaseMeshEffect
	{
		[SerializeField]
		private FlipMode flipMode = FlipMode.None;

		public override void ModifyMesh (VertexHelper vh)
		{
			if (!IsActive ()) {
				return;
			}
			
			if (flipMode == FlipMode.None) {
				return;
			}

			var count = vh.currentVertCount;
			if (count == 0)
				return;

			var vertexs = new List<UIVertex> ();
			for (var i = 0; i < count; i++) {
				var vertex = new UIVertex ();
				vh.PopulateUIVertex (ref vertex, i);
				vertexs.Add (vertex);
			}

			var leftX = vertexs [0].position.x;
			var rightX = vertexs [0].position.x;
			var topY = vertexs [0].position.y;
			var bottomY = vertexs [0].position.y;

			for (var i = 1; i < count; i++) {
				var x = vertexs [i].position.x;
				if (x > rightX) {
					rightX = x;
				} else if (x < leftX) {
					leftX = x;
				}
				var y = vertexs [i].position.y;
				if (y > topY) {
					topY = y;
				} else if (y < bottomY) {
					bottomY = y;
				}
			}
			
			var cx_2 = leftX + rightX;
			var cy_2 = topY + bottomY;

			switch (flipMode) {
			case FlipMode.Horizontal:
				{
					for (var i = 0; i < count; i++) {
						var vertex = vertexs [i];
						Vector3 newPosition = new Vector3 (cx_2 - vertex.position.x, vertex.position.y, vertex.position.z);
						vertex.position = newPosition;
						vh.SetUIVertex (vertex, i);
					}
				}
				break;
			case FlipMode.Vertical:
				{
					for (var i = 0; i < count; i++) {
						var vertex = vertexs [i];
						Vector3 newPosition = new Vector3 (vertex.position.x, cy_2 - vertex.position.y, vertex.position.z);
						vertex.position = newPosition;
						vh.SetUIVertex (vertex, i);
					}
				}
				break;
			case FlipMode.Both:
				{
					for (var i = 0; i < count; i++) {
						var vertex = vertexs [i];
						Vector3 newPosition = new Vector3 (cx_2 - vertex.position.x, cy_2 - vertex.position.y, vertex.position.z);
						vertex.position = newPosition;
						vh.SetUIVertex (vertex, i);
					}
				}
				break;
			}
		}
	}
}