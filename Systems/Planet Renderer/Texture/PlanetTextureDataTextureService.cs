using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class PlanetTextureDataTextureService : ITextureService {

		public override void Init(PlanetConfig config, Material mat){
			mat.SetFloat ("_SeaLevel", config.radius);
		}

		public override void SetMaterialPlanetCenter(Material mat, Vector3 center){
			mat.SetVector ("_Center", center);	
		}

	}

}