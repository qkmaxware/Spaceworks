using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public abstract class ITextureService : MonoBehaviour {
		public abstract void Init (PlanetConfig config, Material mat);

		public abstract void SetMaterialPlanetCenter (Material mat, Vector3 center);
	}

}