using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace JWFramework.UGUI
{
	[AddComponentMenu ("UI/Effects/Text Bold")]
	public class Bold : BaseMeshEffect
	{
		public int boldCount = 4;

		public override void ModifyMesh (VertexHelper vh)
		{
			if (!IsActive ()) {
				return;
			}
			var count = vh.currentVertCount;
			if (count == 0 || count % 4 != 0)
				return;

			var vertexs = new List<UIVertex> ();
			for (var i = 0; i < count; i++) {
				var vertex = new UIVertex ();
				vh.PopulateUIVertex (ref vertex, i);
				vertexs.Add (vertex);
			}
			vh.Clear ();
			for (int i = 0; i < boldCount; i++) {
				for (int j = 0; j < count; j++) {
					var vertex = vertexs [j];
					vh.AddVert (vertex.position, vertex.color, vertex.uv0, vertex.uv1, vertex.normal, vertex.tangent);
					if (j % 4 == 3) {
						vh.AddTriangle (j - 3, j - 2, j - 1);
						vh.AddTriangle (j - 1, j, j - 3);
					}
				}
			}
		}
	}
}