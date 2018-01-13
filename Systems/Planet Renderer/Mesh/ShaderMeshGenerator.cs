using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class ShaderMeshGenerator : IMeshService {

		[System.Serializable]
		public class ShaderThreads {
			public int xy = 8;
			public int x{
				get { return xy; }
			}
			public int y{
				get{ return xy; }
			}
		}

		[Header("General")]
		public string kernelHandle = "CSMain";
		public ComputeShader shader;
		public ShaderThreads numthreads;
		public int resolutionMultipler;

		[Header("Input Parameters")]
		public string arrayWidthParameter = "indexingWidth";
		public string radiusParameter = "baseRadius";
		public string topLeftParameter = "topLeft";
		public string topRightParameter = "topRight";
		public string bottomLeftParameter = "bottomLeft";
		public string bottomRightParameter = "bottomRight";

		[Header("Output Parameters")]
		public string vertexArray = "vertices";
		public string normalArray = "normals";
		public string uvArray = "uvs";
		public string triangleArray = "triangles";

		private int handle;

		void Start(){
			handle = shader.FindKernel (kernelHandle);
		}

		public float GetAltitude (Vector3 pos, float baseRadius){
			return baseRadius;
		}

		public Vector3 GetNormal (Vector3 pos, float baseRadius){
			return pos;
		}

		public override Mesh Make (Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, float radius){
			//Replace resolution with one matching out shader's threads
			//Resolution is number of vertices across any axis
			int resolution = (resolutionMultipler > 1 ? resolutionMultipler : 1) * numthreads.xy; //Ensure multiple of xy

			//Initialize values
			int width = resolution;
			int size = width * width;

			int num_vertices = size;
			int num_normals = size;
			int num_uvs = size;
			int num_triangles = ((width - 1) * (width - 1)) * 6;

			//Create Buffers
			Vector3[] v = new Vector3[num_vertices];
			Vector3[] n = new Vector3[num_normals];
			Vector2[] u = new Vector2[num_uvs];
			int[] t = new int[num_triangles];

			ComputeBuffer vb = new ComputeBuffer (v.Length, 3 * sizeof(float)); //3 floats * 4 bytes / float
			ComputeBuffer nb = new ComputeBuffer (n.Length, 3 * sizeof(float));
			ComputeBuffer ub = new ComputeBuffer (n.Length, 2 * sizeof(float));
			ComputeBuffer tb = new ComputeBuffer (n.Length, sizeof(int));

			//Transfer data to GPU
			shader.SetInt(arrayWidthParameter, width);
			shader.SetFloat (radiusParameter, radius);
			shader.SetVector (topLeftParameter, topLeft);
			shader.SetVector (topRightParameter, topRight);
			shader.SetVector (bottomLeftParameter, bottomLeft);
			shader.SetVector (bottomRightParameter, bottomRight);

			shader.SetBuffer (handle, vertexArray, vb); 
			shader.SetBuffer (handle, normalArray, nb);
			shader.SetBuffer (handle, uvArray, ub);
			shader.SetBuffer (handle, triangleArray, tb);

			//Dispatch the shader
			shader.Dispatch (handle, width / numthreads.x, width / numthreads.y, 1);

			//Retrieve data from GPU
			vb.GetData (v);
			nb.GetData (n);
			ub.GetData (u);
			tb.GetData (t);

			//Dispose buffers to be cleaned up by GC
			vb.Dispose ();
			nb.Dispose ();
			ub.Dispose ();
			tb.Dispose ();

            //Create mesh
            Mesh m = new Mesh ();
			m.name = "Surface_"+resolution;
			m.vertices = v;
			m.normals = n;
			m.uv = u;
			m.triangles = t;

			return m;
		}

	}

}