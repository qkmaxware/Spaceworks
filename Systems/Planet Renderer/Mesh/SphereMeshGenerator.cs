using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class SphereMeshGenerator : IMeshService {

        public int subdivisions = 24;

		public float GetAltitude (Vector3 pos, float baseRadius)
		{
			return baseRadius;
		}

		public Vector3 GetNormal(Vector3 pos, float baseRadius){
			return pos;
		}

        public override void Init() {

        }

        public override MeshData Make(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, Zone2 uvRange, float radius) {
            int width = subdivisions + 2;
            int size = width * width;
            Vector3[] v = new Vector3[size];
            Vector3[] n = new Vector3[size];
            Vector2[] u = new Vector2[size];
            List<int> t = new List<int>();

            Vector2 uvTopLeft = uvRange.a;
			Vector2 uvTopRight = uvRange.b;
			Vector2 uvBottomLeft = uvRange.d;
			Vector2 uvBottomRight = uvRange.c;
            
            float step = 1.0f / (subdivisions + 1);

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < width; j++) {
                    int idx = i + width * j;

                    //Create Vertice
                    Vector3 rawPosition = Vector3.Lerp(
                        Vector3.Lerp(topLeft, topRight, i * step),
                        Vector3.Lerp(bottomLeft, bottomRight, i * step),
                        j * step
                    );
					Vector3 pos = IMeshService.Spherify(rawPosition);
					v[idx] = pos * GetAltitude (pos, radius);;

                    //Create uv
                    Vector2 uv = Vector2.Lerp(
                        Vector2.Lerp(uvTopLeft, uvTopRight, i * step),
                        Vector2.Lerp(uvBottomLeft, uvBottomRight, i * step),
                        j * step
                    );
                    u[idx] = uv;

                    //Create normals
					n[idx] = GetNormal(pos, radius);

                    //Create triangles
                    if (i > 0 && j > 0) {
                        t.Add((i - 1) + width * (j - 1));
                        t.Add((i) + width * (j - 1));
                        t.Add(idx);

                        t.Add((i - 1) + width * (j - 1));
                        t.Add(idx);
                        t.Add((i - 1) + width * (j));
                    }
                }
            }

            MeshData m = new MeshData();
            m.name = "Surface_r" + subdivisions;

            m.vertices = v;
            m.uvs = u;
            m.triangles = t.ToArray();
            m.normals = n;

            return m;
        }

    }
}
