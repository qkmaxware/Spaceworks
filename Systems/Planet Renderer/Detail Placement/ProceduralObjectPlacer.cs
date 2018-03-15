using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Pooling;

namespace Spaceworks {

    public class ProceduralObjectPlacer : IDetailer {

		[System.Serializable]
		public class PlacementRule {
			public string name;
            public int poolBufferSize = 3;

            public ProceduralPlacementRule rule;

			[HideInInspector] 
			public GameObjectPool pool;
		}

		//List of placement rules in inspector
		public PlacementRule[] objectsToPool;

		//Internal pooling
		private Dictionary<string, PlacementRule> pools = new Dictionary<string, PlacementRule> ();
		private Dictionary<QuadNode<ChunkData>, List<GameObject>> active = new Dictionary<QuadNode<ChunkData>, List<GameObject>>();
		private Dictionary<QuadNode<ChunkData>, Coroutine> spawning = new Dictionary<QuadNode<ChunkData>, Coroutine>();

		void Start(){

            //Create pools
			foreach (PlacementRule rule in this.objectsToPool) {
                GameObjectPool pool = PoolManager.DefaultInstancePool(rule.rule.prefab, rule.poolBufferSize, rule.poolBufferSize);
				rule.pool = pool;
                if(!pools.ContainsKey(rule.rule.prefab.name))
				    pools.Add (rule.rule.prefab.name, rule);
			}
		}

		/// <summary>
		/// Spawn gameobejects for all rules on a given chunk.
		/// </summary>
		/// <param name="node">Node.</param>
		private IEnumerator SpawnForChunk(QuadNode<ChunkData> node, Mesh m){
            
            //Get list of spawned objects for this node
			List<GameObject> spawned;
			if (active.ContainsKey (node))
				spawned = active [node];
			else {
				spawned = new List<GameObject> ();
				active [node] = spawned;
			}

            int j = 0;
            foreach (PlacementRule rule in objectsToPool) {
                yield return rule.rule.SpawnObjectOnChunk(this.transform, j++, rule.pool, spawned, node, m);
            }
           
		}			

		/// <summary>
		/// Shows the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void ShowChunkDetails (QuadNode<ChunkData> node,  Mesh m){
			//Only spawn if we don't already have an active spawning session for it
			if(!this.spawning.ContainsKey(node)){
				Coroutine co = StartCoroutine (SpawnForChunk (node, m));
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
                    pool.pool.Push(obj);
				}
			}
		}

	}
}