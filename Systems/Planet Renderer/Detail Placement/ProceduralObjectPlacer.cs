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
			public HighLowPair altitudeRange = new HighLowPair (float.MaxValue, 0);
			public HighLowPair amountRange;
			public HighLowPair scaleRange;

			[HideInInspector] 
			public Queue<GameObject> pool;
		}

		//List of placement rules in inspector
		public PlacementRule[] objectsToPool;

		//Internal pooling
		private Dictionary<string, PlacementRule> pools = new Dictionary<string, PlacementRule> ();
		private Dictionary<QuadNode<ChunkData>, List<GameObject>> active = new Dictionary<QuadNode<ChunkData>, List<GameObject>>();
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
		private void SpawnForChunk(QuadNode<ChunkData> node, float radius, IMeshService meshService){
			//Get list of spawned objects for this node
			List<GameObject> spawned;
			if (active.ContainsKey (node))
				spawned = active [node];
			else {
				spawned = new List<GameObject> ();
				active [node] = spawned;
			}

			//Go through all objects to place
			int j = 0;
			foreach (PlacementRule rule in objectsToPool) {
				//Bitshift ensures that successive rules with the same seeds will end up with different placements
				Random.InitState ((rule.seed << j++) * node.value.bounds.center.GetHashCode());

				//Number of this prefab to spawn
				int number = (int)Random.Range (rule.amountRange.low, rule.amountRange.high);

				for (int i = 0; i < number; i++) {
					//Random x and y coordinates
					float rx = Random.value;
					float ry = Random.value;

					//Position on cube face
					Vector3 cubePos = Vector3.Lerp (
						Vector3.Lerp(node.range.a, node.range.b, rx),
						Vector3.Lerp(node.range.d, node.range.c, rx),
						ry
					);

					//Convert to sphere
					Vector3 spherePos = IMeshService.Spherify(cubePos);

					//Determine altitude, skip if not in range
					Vector3 norm;
					float altitude = meshService.GetAltitude(spherePos, radius, out norm);
					if (altitude < rule.altitudeRange.low || altitude > rule.altitudeRange.high)
						continue;
					Vector3 pos = spherePos * altitude; 
					
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
					go.transform.localRotation = Quaternion.FromToRotation (Vector3.up, spherePos);// * Quaternion.Euler(0, 360 * Random.value, 0);

					//Add spawned object to reference list
					spawned.Add(go);
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
		public override void ShowChunkDetails (QuadNode<ChunkData> node,  float radius, IMeshService meshService){
			//TODO request based system to not hog resources or slow down game
			SpawnForChunk (node, radius, meshService);
		}

		/// <summary>
		/// Hides the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void HideChunkDetails (QuadNode<ChunkData> node){
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