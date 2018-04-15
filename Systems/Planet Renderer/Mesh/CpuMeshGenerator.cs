using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class CpuMeshGenerator : IMeshService {

        [Header("Generation Options")]
        public int resolution = 24;
		public bool useSkirts = false;
		[Range(0,1)]
		public float skirtSize = 0.9f;

		[Header("Surface Shape")]
		public NoiseOptions surfaceShape;
		[Range(-1,1)]
		public float seaLevel;
		public float continentHeight = 400;

		[Header("Surface Details")]
		public NoiseOptions hillNoise;
		public float hillHeight = 120;
		public NoiseOptions mountainNoise;
		public float mountainHeight = 800;
		[Range(0,1)]
		public float blendAmount = 0;

		public INoise noise = new Perlin();

		public float GetAltitude(Vector3 pos, float baseRadius){
			//Sample
			float surface = noise.Sum(pos, surfaceShape);

			float detail = noise.Sum (pos, hillNoise);
			float mountains = noise.Sum (pos, mountainNoise);
			float diff = mountains < detail ? 0 : (mountains - detail);

			//Blend
			float delta = detail * hillHeight;/*(detail > mountains) 
				? detail * hillHeight 
				: (diff < blendAmount 
					? Mathf.Lerp(detail * hillHeight, mountains * mountainHeight, Mathf.Clamp01(diff / blendAmount)) 
					: mountains * mountainHeight);*/
			float combined = (surface > seaLevel) ? (surface * continentHeight) + delta : (surface * continentHeight);

			//Assign
			return baseRadius + combined;
		}

		public Vector3 GetNormal(Vector3 pos, float baseRadius){
			//Default operation
			//return pos;

			//Setup
			float e = 0.0001f; //Some very small number
			Vector3 r = pos;

			//Position at the target sphere point
			Vector3 target = r * GetAltitude (r, baseRadius);

			//Rotate r by some small amount around the y-axis
			r = Quaternion.Euler (0, e, 0) * pos ; 
			Vector3 targetRight = r * GetAltitude (r, baseRadius);

			//r = Quaternion.Euler (0, -e, 0) * pos;
			//Vector3 targetLeft = r * GetAltitude (r, baseRadius);

			//Rotate r by some small amount in the "x-axis"
			Vector3 xaxis = Vector3.Cross(Vector3.up, pos).normalized;

			r = Quaternion.AngleAxis (e, xaxis) * pos;
			Vector3 targetUp = r * GetAltitude (r, baseRadius);

			//r = Quaternion.AngleAxis (-e, xaxis) * pos;
			//Vector3 targetDown = r * GetAltitude (r, baseRadius);

			//Create Normal Fragment Vectors
			//TODO Add Noraml Fragments From All Directions
			Vector3 up = (targetUp - target).normalized;
			Vector3 right = (targetRight - target).normalized;
			//Vector3 left = (targetLeft - target).normalized;
			//Vector3 down = (targetDown - target).normalized;

			Vector3 normal = -Vector3.Cross(right, up);
			//normal += Vector3.Cross(down, right);
			//normal += Vector3.Cross (up, left);
			//normal += Vector3.Cross (left, down);

			//Return "Averaged" Normal
			//normal = normal.normalized;
			return normal;
		}

        public override void Init() {

        }

        public override MeshData Make(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, Zone2 uvRange, float radius){
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

			Vector2 uvTopLeft = uvRange.a;
			Vector2 uvTopRight = uvRange.b;
			Vector2 uvBottomLeft = uvRange.d;
			Vector2 uvBottomRight = uvRange.c;

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
					float alt = GetAltitude(pos, radius);

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
					n[idx] = GetNormal(pos, radius);

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

			MeshData m = new MeshData();
			m.name = "Surface_r" + resolution;

			m.vertices = v;
			m.uvs = u;
			m.triangles = t;
			m.normals = n;

			return m;
		}

	}

}
