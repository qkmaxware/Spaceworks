using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public class ProceduralObjectPlacer : IDetailer {

		[System.Serializable]
		public class PlacementRule {
			[Header("Prefab Detals")]
			public string name;
			public GameObject prefab;
			public int poolBufferSize = 3;

			[Header("Placement Rules")]
			public int seed = 0;
			[Range(0, 90)]
			public float slopeLimit = 60.0f;
			public HighLowPair altitudeRange = new HighLowPair (float.MaxValue, 0);
			public HighLowPair amountRange;
			public HighLowPair scaleRange;

			[HideInInspector] 
			public Queue<GameObject> pool;
		}

		//List of placement rules in inspector
		public int amountToSpawnAtOnce = 5;
		public PlacementRule[] objectsToPool;

		//Internal pooling
		private Dictionary<string, PlacementRule> pools = new Dictionary<string, PlacementRule> ();
		private Dictionary<QuadNode<ChunkData>, List<GameObject>> active = new Dictionary<QuadNode<ChunkData>, List<GameObject>>();
		private Dictionary<QuadNode<ChunkData>, Coroutine> spawning = new Dictionary<QuadNode<ChunkData>, Coroutine>();
		private GameObject poolStore;

		void Start(){
			//Create object to hold pooled resources
			poolStore = new GameObject("Detail Pool");
			poolStore.transform.SetParent (this.transform);
			poolStore.transform.localPosition = Vector3.zero;
			poolStore.transform.localScale = Vector3.one;

			//Create pools
			foreach (PlacementRule rule in this.objectsToPool) {
				Queue<GameObject> pool = new Queue<GameObject> ();
				rule.pool = pool;
				pools.Add (rule.prefab.name, rule);
			}
		}

		/// <summary>
		/// Spawn gameobejects for all rules on a given chunk.
		/// </summary>
		/// <param name="node">Node.</param>
		private IEnumerator SpawnForChunk(QuadNode<ChunkData> node, Mesh m, int amountToSpawnAtOnce){
			//Get list of spawned objects for this node
			List<GameObject> spawned;
			if (active.ContainsKey (node))
				spawned = active [node];
			else {
				spawned = new List<GameObject> ();
				active [node] = spawned;
			}

			Vector3[] verts = m.vertices;
			int[] tris = m.triangles;

			//Go through all objects to place
			int j = 0; int k = 0;
			foreach (PlacementRule rule in objectsToPool) {
				//Bitshift ensures that successive rules with the same seeds will end up with different placements
				Random.InitState ((rule.seed << j++) * node.value.bounds.center.GetHashCode());

				//Number of this prefab to spawn
				int number = (int)Random.Range (rule.amountRange.low, rule.amountRange.high);
				float sl = rule.slopeLimit;

				for (int i = 0; i < number; i++) {
					int faceIdx = Random.Range (0, (tris.Length / 3) - 1) * 3;

					//Random x and y coordinates
					float rx = Random.value;
					float ry = Random.value;

					float sqrt_rx = Mathf.Sqrt (rx);

					Vector3 a = verts [tris[faceIdx]];
					Vector3 b = verts [tris[faceIdx + 1]];
					Vector3 c = verts [tris[faceIdx + 2]];

					Vector3 pos = (1 - sqrt_rx) * a + (sqrt_rx * (1 - ry)) * b + (sqrt_rx * ry) * c;

					//Ignore if slope is too much
					float angle = Vector3.Angle(pos, Vector3.Cross(b - a, c - a));
					if (angle > rule.slopeLimit)
						continue;

					//Ignore if not in altitude range
					float distance = pos.magnitude;
					if (distance < rule.altitudeRange.low || distance > rule.altitudeRange.high)
						continue;

					//Determine scale
					Vector3 scale = Vector3.one * Random.Range(rule.scaleRange.low, rule.scaleRange.high);

					//Pool and spawn
					if(rule.pool.Count < 1){
						ExpandPool (rule, rule.poolBufferSize);
					}
					GameObject go = rule.pool.Dequeue ();
					go.SetActive(true);
					go.transform.localScale = scale;
					go.transform.localPosition = pos;
					go.transform.localRotation = Quaternion.FromToRotation (Vector3.up, pos);// * Quaternion.Euler(0, 360 * Random.value, 0);

					//Add spawned object to reference list
					spawned.Add(go);

					//If passed frame spawn limit, wait till next frame and reset the count
					k++;
					if (k >= amountToSpawnAtOnce) {
						k = 0;
						yield return null;
					}
				}

			}
		}

		/// <summary>
		/// Expands the pool.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="amount">Amount.</param>
		public void ExpandPool(PlacementRule pool, int amount){
			for (int i = 0; i < amount; i++) {
				GameObject p = Instantiate (pool.prefab);
				p.name = pool.prefab.name; //Preserve name (for pooling findign purposes)
				p.transform.SetParent (poolStore.transform);
				p.transform.localScale = Vector3.one;
				p.SetActive (false);
				pool.pool.Enqueue (p);
			}
		}				

		/// <summary>
		/// Shows the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void ShowChunkDetails (QuadNode<ChunkData> node,  Mesh m){
			//Only spawn if we don't already have an active spawning session for it
			if(!this.spawning.ContainsKey(node)){
				Coroutine co = StartCoroutine (SpawnForChunk (node, m, Mathf.Max(1, this.amountToSpawnAtOnce)));
				this.spawning [node] = co;
			}
		}

		/// <summary>
		/// Hides the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void HideChunkDetails (QuadNode<ChunkData> node){
			//Stop active spawning session if it exists
			if (this.spawning.ContainsKey (node)) {
				Coroutine c = (this.spawning [node]);
				if(c != null)
					StopCoroutine (this.spawning [node]);
				this.spawning.Remove (node);
			}

			//This chunk has no objects associated to it
			List<GameObject> objects;
			if (!active.TryGetValue (node, out objects)) {
				return;
			}

			//Delete the object array from the active collection
			active.Remove (node);

			//This chunk needs to have its gameobjects either deleted or pooled
			foreach (GameObject obj in objects) {
				PlacementRule pool;
				if (!pools.TryGetValue (obj.name, out pool)) {
					//No pool
					GameObject.Destroy (obj);
				} else {
					//Pool exists
					obj.SetActive(false);
					obj.transform.localScale = Vector3.one;
					pool.pool.Enqueue (obj);
				}
			}
		}

	}
}