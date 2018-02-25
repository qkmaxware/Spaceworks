using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class Pool {
        private int bufferSize = 3;
        private Transform parent;
        private GameObject prefab;

        private Queue<GameObject> pool = new Queue<GameObject>();

        public Pool(GameObject poolable, int initialSize, int bufferSize = 3, Transform parent = null) {
            this.prefab = poolable;
            this.bufferSize = bufferSize;
            this.parent = parent;
            ExpandPool(initialSize);
        }

        public bool Empty() {
            return pool.Count < 1;
        }

        public GameObject Pop() {
            if (Empty()) {
                ExpandPool(bufferSize);
            }
            GameObject go = pool.Dequeue();
            return go;
        }

        public void Push(GameObject go) {
            pool.Enqueue(go);
            go.SetActive(false);
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Expands the pool.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="amount">Amount.</param>
        public void ExpandPool(int amount) {
            for (int i = 0; i < amount; i++) {
                GameObject p = GameObject.Instantiate(prefab);
                p.name = prefab.name; //Preserve name (for pooling finding purposes)
                if (parent != null)
                    p.transform.SetParent(parent);
                p.transform.localScale = Vector3.one;
                p.SetActive(false);
                pool.Enqueue(p);
            }
        }
    }

    [System.Serializable]
    public class ProceduralPlacementRule{

        public enum RuleType {
            SurfaceRandom, SpecificCoordinate
        }

        [Header("Global")]
        public RuleType placementType = RuleType.SurfaceRandom;
        public GameObject prefab;

        //Stuff for surface random
        [Header("Surface Random Settings")]
        public int seed = 0;
        [Range(0, 90)]
        public float slopeLimit = 60.0f;
        public HighLowPair altitudeRange = new HighLowPair(float.MaxValue, 0);
        public HighLowPair amountRange;
        [Range(-1,1)]
        public float probabilityScale = 0;
        public HighLowPair scaleRange;

        //Stuff for specific coordinate
        [Header("Specific Coordinate Settings (spherical)")]
        public float fixedScale;
        public float theta;
        public float phi;

        public IEnumerator SpawnObjectOnChunk(int stackDepth, Pool srcPool, List<GameObject> destinationPool, QuadNode<ChunkData> node, Mesh meshData) {

            switch (placementType) {
                case RuleType.SurfaceRandom:
                    yield return SpawnSurfaceRandom(stackDepth, srcPool, destinationPool, node, meshData);
                    break;
                case RuleType.SpecificCoordinate:
                    yield return SpawnSpecific(stackDepth, srcPool, destinationPool, node, meshData);
                    break;
            }

        }

        public Vector3 CartesianToSpherical(Vector3 cartesian) {
            float xx = cartesian.x * cartesian.x;
            float zz = cartesian.z * cartesian.z;

            float r = Mathf.Sqrt(xx + cartesian.y * cartesian.y + zz);
            float theta = Mathf.Atan2(cartesian.z, cartesian.x);
            float phi = Mathf.Atan2(Mathf.Sqrt(xx + zz), cartesian.y);

            return new Vector3(r, theta, phi);
        }

        public Vector3 SphericalToCartesian(Vector3 sphere) {
            float sinPhi = Mathf.Sin(sphere.z);

            float x = sphere.x * sinPhi * Mathf.Cos(sphere.y);
            float y = sphere.x * Mathf.Cos(sphere.z);
            float z = sphere.x * sinPhi * Mathf.Sin(sphere.y);

            return new Vector3(x, y, z);
        }

        private float HeronsFormula(float sideA, float sideB, float sideC) {
            float s = 0.5f * (sideA + sideB + sideC);
            float a = Mathf.Sqrt(s * (s - sideA) * (s - sideB) * (s - sideC));
            return a;
        }

        /// <summary>
        /// A B
        /// D C
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool PointInRectangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {

            float areaAPD = HeronsFormula((a - point).magnitude, (point - d).magnitude, (d - a).magnitude);
            float areaABP = HeronsFormula((a - b).magnitude, (b - point).magnitude, (point - a).magnitude);
            float areaBCP = HeronsFormula((b - c).magnitude, (c - point).magnitude, (point - b).magnitude);
            float areaCPD = HeronsFormula((c - point).magnitude, (point - d).magnitude, (d - c).magnitude);

            float areaACD = HeronsFormula((a - c).magnitude, (c - d).magnitude, (d - a).magnitude);
            float areaABC = HeronsFormula((a - b).magnitude, (b - c).magnitude, (c - a).magnitude);

            float quadArea = areaACD + areaABC;
            float pointArea = areaAPD + areaABP + areaBCP + areaCPD;

            float cmp = Mathf.Abs(quadArea - pointArea);
            if (cmp < 0.0000001f) {
                return true;
            }
            return false;
        }

        private IEnumerator SpawnSpecific(int stackDepth, Pool srcPool, List<GameObject> destinationPool, QuadNode<ChunkData> node, Mesh meshData) {

            Vector3 worldCoordinates = SphericalToCartesian(new Vector3(2, theta, phi));

            Plane p = new Plane(node.range.normal, node.range.center);

            Vector3 onPlane = p.ClosestPointOnPlane(worldCoordinates);

            if (PointInRectangle(onPlane, node.range.a, node.range.b, node.range.c, node.range.d)) {
                //Pool and spawn
                GameObject go = srcPool.Pop();

                //Position object from pool
                Vector3 pos = SphericalToCartesian(new Vector3(meshData.bounds.center.magnitude, theta, phi));
                go.SetActive(true);
                go.transform.localScale = Vector3.one * fixedScale;
                go.transform.localPosition = pos;
                go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, pos);// * Quaternion.Euler(0, 360 * Random.value, 0);
            }
            
            return null;
        }

        private IEnumerator SpawnSurfaceRandom(int stackDepth, Pool srcPool, List<GameObject> destinationPool, QuadNode<ChunkData> node, Mesh meshData) {
            Vector3[] verts = meshData.vertices;
            int[] tris = meshData.triangles;

            //Bitshift ensures that successive rules with the same seeds will end up with different placements
            Random.InitState((seed << stackDepth) * node.value.bounds.center.GetHashCode());

            //Number of this prefab to spawn
            float slider = Random.value;
            if (probabilityScale < 0) {

            }
            else if (probabilityScale > 0){
                
            }
            int number = (int)Mathf.Lerp(amountRange.low, amountRange.high, slider);

            //Place
            for (int i = 0; i < number; i++) {
                int faceIdx = Random.Range(0, (tris.Length / 3) - 1) * 3;

                //Random x and y coordinates
                float rx = Random.value;
                float ry = Random.value;

                //Random position on face
                float sqrt_rx = Mathf.Sqrt(rx);
                Vector3 a = verts[tris[faceIdx]];
                Vector3 b = verts[tris[faceIdx + 1]];
                Vector3 c = verts[tris[faceIdx + 2]];

                Vector3 pos = (1 - sqrt_rx) * a + (sqrt_rx * (1 - ry)) * b + (sqrt_rx * ry) * c;

                //Ignore if slope is too much
                float angle = Vector3.Angle(pos, Vector3.Cross(b - a, c - a));
                if (angle > slopeLimit)
                    continue;

                //Ignore if not in altitude range
                float distance = pos.magnitude;
                if (distance < altitudeRange.low || distance > altitudeRange.high)
                    continue;

                //Determine scale
                Vector3 scale = Vector3.one * Random.Range(scaleRange.low, scaleRange.high);

                //Pool and spawn
                GameObject go = srcPool.Pop();

                //Position object from pool
                go.SetActive(true);
                go.transform.localScale = scale;
                go.transform.localPosition = pos;
                go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, pos);// * Quaternion.Euler(0, 360 * Random.value, 0);

                //Add spawned object to reference list
                destinationPool.Add(go);

                //Wait till next frame for next spawning
                yield return null;
            }
        }

    }

}
