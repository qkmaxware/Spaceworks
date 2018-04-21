using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Pooling;
using Spaceworks;

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

		private class SpawnRequest {
			public QuadNode<ChunkData> node;
			public Mesh mesh;
			public int ruleidx = 0;
			public bool ruleStarted = false;
			public int amountSpawned = 0;
			public int amountToSpawn = 0;

			public System.Random randomGenerator = new System.Random();

			public void NewRandom(int seed){
				randomGenerator = new System.Random(seed);
			}

			public void Start(int number){
				amountSpawned = 0;
				amountToSpawn = number;
				ruleStarted = true;
			}
			public bool Complete(){
				if(amountSpawned >= amountToSpawn){
					ruleidx ++;
					ruleStarted = false;
					return true;
				}else {
					return false;
				}
			}
		}

		//List of placement rules in inspector
		public PlacementRule[] objectsToPool;

		//Internal pool references
		private Dictionary<string, PlacementRule> pools = new Dictionary<string, PlacementRule> ();

		//Spawning configuration
		public int amountToSpawnAtOnce = 1;
		public int amountToDestroyAtOnce = 5;
		private Coroutine spawnloop;
		private Dictionary<SpawnRequest, List<GameObject>> spawnedGoPerRequest = new Dictionary<SpawnRequest, List<GameObject>>();
		private Dictionary<QuadNode<ChunkData>, SpawnRequest> spawnRequestMap = new Dictionary<QuadNode<ChunkData>, SpawnRequest>();
		private DualCollectionStore<SpawnRequest> spawnRequests = new DualCollectionStore<SpawnRequest>();
		private DualCollectionStore<SpawnRequest> destroyRequests = new DualCollectionStore<SpawnRequest>();

		void Start(){

            //Create pools
			foreach (PlacementRule rule in this.objectsToPool) {
                GameObjectPool pool = PoolManager.DefaultInstancePool(rule.rule.prefab, rule.poolBufferSize, rule.poolBufferSize);
				rule.pool = pool;
                if(!pools.ContainsKey(rule.rule.prefab.name))
				    pools.Add (rule.rule.prefab.name, rule);
			}

			spawnloop = StartCoroutine(SpawnLoop());

		}

		/// <summary>
		/// Coroutine loop handling creation and destruction of objects
		/// </summary>
		/// <returns></returns>
		private IEnumerator SpawnLoop(){
			while(true){
				yield return null;

				//Spawn up to amountToSpawnAtOnce objects
				if(spawnRequests.Count > 0 && objectsToPool.Length > 0){
					for(int i = 0; i < amountToSpawnAtOnce; i++){
						SpawnRequest req = spawnRequests.Peek();
						//Completed request, move onto next
						if(req.ruleidx >= objectsToPool.Length){
							spawnRequests.Pop();
							continue;
						}
						PlacementRule rule = objectsToPool[req.ruleidx];

						if(!req.ruleStarted){
							//Get rng seed
							int seed = rule.rule.GetRandomSeed(req.ruleidx, req.node);
							//Create generator
							req.NewRandom(seed);
							//Get number to spawn
							req.Start(rule.rule.Init(req.randomGenerator));
						}

						List<GameObject> objects;
						if(!spawnedGoPerRequest.TryGetValue(req, out objects)){
							objects = new List<GameObject>();
							spawnedGoPerRequest[req] = objects;
						}

						//Spawn a single object
						rule.rule.SpawnObjectOnChunk(
							req.randomGenerator,
							this.transform, 
							req.ruleidx,
							rule.pool, 
							objects, 
							req.node, 
							req.mesh
						);
						//Increment the amount spawned, and if this rule is complete move to the next (eventually moving to the next rule layer)
						req.amountSpawned ++;
						req.Complete();
					}
				}

				//Destroy up to amountToDestroyAtOnce objects
				if(destroyRequests.Count > 0){
					int destroy = amountToDestroyAtOnce;
					while(destroy > 0 && destroyRequests.Count > 0){
						//Destroy request if empty
						SpawnRequest first = destroyRequests.Peek();
						if(first == null){
							destroyRequests.Pop();
							break;
						}

						List<GameObject> toDestroy;
						if(spawnedGoPerRequest.TryGetValue(first, out toDestroy)){
							//Gameobject list associated with request has been cleared
							if(toDestroy.Count < 1){
								destroyRequests.Pop();
								continue;
							}

							//Start destroying gameobjects
							GameObject obj = toDestroy[0];

							PlacementRule pool;
							if (!pools.TryGetValue (obj.name, out pool)) {
								//No pool
								GameObject.Destroy (obj);
							} else {
								//Pool exists
								pool.pool.Push(obj);
							}
							toDestroy.RemoveAt(0);
							destroy --;
							continue;
						}
						//No gameobject list associated with request
						else{
							destroyRequests.Pop();
							continue;
						}
					}
				}
			}
		}		

		/// <summary>
		/// Shows the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void ShowChunkDetails (QuadNode<ChunkData> node,  Mesh m){
			SpawnRequest r = new SpawnRequest();
			r.node = node;
			r.mesh = m;
			spawnRequests.Add(r);
			spawnRequestMap.Add(node, r);
		}

		/// <summary>
		/// Hides the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void HideChunkDetails (QuadNode<ChunkData> node){
			if(spawnRequestMap.ContainsKey(node)){
				SpawnRequest r = spawnRequestMap[node];
				//Stop spawing if still trying to
				spawnRequests.Remove(r);
				spawnRequestMap.Remove(node);
				//Tell system to clean up old objects
				destroyRequests.Add(r);
			}
		}

	}
}