using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks{

	public class CubemapMeshGenerator : IMeshService {

		public enum ColourComponent{
			Red, Green, Blue, Alpha
		}

		public bool useSkirts = false;
		[Range(0,1)]
		public float skirtSize = 0.9f;
		public HighLowPair range;
		public Cubemap heightmap;
		public ColourComponent heightColour;

		private float SampleColour(Color colour){
			switch (heightColour) {
				case ColourComponent.Alpha:
					return colour.a;
				case ColourComponent.Blue:
					return colour.b;
				case ColourComponent.Green:
					return colour.g;
				default:
					return colour.r;
			}
		}

		private float SampleHeightmap(Vector3 pos){
			pos = new Vector3 (Mathf.Clamp (pos.x, -1, 1), Mathf.Clamp (pos.y, -1, 1), Mathf.Clamp (pos.z, -1, 1));
			float dot = float.MinValue;
			CubemapFace face = CubemapFace.PositiveX;

			//Determine closest face
			float tmp = Vector3.Dot (pos, Vector3.up);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.PositiveY;
			}

			tmp = Vector3.Dot (pos, Vector3.down);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.NegativeY;
			}

			tmp = Vector3.Dot (pos, Vector3.right);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.PositiveX;
			}

			tmp = Vector3.Dot (pos, Vector3.left);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.NegativeX;
			}

			tmp = Vector3.Dot (pos, Vector3.forward);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.PositiveZ;
			}

			tmp = Vector3.Dot (pos, Vector3.back);
			if (tmp > dot) {
				dot = tmp;
				face = CubemapFace.NegativeZ;
			}
		
			//Sample
			float value = 0;
			switch(face){
				//Right
				case CubemapFace.PositiveX:
					float x = ((pos.z + 1) * 0.5f) * heightmap.width;
					float y = (1 - (pos.y + 1) * 0.5f) * heightmap.height;
					Color c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;
				//Left
				case CubemapFace.NegativeX:
					x = (1 - (pos.z + 1) * 0.5f) * heightmap.width;
					y = (1 - (pos.y + 1) * 0.5f) * heightmap.height;
					c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;

				//Up
				case CubemapFace.PositiveY:
					x = ((pos.x + 1) * 0.5f) * heightmap.width;
					y = (1 - (pos.z + 1) * 0.5f) * heightmap.height;
					c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;
				//Down
				case CubemapFace.NegativeY:
					x = ((pos.x + 1) * 0.5f) * heightmap.width;
					y = ((pos.z + 1) * 0.5f) * heightmap.height;
					c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;

				//Forward
				case CubemapFace.PositiveZ:
					x = (1 - (pos.x + 1) * 0.5f) * heightmap.width;
					y = (1 - (pos.y + 1) * 0.5f) * heightmap.height;
					c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;
				//Back
				case CubemapFace.NegativeZ:
					x = ((pos.x + 1) * 0.5f) * heightmap.width;
					y = (1 - (pos.y + 1) * 0.5f) * heightmap.height;
					c = heightmap.GetPixel (face, (int)x, (int)y);
					value = SampleColour (c);
					break;
			}

			return value;
		}

		public override float GetAltitude (Vector3 pos, float baseRadius, out Vector3 normal)
		{
			normal = pos;
			return Mathf.Lerp(range.low, range.high, SampleHeightmap (pos)) + baseRadius;
		}

		public override Mesh Make (Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, int resolution, float radius)
		{
			//Initial Calculations
			int width = resolution + 2;
			int size = width * width;
			int skirtVertices = useSkirts ? (width * 4) : 0;//4 + resolution * 4;
			Vector3[] v = new Vector3[size + skirtVertices];
			Vector3[] n = new Vector3[size + skirtVertices];
			Vector2[] u = new Vector2[size + skirtVertices];
			int[] t = new int[
				((2 * (width - 1) * (width - 1)) * 3) + 
				(useSkirts ? (6 * (skirtVertices - 1)) : 0)
			];

			//Initial Representivitive Normals and UVs 
			Vector3 topNormal = (topLeft - bottomLeft).normalized;
			Vector3 rightNormal = (topRight - topLeft).normalized;

			Vector2 uvTopLeft = new Vector2(0, 0);
			Vector2 uvTopRight = new Vector2(0, 1);
			Vector2 uvBottomLeft = new Vector2(1, 0);
			Vector2 uvBottomRight = new Vector2(1, 1);

			float step = 1.0f / (resolution + 1);

			//Create Planar Faces
			int tidx = 0;
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < width; j++) {
					int idx = i + width * j;

					//Create Vertice on Cube
					Vector3 rawPosition = Vector3.Lerp(
						Vector3.Lerp(topLeft, topRight, i * step),
						Vector3.Lerp(bottomLeft, bottomRight, i * step),
						j * step
					);
					Vector3 pos = IMeshService.Spherify(rawPosition);

					//Sample Noise to Get Altitude
					Vector3 norm;
					float alt = GetAltitude(pos, radius, out norm);

					v [idx] = pos * alt;

					//Create UV for Texturing (optional)
					Vector2 uv = Vector2.Lerp(
						Vector2.Lerp(uvTopLeft, uvTopRight, i * step),
						Vector2.Lerp(uvBottomLeft, uvBottomRight, i * step),
						j * step
					);
					u[idx] = uv;

					//Create Normals
					//TODO take terrain shape into account
					n[idx] = pos;

					//Create Triangles
					if (i > 0 && j > 0) {
						t[tidx++] = ((i - 1) + width * (j - 1));
						t[tidx++] = ((i) + width * (j - 1));
						t[tidx++] = (idx);

						t[tidx++] = ((i - 1) + width * (j - 1));
						t[tidx++] = (idx);
						t[tidx++] = ((i - 1) + width * (j));
					}
				}
			}

			//Create Skirts
			if (useSkirts) {
				for (int i = 0; i < width; i++) {
					//Top 
					int idx = i;
					Vector3 p = v [idx];
					Vector2 uv = u [idx];
					Vector3 norm = topNormal;
					v [size + i] = p * skirtSize;
					n [size + i] = norm;
					u [size + i] = uv;
					if (i != 0) {
						t [tidx++] = idx;
						t [tidx++] = idx - 1;
						t [tidx++] = size + (i - 1);

						t [tidx++] = idx;
						t [tidx++] = size + (i - 1);
						t [tidx++] = size + i;
					}

					//Bottom
					idx = i + width * (width - 1);
					p = v [idx];
					uv = u [idx];
					norm = -topNormal;
					v [size + width + i] = p * skirtSize;
					n [size + width + i] = norm;
					u [size + width + i] = uv;
					if (i != 0) {
						t [tidx++] = idx - 1;
						t [tidx++] = idx;
						t [tidx++] = size + (width + i);

						t [tidx++] = idx - 1;
						t [tidx++] = size + (width + i);
						t [tidx++] = size + (width + i - 1);
					}

					//Left
					idx = 0 + width * (i);
					p = v [idx];
					uv = u [idx];
					norm = -rightNormal;
					v [size + 2*width + i] = p * skirtSize;
					n [size + 2*width + i] = norm;
					u [size + 2*width + i] = uv;
					if (i != 0) {
						t [tidx++] = width * (i - 1);
						t [tidx++] = idx;
						t [tidx++] = size + (2*width + i);

						t [tidx++] = width * (i - 1);
						t [tidx++] = size + (2*width + i);
						t [tidx++] = size + (2*width + i - 1);
					}

					//Right
					idx = (width - 1) + width * i;
					p = v [idx];
					uv = u [idx];
					norm = rightNormal;
					v [size + 3*width + i] = p * skirtSize;
					n [size + 3*width + i] = norm;
					u [size + 3*width + i] = uv;
					if (i != 0) {
						t [tidx++] = idx;
						t [tidx++] = (width - 1) + width * (i - 1);
						t [tidx++] = size + (3*width + i - 1);

						t [tidx++] = idx;
						t [tidx++] = size + (3*width + i - 1);
						t [tidx++] = size + (3*width + i);
					}

				}
			}

			Mesh m = new Mesh();
			m.name = "Surface_r" + resolution;

			m.vertices = v;
			m.uv = u;
			m.triangles = t;
			m.normals = n;

			return m;
		}

	}
}