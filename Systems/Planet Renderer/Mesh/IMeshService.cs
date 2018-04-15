using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public abstract class IMeshService : MonoBehaviour {

		public static Vector3 Spherify(Vector3 position) {
			float x = position.x;
			float y = position.y;
			float z = position.z;

			float xx = x * x;
			float yy = y * y;
			float zz = z * z;

			float X = x * Mathf.Sqrt(1 - (yy / 2) - (zz / 2) + ((yy * zz) / 3));
			float Y = y * Mathf.Sqrt(1 - (zz / 2) - (xx / 2) + ((zz * xx) / 3));
			float Z = z * Mathf.Sqrt(1 - (xx / 2) - (yy / 2) + ((xx * yy) / 3));

			return new Vector3(X, Y, Z);
		}

        public abstract void Init();

		public abstract MeshData Make (
			Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, 
			Zone2 uvRange,
			float radius
		);
	}

}