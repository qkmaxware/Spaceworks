using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public abstract class ITextureService : MonoBehaviour {

		public abstract void Init (PlanetConfig config);

		public abstract void SetPlanetCenter (Vector3 center);

        public abstract Material GetMaterialFor(PlanetFace face, QuadNode<ChunkData> node, Mesh mesh);
	}

}