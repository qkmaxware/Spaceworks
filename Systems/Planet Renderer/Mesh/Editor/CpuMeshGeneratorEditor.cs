using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Spaceworks {

	[CustomEditor (typeof(CpuMeshGenerator))]
	public class CpuMeshGeneratorEditor : Editor {

		private SixTexture surface;
		private SixTexture hills;
		private SixTexture mountains;

		public class SixTexture {

			public enum Face {
				Top,
				Bottom,
				Left,
				Right,
				Front,
				Back
			}

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

			public SixTexture (Color a, Color b, int x, int y) {
				this.x = x;
				this.y = y;

				this.a = a;
				this.b = b;
			}

			private void TestExistance () {
				if (top == null) {
					top = new Texture2D (x, y);
					bottom = new Texture2D (x, y);
					left = new Texture2D (x, y);
					right = new Texture2D (x, y);
					front = new Texture2D (x, y);
					back = new Texture2D (x, y);
				}
			}

			private void FillTexture (Texture2D t, Face face, NoiseOptions opts, float discrete = -1000) {
				float w = t.width - 1;
				float h = t.height - 1;
				Perlin perl = new Perlin ();

				for (int i = 0; i <= w; i++) {
					for (int j = 0; j <= h; j++) {
						float xp = 0, yp = 0, zp = 0;
                
						switch (face) {
							case Face.Top:
								yp = 1;
								xp = (i / w) * 2 - 1;
								zp = 1 - (j / h) * 2;
								break;
							case Face.Bottom:
								yp = -1;
								xp = (i / w) * 2 - 1;
								zp = (j / h) * 2 - 1;
								break;
							case Face.Left:
								xp = -1;
								zp = 1 - (i / w) * 2;
								yp = 1 - (j / h) * 2;
								break;
							case Face.Back:
								zp = -1;
								xp = (i / w) * 2 - 1;
								yp = 1 - (j / h) * 2;
								break;
							case Face.Right:
								xp = 1;
								zp = (i / w) * 2 - 1;
								yp = 1 - (j / h) * 2;
								break;
							case Face.Front:
								zp = 1;
								xp = 1 - (i / w) * 2;
								yp = 1 - (j / h) * 2;
								break;
						}
                
						Vector3 pos = IMeshService.Spherify (new Vector3(xp, yp, zp));
                
						float n = perl.Sum (pos, opts);
						t.SetPixel(i,j, (discrete <= 1 && discrete >= -1) ? (n < discrete ? a : b) : Color.Lerp (a, b, (n + 1) * 0.5f));
					}
				}

			}

			public void Draw (NoiseOptions opts, float discrete = -1000) {
				//Ensure textures exist
				TestExistance ();

				//Generate Textures
				FillTexture(top, Face.Top, opts, discrete);
				FillTexture(bottom, Face.Bottom, opts, discrete);
				FillTexture(left, Face.Left, opts, discrete);
				FillTexture(right, Face.Right, opts, discrete);
				FillTexture(front, Face.Front, opts, discrete);
				FillTexture(back, Face.Back, opts, discrete);

				//Apply textures
				top.Apply ();
				bottom.Apply ();
				left.Apply ();
				right.Apply ();
				front.Apply();
				back.Apply ();

				//Draw Textures
				GUILayout.BeginVertical ();

				GUILayout.Label (top);

				GUILayout.BeginHorizontal ();

				GUILayout.Label (back);
				GUILayout.Label (right);
				GUILayout.Label (front);
				GUILayout.Label (left);

				GUILayout.EndHorizontal ();

				GUILayout.Label (bottom);

				GUILayout.EndVertical ();
			}
		}

		public override void OnInspectorGUI () {
			//Get generator
			CpuMeshGenerator gen = (CpuMeshGenerator)target;

			//Ensure references
			//if (surface == null)
				//surface = new SixTexture (Color.blue, Color.green, 64, 64);
			//if (hills == null)
				//hills = new SixTexture (Color.black, Color.white, 64, 64);
			//if (mountains == null)
				//mountains = new SixTexture (Color.black, Color.white, 64, 64);

			//Draw image
			//surface.Draw (gen.surfaceShape, gen.seaLevel);
			//hills.Draw (gen.hillNoise);
			//mountains.Draw (gen.mountainNoise);

			//Draw the default inspector
			DrawDefaultInspector ();
		}
	}

}