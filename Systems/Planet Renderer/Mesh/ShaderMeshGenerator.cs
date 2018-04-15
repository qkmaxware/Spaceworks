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
        public bool useSkirts = false;
        public string skirtKernelHandle = "CSSecondary";
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
        public string skirtParameter = "skirtLength";
        public float skirtLength = 0.99f;

		[Header("Output Parameters")]
		public string vertexArray = "vertices";
		public string normalArray = "normals";
		public string uvArray = "uvs";
		public string triangleArray = "triangles";

		private int handle;
        private int skirtHandle;

		void Start(){
			handle = shader.FindKernel (kernelHandle);
            if(useSkirts)
                skirtHandle = shader.FindKernel(skirtKernelHandle);
		}

		public float GetAltitude (Vector3 pos, float baseRadius){
			return baseRadius;
		}

		public Vector3 GetNormal (Vector3 pos, float baseRadius){
			return pos;
		}

        public override void Init(){
            
        }

        public override MeshData Make (Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, Zone2 uvRange, float radius){
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
			ComputeBuffer ub = new ComputeBuffer (u.Length, 2 * sizeof(float));
			ComputeBuffer tb = new ComputeBuffer (t.Length, sizeof(int));

			//Transfer data to GPU
			shader.SetInt(arrayWidthParameter, width);
			shader.SetFloat (radiusParameter, radius);
			shader.SetVector (topLeftParameter, topLeft);
			shader.SetVector (topRightParameter, topRight);
			shader.SetVector (bottomLeftParameter, bottomLeft);
			shader.SetVector (bottomRightParameter, bottomRight);

            if (useSkirts) {

            }

			shader.SetBuffer (handle, vertexArray, vb); 
			shader.SetBuffer (handle, normalArray, nb);
			shader.SetBuffer (handle, uvArray, ub);
			shader.SetBuffer (handle, triangleArray, tb);

			//Dispatch the shader
			shader.Dispatch (handle, width / numthreads.x, width / numthreads.y, 1);

            if (useSkirts) {
                //shader.Dispatch(skirtHandle, width / numthreads.x, 1, 1);
            }

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