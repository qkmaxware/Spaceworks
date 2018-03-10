using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class CubemapTextureService : ITextureService {

        public Material top;
        public Material bottom;
        public Material left;
        public Material right;
        public Material front;
        public Material back;

        public override Material GetMaterialFor(PlanetFace face, QuadNode<ChunkData> node, Mesh mesh) {
            switch (face.direction) {
                case PlanetFaceDirection.Top:
                    return top;
                case PlanetFaceDirection.Bottom:
                    return bottom;
                case PlanetFaceDirection.Left:
                    return left;
                case PlanetFaceDirection.Right:
                    return right;
                case PlanetFaceDirection.Front:
                    return front;
                default:
                    return back;
            }
        }

        public override void Init(PlanetConfig config) {}

        public override void SetPlanetCenter(Vector3 center) {}
    }

}