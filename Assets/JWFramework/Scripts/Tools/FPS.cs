using UnityEngine;
using System.Collections;

namespace JWFramework.Tools
{
	public class FPS : MonoBehaviour
	{
		public float m_fUpdateInterval = 0.5F;
		private float m_fLastInterval;
		private int m_iFrames = 0;
		private float m_fFps;
		public static string str = "";

		void Start ()
		{
			m_fLastInterval = Time.realtimeSinceStartup;
			m_iFrames = 0;
		}

		void OnGUI ()
		{
			if (true) {
				GUI.color = Color.red;
				GUIStyle style = new GUIStyle ();
				style.fontSize = 20;
				style.normal.textColor = Color.red;
				if (str != "") {
					GUI.Label (new Rect (0, 200, 400, 400), "FPS:" + str, style);
				} else {
					GUI.Label (new Rect (0, 200, 400, 400), "FPS:" + m_fFps.ToString ("f2"), style);
				}
			}
		}

		void Update ()
		{
			m_iFrames++;			
			if (Time.realtimeSinceStartup > m_fLastInterval + m_fUpdateInterval) {
				m_fFps = m_iFrames / (Time.realtimeSinceStartup - m_fLastInterval);
				m_iFrames = 0;
				m_fLastInterval = Time.realtimeSinceStartup;
			}
		}
	}
}