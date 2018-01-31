using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class HeightField {

        private float[] heightmap;

        public int width {
            get; private set;
        }

        public int height {
            get; private set;
        }

        public float this[int x, int y] {
            get {
                return heightmap[x + width * y];
            }
            set {
                heightmap[x + width * y] = value;
            }
        }

        public HeightField(int width, int height) {
            this.width = Mathf.Max(1, width);
            this.height = Mathf.Max(1, height);

            heightmap = new float[this.width * this.height];
        }

    }

	public class CubemapMeshGenerator : IMeshService {

		public enum ColourComponent{
			Red, Green, Blue, Alpha, Luminosity
		}

		[Header("Mesh Settings")]
        public int resolution = 24;
        public bool useSkirts = false;
		[Range(0,1)]
		public float skirtSize = 0.9f;
		public HighLowPair range;

		[Header("Heightmap Settings")]
		public Texture2D heightmapTop;
		public Texture2D heightmapBottom;
		public Texture2D heightmapLeft;
		public Texture2D heightmapRight;
		public Texture2D heightmapFront;
		public Texture2D heightmapBack;
		public ColourComponent heightColour;

        //Internal calculated heightfields
        private HeightField h_top;
        private HeightField h_bottom;
        private HeightField h_left;
        private HeightField h_right;
        private HeightField h_front;
        private HeightField h_back;

        private static Perlin perlin = new Perlin();

        /// <summary>
        /// Create HeightFields from textures
        /// </summary>
        private void GenerateHeightFields(bool force = false) {
            if (!force && h_top != null)
                return;

            Texture2D[] maps = new Texture2D[] {
                heightmapTop,
                heightmapBottom,
                heightmapLeft,
                heightmapRight,
                heightmapFront,
                heightmapBack
            };
            
            for (int i = 0; i < 6; i++) {
                Texture2D map = maps[i];
                HeightField field = new HeightField(map.width, map.height);

                for (int w = 0; w < map.width; w++) {
                    for (int h = 0; h < map.height; h++) {
                        field[w, h] = SampleColour(map.GetPixel(w,h));
                    }
                }

                switch (i) {
                    case 0:
                        h_top = field;
                        break;
                    case 1:
                        h_bottom = field;
                        break;
                    case 2:
                        h_left = field;
                        break;
                    case 3:
                        h_right = field;
                        break;
                    case 4:
                        h_front = field;
                        break;
                    case 5:
                        h_back = field;
                        break;
                }
                
            }

        }

        /// <summary>
        /// Sampel a height from a colour
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        private float SampleColour(Color colour){
			switch (heightColour) {
				case ColourComponent.Alpha:
					return colour.a;
				case ColourComponent.Blue:
					return colour.b;
				case ColourComponent.Green:
					return colour.g;
                case ColourComponent.Luminosity:
                    return 0.21f * colour.r + 0.72f * colour.g + 0.07f * colour.b;
				default:
					return colour.r;
			}
		}

        /// <summary>
        /// Sample one of the 6 heightfields at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
		private float SampleHeightmap(Vector3 pos){
            GenerateHeightFields();

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
					float x = ((pos.z + 1) * 0.5f) * (h_right.width - 1);
					float y = ((pos.y + 1) * 0.5f) * (h_right.height - 1);
                    value = h_right[(int)x, (int)y];
					break;
				//Left
				case CubemapFace.NegativeX:
					x = (1 - (pos.z + 1) * 0.5f) * (h_left.width - 1);
					y = ((pos.y + 1) * 0.5f) * (h_left.height - 1);
                    value = h_left[(int)x, (int)y];
                    break;

				//Up
				case CubemapFace.PositiveY:
					x = ((pos.x + 1) * 0.5f) * (h_top.width - 1);
					y = (1 - (pos.z + 1) * 0.5f) * (h_top.height - 1);
                    value = h_top[(int)x, (int)y];
                    break;
				//Down
				case CubemapFace.NegativeY:
					x = ((pos.x + 1) * 0.5f) * (h_bottom.width - 1);
					y = ((pos.z + 1) * 0.5f) * (h_bottom.height - 1);
                    value = h_bottom[(int)x, (int)y];
                    break;

				//Forward
				case CubemapFace.PositiveZ:
					x = (1 - (pos.x + 1) * 0.5f) * (h_front.width - 1);
					y = ((pos.y + 1) * 0.5f) * (h_front.height - 1);
                    value = h_front[(int)x, (int)y];
                    break;
				//Back
				case CubemapFace.NegativeZ:
					x = ((pos.x + 1) * 0.5f) * (h_back.width - 1);
					y = ((pos.y + 1) * 0.5f) * (h_back.height - 1);
                    value = h_back[(int)x, (int)y];
                    break;
			}

			return value;
		}

        /// <summary>
        /// Get the height of the planet at a certain point
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="baseRadius"></param>
        /// <returns></returns>
		public float GetAltitude (Vector3 pos, float baseRadius){
            //Sample cubemap and add a small random value
            float sampled = Mathf.Lerp(range.low, range.high, SampleHeightmap(pos)) + baseRadius;
            float rng = 0.25f * perlin.Value(pos, 18f);
            return sampled + rng;
		}

        /// <summary>
        /// Get the normal of the planet at a certain point
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="baseRadius"></param>
        /// <returns></returns>
		public Vector3 GetNormal(Vector3 pos, float baseRadius){
            float e = 0.001f; //Some small value
            Vector3 p = IMeshService.Spherify(pos);
            Vector3 p1 = new Vector3(pos.x + e, pos.y, pos.z);
            Vector3 p2 = new Vector3(pos.x, pos.y + e, pos.z);
            Vector3 p3 = new Vector3(pos.x, pos.y, pos.z + e);

            Vector3 a = p * SampleHeightmap(pos);
            Vector3 b = IMeshService.Spherify(p1) * SampleHeightmap(p1);
            Vector3 c = IMeshService.Spherify(p2) * SampleHeightmap(p2);
            Vector3 d = IMeshService.Spherify(p3) * SampleHeightmap(p3);

            Vector3 delta = (b - a) + (c - a) + (d - a);
            Vector3 normal = delta - p * Vector3.Dot(delta, p) + p;

            return normal.normalized;
        }

        /// <summary>
        /// Initialize service
        /// </summary>
        public override void Init() {
            GenerateHeightFields();
        }

        /// <summary>
        /// Make mesh
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public override MeshData Make (Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, float radius)
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

					//Create Vertice on Unit Cube
					Vector3 rawPosition = Vector3.Lerp(
						Vector3.Lerp(topLeft, topRight, i * step),
						Vector3.Lerp(bottomLeft, bottomRight, i * step),
						j * step
					);

                    //Transform Vertice to Unit Sphere
					Vector3 pos = IMeshService.Spherify(rawPosition);

					//Sample Noise to Get Altitude
					float alt = GetAltitude(rawPosition, radius);

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
					n[idx] = GetNormal(rawPosition, radius);

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