using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class GlPlainLineRenderer : MonoBehaviour {

        public bool localSpace = false;
        public Color colour;
        public Vector3[] points;


        private Material mat;

        private void SetupMaterial() {
            if (!mat) {

                Shader shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                mat.hideFlags = HideFlags.HideAndDontSave;
                //Blending
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //Backface culling
                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                //Depth writes
                mat.SetInt("_ZWrite", 0);

            }
        }

        public void OnRenderObject() {
            SetupMaterial();

            mat.SetPass(0);

            GL.PushMatrix();

            if (localSpace) {
                GL.MultMatrix(transform.localToWorldMatrix);
            }

            GL.Begin(GL.LINES);
            GL.Color(colour);
            if (points != null && points.Length > 1)
                for (int i = 0; i < points.Length; i += 2) {
                    GL.Vertex3(points[i].x, points[i].y, points[i].z);
                    GL.Vertex3(points[i + 1].x, points[i + 1].y, points[i + 1].z);
                }

            GL.End();
            GL.PopMatrix();
        }

    }

}