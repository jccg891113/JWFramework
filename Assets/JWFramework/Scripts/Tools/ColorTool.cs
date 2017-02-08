using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JWFramework.Tools
{
	public class ColorTool
	{
		public static Color Color16 (string colorValue)
		{
			int r, g, b, a;
			if (colorValue.Length == 6 || colorValue.Length == 8) {
				r = Convert.ToInt32 (colorValue.Substring (0, 2), 16);
				g = Convert.ToInt32 (colorValue.Substring (2, 2), 16);
				b = Convert.ToInt32 (colorValue.Substring (4, 2), 16);
				a = 255;
				if (colorValue.Length == 8) {
					a = Convert.ToInt32 (colorValue.Substring (6, 2), 16);
				}
				return Color10 (r, g, b, a);
			} else {
				throw new Exception ("16 Color Length Error");
			}
		}

		public static Color Color10 (int r, int g, int b, int a = 255)
		{
			return new Color (r / 255f, g / 255f, b / 255f, a / 255f);
		}
	}
}
