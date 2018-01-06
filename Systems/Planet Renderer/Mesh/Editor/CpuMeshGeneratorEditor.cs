using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Spaceworks{

	[CustomEditor(typeof(CpuMeshGenerator))]
	public class CpuMeshGeneratorEditor : Editor {

		private SixTexture surface;
		private SixTexture hills;
		private SixTexture mountains;

		public class SixTexture{

			private int x;
			private int y;

			private Texture2D top;
			private Texture2D bottom;
			private Texture2D left;
			private Texture2D right;
			private Texture2D front;
			private Texture2D back;

			private Color a;
			private Color b;

			public SixTexture(Color a, Color b, int x, int y){
				this.x = x;
				this.y = y;

				this.a = a;
				this.b = b;
			}

			private void TestExistance(){
				if (top == null) {
					top = new Texture2D (x, y);
					bottom = new Texture2D (x, y);
					left = new Texture2D (x, y);
					right = new Texture2D (x, y);
					front = new Texture2D (x, y);
					back = new Texture2D (x, y);
				}
			}

			public void Draw(NoiseOptions opts, float discrete = -1000){
				//Ensure textures exist
				TestExistance();

				//Generate Textures
				PerlinF perl = new PerlinF ();
				for(int i = 0; i < x; i++){
					for (int j = 0; j < y; j++) {
						float fx = Mathf.Lerp(-1, 1, (i / (x - 0.1f)));
						float fy = Mathf.Lerp (-1, 1, j / (y - 0.1f));

						Vector3 p = new Vector3 (fx, 0, fy);

						//float t = noise.value(p, n);
						float t = perl.Sum(p, opts).value;

						Color c = (discrete <= 1 && discrete >= -1) ? (t < discrete ? a : b) : Color.Lerp (a, b, (t + 1) * 0.5f);

						top.SetPixel (i, j, c);
					}
				}
				top.Apply ();

				//Draw Textures
				GUILayout.BeginVertical ();

				GUILayout.Label(top);

				GUILayout.BeginHorizontal ();

				//GUILayout.Label (back);
				//GUILayout.Label (right);
				//GUILayout.Label (front);
				//GUILayout.Label (left);

				GUILayout.EndHorizontal ();

				//GUILayout.Label (bottom);

				GUILayout.EndVertical ();
			}
		}

		public override void OnInspectorGUI () {
			//Get generator
			CpuMeshGenerator gen = (CpuMeshGenerator)target;

			//Ensure references
			if (surface == null)
				surface = new SixTexture (Color.blue, Color.green, 64, 64);
			if (hills == null)
				hills = new SixTexture (Color.black, Color.white, 64, 64);
			if (mountains == null)
				mountains = new SixTexture (Color.black, Color.white, 64, 64);

			//Draw image
			surface.Draw(gen.surfaceShape, gen.seaLevel);
			//hills.Draw (gen.hillNoise);
			//mountains.Draw (gen.mountainNoise);

			//Draw the default inspector
			DrawDefaultInspector();
		}
	}

}