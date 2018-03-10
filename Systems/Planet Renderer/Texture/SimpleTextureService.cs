using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class SimpleTextureService : ITextureService {

        public Material material;

        public override Material GetMaterialFor(PlanetFace face, QuadNode<ChunkData> node, Mesh mesh) {
            return material;
        }

        public override void Init(PlanetConfig config) {
            material.SetFloat("_SeaLevel", config.radius);
        }

        public override void SetPlanetCenter(Vector3 center){
            material.SetVector ("_Center", center);	
		}
    }

}