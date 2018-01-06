using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks{

	public class PerlinF : INoise {
		
		private static int[] hash = {
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233,  7, 225,
			140, 36, 103, 30, 69, 142,  8, 99, 37, 240, 21, 10, 23, 190,  6, 148,
			247, 120, 234, 75,  0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
			57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
			74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
			65, 25, 63, 161,  1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
			200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186,  3, 64,
			52, 217, 226, 250, 124, 123,  5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
			207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			119, 248, 152,  2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172,  9,
			129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
			218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
			81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
			184, 84, 204, 176, 115, 121, 50, 45, 127,  4, 150, 254, 138, 236, 205, 93,
			222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

			151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233,  7, 225,
			140, 36, 103, 30, 69, 142,  8, 99, 37, 240, 21, 10, 23, 190,  6, 148,
			247, 120, 234, 75,  0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
			57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
			74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
			65, 25, 63, 161,  1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
			200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186,  3, 64,
			52, 217, 226, 250, 124, 123,  5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
			207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			119, 248, 152,  2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172,  9,
			129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
			218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
			81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
			184, 84, 204, 176, 115, 121, 50, 45, 127,  4, 150, 254, 138, 236, 205, 93,
			222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
		};
	
		private const int hashMask = 255;

		private const int gradientsMask3D = 15;

		private static int Fastfloor(float x) {
			return x>0 ? (int)x : (int)x-1;
		}

		/// <summary>
		/// Evaluate the quintic funtion at t
		/// </summary>
		/// <param name="t">T.</param>
		private static float Quintic(float t) {
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		/// <summary>
		/// Evaluate the derivative of the quintic function at t
		/// </summary>
		/// <returns>The derivative.</returns>
		/// <param name="t">T.</param>
		private static float QuinticDerivative(float t){
			return 30 * t * t * (t * (t - 2) + 1);
		}

		/// <summary>
		/// Determines the hash of x y z.
		/// </summary>
		/// <returns><c>true</c> if hash x y z; otherwise, <c>false</c>.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private static int Hash(int x, int y, int z){
			return hash [hash [hash [x] + y] + z];
		}

		/// <summary>
		/// Evaluate the gradient
		/// </summary>
		/// <param name="hash">Hash.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private static float Grad(int hash, float x, float y, float z){
			int h = hash & gradientsMask3D;         // CONVERT LO 4 BITS OF HASH CODE
			float u = h < 8 ? x : y, 				// INTO 12 GRADIENT DIRECTIONS.
			v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		public NoiseSample Value (Vector3 point, float frequency) {
			point *= frequency;

			int xi0 = Fastfloor (point.x);
			int yi0 = Fastfloor (point.y);
			int zi0 = Fastfloor(point.z);

			int xi1 = (xi0 + 1) & hashMask;
			int yi1 = (yi0 + 1) & hashMask;
			int zi1 = (zi0 + 1) & hashMask;

			float x0 = point.x - xi0;
			float y0 = point.y - yi0;
			float z0 = point.z - zi0;

			float x1 = x0 - 1f;
			float y1 = y0 - 1f;
			float z1 = z0 - 1f;

			xi0 &= hashMask;
			yi0 &= hashMask;
			zi0 &= hashMask;

			float a = Grad(Hash(xi0, yi0, zi0), x0, y0, z0); 
	        float b = Grad(Hash(xi1, yi0, zi0), x1, y0, z0); 
	        float c = Grad(Hash(xi0, yi1, zi0), x0, y1, z0); 
	        float d = Grad(Hash(xi1, yi1, zi0), x1, y1, z0); 
	        float e = Grad(Hash(xi0, yi0, zi1), x0, y0, z1); 
	        float f = Grad(Hash(xi1, yi0, zi1), x1, y0, z1); 
	        float g = Grad(Hash(xi0, yi1, zi1), x0, y1, z1); 
	        float h = Grad(Hash(xi1, yi1, zi1), x1, y1, z1); 

			float du = QuinticDerivative (x0);
			float dv = QuinticDerivative (y0);
			float dw = QuinticDerivative (z0);
			float u = Quintic (x0);
			float v = Quintic (y0);
			float w = Quintic (z0);

			float k0 = a;
			float k1 = (b - a);
			float k2 = (c - a);
			float k3 = (e - a);
			float k4 = (a + d - b - c);
			float k5 = (a + f - b - e);
			float k6 = (a + g - c - e);
			float k7 = (b + c + e + h - a - d - f - g);

			NoiseSample sample = new NoiseSample();
			sample.value = k0 + k1 * u + k2 * v + k3 * w + k4 * u * v + k5 * u * w + k6 * v * w + k7 * u * v * w;
			sample.derivative = new Vector3 (
				du *(k1 + k4 * v + k5 * w + k7 * v * w),
				dv *(k2 + k4 * u + k6 * w + k7 * v * w),
				dw *(k3 + k5 * u + k6 * v + k7 * v * w)
			);

			return sample;
		}

		public NoiseSample Sum(Vector3 pos, NoiseOptions opts){
			//Setup
			NoiseSample sum = Value(pos, opts.frequency);
			float range = 1.0f;
			float amplitude = 1.0f;

			float frequency = opts.frequency;
			float persistence = opts.persistence;
			float lacunarity = opts.lacunarity;

			for (int i = 0; i < opts.octaves; i++) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Value (pos, frequency) * amplitude;
			}

			//Return the processed value
			return sum * (1.0f / range);
		}

	}

}