using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Class for building subdivided planes as a unity mesh
    /// </summary>
    public class SubPlane {

        public static readonly Vector3 topLeft = new Vector3(-1, 0, 1);
        public static readonly Vector3 topRight = new Vector3(1, 0, 1);
        public static readonly Vector3 bottomLeft = new Vector3(-1, 0, -1);
        public static readonly Vector3 bottomRight = new Vector3(1, 0, -1);

        public static readonly Vector2 uvTopLeft = new Vector2(0, 0);
        public static readonly Vector2 uvTopRight = new Vector2(0, 1);
        public static readonly Vector2 uvBottomLeft = new Vector2(1, 0);
        public static readonly Vector2 uvBottomRight = new Vector2(1, 1);

        /// <summary>
        /// Create a subdivided plane
        /// </summary>
        /// <param name="subdivisions"></param>
        /// <returns></returns>
        public static Mesh Make(int subdivisions) {

            return Make(
                topLeft, topRight, bottomLeft, bottomRight, 
                uvTopLeft, uvTopRight, uvBottomLeft, uvBottomRight,
                subdivisions
            );

        }

        /// <summary>
        /// Create a subdivided plane with the given corner coordinates
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="subdivisions"></param>
        /// <returns></returns>
        public static Mesh Make(
            Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight,
            Vector2 uvTopLeft, Vector2 uvTopRight, Vector2 uvBottomLeft, Vector2 uvBottomRight,
            int subdivisions
        ) {
            Vector3 normal = Vector3.Cross(
                (topLeft - topRight).normalized,
                (bottomRight - topRight).normalized
            ).normalized;

            int width = subdivisions + 2;
            int size = width * width;
            Vector3[] v = new Vector3[size];
            Vector3[] n = new Vector3[size];
            Vector2[] u = new Vector2[size];
            List<int> t = new List<int>();

            float step = 1.0f / (subdivisions + 1);

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < width; j++) {
                    int idx = i + width * j;

                    //Create Vertice
                    Vector3 pos = Vector3.Lerp(
                        Vector3.Lerp(topLeft, topRight, i * step),
                        Vector3.Lerp(bottomLeft, bottomRight, i * step),
                        j * step
                    );
                    v[idx] = pos;

                    //Create uv
                    Vector2 uv = Vector2.Lerp(
                        Vector2.Lerp(uvTopLeft, uvTopRight, i * step),
                        Vector2.Lerp(uvBottomLeft, uvBottomRight, i * step),
                        j * step
                    );
                    u[idx] = uv;

                    //Create normals
                    n[idx] = normal;

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

            Mesh m = new Mesh();
            m.name = "SubPlane_" + subdivisions;

            m.vertices = v;
            m.uv = u;
            m.triangles = t.ToArray();
            m.normals = n;

            return m;

        }

    }

}