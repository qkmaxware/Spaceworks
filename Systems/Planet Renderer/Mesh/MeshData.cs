using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spaceworks {

    public class MeshData {
        public string name;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public int[] triangles;

        public Mesh mesh {
            get {
                Mesh m = new Mesh();
                m.name = this.name;

                m.vertices = this.vertices;
                m.uv = this.uvs;
                m.triangles = this.triangles;
                m.normals = this.normals;

                return m;
            }
        }
    }

}
