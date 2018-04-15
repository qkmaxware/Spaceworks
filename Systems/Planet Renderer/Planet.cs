using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Spaceworks.Threading;
using Spaceworks.Pooling;
using System;

namespace Spaceworks {

    [System.Serializable]
    public class PlanetConfig {
        [Header("General information")]
        public string name;
        public float radius = 1;

        [Header("Level of detail")]
        public int lodDepth = 1;
        public int highestQualityAtDistance = 50;

        [Header("References")]
        public IMeshService generationService;
        public ITextureService textureService;
        public IDetailer detailService;

        public PlanetConfig() { }
        public PlanetConfig(PlanetConfig other) {
            this.name = other.name;
            this.radius = other.radius;
            this.lodDepth = other.lodDepth;
            this.highestQualityAtDistance = other.highestQualityAtDistance;
            this.generationService = other.generationService;
            this.textureService = other.textureService;
            this.detailService = other.detailService;
        }
    }

    [System.Serializable]
    public class ChunkData {
        public Sphere bounds;
        public float breakpoint = 50;
        public Zone2 faceRegion;
    }

    public enum PlanetFaceDirection {
        Top,Bottom,Left,Right,Front,Back
    }

    public class PlanetFace {

        private static string PlanetFacePoolTag = "SW_PFpool";
        private static Guid classid;

        static PlanetFace() {
            //Give the mesh pool a very unique id to avoid conflicts with user defined pools
            classid = Guid.NewGuid();
            PlanetFacePoolTag = "pf_"+classid;//+ ":SW_PFpool";
        }


        private class PlanetSplitTask : Task {
            public QuadNode<ChunkData> parent;
            public MeshData[] meshes = new MeshData[4];

            public PlanetSplitTask(System.Action<Task> fn) : base(fn) { }
        }

        private class PlanetMergeTask : Task {
            public QuadNode<ChunkData> node;
            public MeshData mesh;

            public PlanetMergeTask(System.Action<Task> fn) : base(fn) { }
        }

        private class CachedMeshHolder : IPoolable{
            public GameObject gameObject;
            public MeshFilter filter;
            public MeshCollider collider;
            public MeshRenderer renderer;

            public void OnCreate() {}

            public void OnDestroy() {
                gameObject.SetActive(false);
            }
        }

        public PlanetFaceDirection direction {get; private set;}

        public GameObject go { get; private set; }
        public Transform transform { get { return go.transform; } }

        public QuadNode<ChunkData> root { get; private set; }

        public int maxResolutionAt = 50;
        public int maxDepth { get; private set; }

        public float radius { get; private set; }

        public IMeshService meshService { get; private set; }
        public IDetailer detailService { get; private set; }

        private ITextureService textureService;

        //Meshpooling
        private List<CachedMeshHolder> activeMeshes = new List<CachedMeshHolder>();
        //Chunkpooling
        private HashSet<QuadNode<ChunkData>> activeChunks = new HashSet<QuadNode<ChunkData>>();
        private Dictionary<QuadNode<ChunkData>, CachedMeshHolder> chunkMeshMap = new Dictionary<QuadNode<ChunkData>, CachedMeshHolder>();
        //Threading
        private Dictionary<QuadNode<ChunkData>, Task> splitTasks = new Dictionary<QuadNode<ChunkData>, Task>();
        private Dictionary<QuadNode<ChunkData>, Task> mergeTasks = new Dictionary<QuadNode<ChunkData>, Task>();

        //Actionlist
        private List<System.Action<QuadNode<ChunkData>>> listeners = new List<System.Action<QuadNode<ChunkData>>>();

        public PlanetFace(PlanetFaceDirection direction, IMeshService ms, IDetailer ds, float baseRadius, int minDistance, int treeDepth, Zone3 range, ITextureService textureService) {
            //Apply params
            this.direction = direction;
            this.maxResolutionAt = minDistance;
            this.maxDepth = treeDepth;
            this.textureService = textureService;
            this.meshService = ms;
            this.detailService = ds;
            this.radius = baseRadius;

            //Create Gameobjects
            go = new GameObject("PlanetFace");

            //Create quadtree
            root = new QuadNode<ChunkData>(range);
            Vector2 topLeft = new Vector2(0,1);
            Vector2 topRight = new Vector2(1,1);
            Vector2 bottomRight = new Vector2(1,0);
            Vector2 bottomLeft = new Vector2(0,0);
            GenerateChunkdata(
                root, 
                //Default zone for root quadnode - covers whole 0:1 range
                new Zone2(
                    topLeft,
                    topRight,
                    bottomRight,
                    bottomLeft
                )
            );
        }

        /// <summary>
        /// Adds a highest detail listener.
        /// </summary>
        /// <param name="fn">Fn.</param>
        public void AddHighestDetailListener(System.Action<QuadNode<ChunkData>> fn) {
            listeners.Add(fn);
        }

        /// <summary>
        /// Removes a highest detail listener.
        /// </summary>
        /// <param name="fn">Fn.</param>
        public void RemoveHighestDetailListener(System.Action<QuadNode<ChunkData>> fn) {
            listeners.Remove(fn);
        }

        /// <summary>
        /// Force the planet face to pick the best LODs immediately
        /// </summary>
        /// <param name="camera"></param>
        public void ForceUpdateLODs(Vector3 camera) {
            //Deactivate all active LODS
            DiscardAllNodes();

            //Clear all old tasks
            splitTasks.Clear();
            mergeTasks.Clear();

            //Check which LODS need to be activated
            ForceCheckLod(camera, root);
        }

        /// <summary>
        /// Update LODs going up or down a single step only for runtime optimizations
        /// </summary>
        /// <param name="camera"></param>
        public void UpdateLODs(Vector3 camera) {
            //Loop Trough Active Nodes Deciding If We Should Keep Them Or Not
            HashSet<QuadNode<ChunkData>> ac = new HashSet<QuadNode<ChunkData>>();
            foreach (QuadNode<ChunkData> node in this.activeChunks) {
                //Show Node
                if ((node.isBranch || node.depth < maxDepth) && canSplit(camera, node)) {
                    if (node.isLeaf)
                        Split(node);

                    //If I already am generating this LOD
                    if (splitTasks.ContainsKey(node)) {
                        Task t = splitTasks[node];
                        if (t.state == TaskStatus.Complete) {
                            //Can switch to child nodes
                            DiscardNode(node);
                            ShowNodeSplit(node, ((PlanetSplitTask)t).meshes, ac);
                            splitTasks.Remove(node);
                        }
                        else {
                            //Not yet, keep current
                            ShowNode(node, ac);
                        }
                    }
                    //If I am not yet generating this LOD, keep current
                    else {
                        PlanetSplitTask t = new PlanetSplitTask((s) => {
                            PlanetSplitTask self = (PlanetSplitTask)s;

                            //Generate Meshes
                            for (int i = 0; i < 4; i++) {
                                QuadNode<ChunkData> child = self.parent[(Quadrant)i];
                                MeshData m = meshService.Make(
                                    child.range.a, child.range.b, child.range.d, child.range.c,
                                    child.value.faceRegion,
                                    this.radius);
                                self.meshes[i] = m;
                            }
                        });
                        t.parent = node;
                        splitTasks[node] = t;
                        TaskPool.EnqueueInvocation(t);
                        ShowNode(node, ac);
                    }
                }
                //Collapse Or Keep Node
                else {
                    //Is Root, Cannot Split OR Child Wants To Merge But Parent Wants To Split
                    if (node.isRoot || canSplit(camera, node.parent)) {
                        ShowNode(node, ac);
                    }
                    //Child Wants To Merge And Parent Wants To Be Shown
                    else {
                        if (mergeTasks.ContainsKey(node.parent)) {
                            //Generation already started
                            Task t = mergeTasks[node.parent];
                            if (t.state == TaskStatus.Complete) {
                                //Generation is complete, show parent
                                DiscardNode(node.parent[Quadrant.NorthEast]);
                                DiscardNode(node.parent[Quadrant.SouthEast]);
                                DiscardNode(node.parent[Quadrant.NorthWest]);
                                DiscardNode(node.parent[Quadrant.SouthWest]);
                                ShowNodeMerge(node.parent, ((PlanetMergeTask)t).mesh, ac);
                                mergeTasks.Remove(node.parent);
                            }
                            else {
                                //Generation is not complete, keep node
                                ShowNode(node, ac);
                            }
                        }
                        else if (!ac.Contains(node.parent)) {
                            //No generation started, keep node, start generation
                            PlanetMergeTask t = new PlanetMergeTask((s) => {
                                PlanetMergeTask self = (PlanetMergeTask)s;

                                MeshData m = meshService.Make(
                                    self.node.range.a, self.node.range.b, self.node.range.d, self.node.range.c, 
                                    self.node.value.faceRegion,
                                    this.radius);
                                self.mesh = m;
                            });
                            t.node = node.parent;
                            mergeTasks[node.parent] = t;
                            TaskPool.EnqueueInvocation(t);
                            ShowNode(node, ac);
                        }
                    }
                }
            }

            //Set The Active Chunks
            this.activeChunks = ac;
        }

        /// <summary>
        /// Recursive check on this quadnode and its children. Test what LOD to load
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="node"></param>
        private void ForceCheckLod(Vector3 camera, QuadNode<ChunkData> node) {
            //I need to go deeper
            DiscardNode(node);
            if ((node.isBranch || node.depth < maxDepth) && canSplit(camera, node)) {
                if (node.isLeaf)
                    Split(node);
                ForceCheckLod(camera, node[Quadrant.NorthEast]);
                ForceCheckLod(camera, node[Quadrant.NorthWest]);
                ForceCheckLod(camera, node[Quadrant.SouthEast]);
                ForceCheckLod(camera, node[Quadrant.SouthWest]);
            }
            //Nope this is good (don't need to worry about merges on a forced method)
            else {
                //Show LOD
                ShowNode(node, this.activeChunks);
            }
        }

        /// <summary>
        /// Create metadata from a quadnode
        /// </summary>
        /// <param name="node"></param>
        private void GenerateChunkdata(QuadNode<ChunkData> node, Zone2 region) {
            ChunkData data = new ChunkData();
            data.bounds = null;
            data.breakpoint = maxResolutionAt * Mathf.Pow(2, maxDepth - node.depth);
            data.faceRegion = region;
            node.value = data;
        }

        private void GenerateChildChunkdata(QuadNode<ChunkData> parent){
            Zone2 NE; Zone2 NW; Zone2 SE; Zone2 SW;
            parent.value.faceRegion.QuadSubdivide(out NE, out NW, out SE, out SW);
            
            GenerateChunkdata(parent[Quadrant.NorthEast], NE);
            GenerateChunkdata(parent[Quadrant.NorthWest], NW);
            GenerateChunkdata(parent[Quadrant.SouthEast], SE);
            GenerateChunkdata(parent[Quadrant.SouthWest], SW);
            
        }

        /// <summary>
        /// Split a quadnode and create the appropriate metadata
        /// </summary>
        /// <param name="parent"></param>
        private void Split(QuadNode<ChunkData> parent) {
            parent.Subdivide();

            GenerateChildChunkdata(parent);
        }

        private void DiscardNode(QuadNode<ChunkData> node) {
            CachedMeshHolder mf;
            if (chunkMeshMap.TryGetValue(node, out mf)) {
                //Hide and pool node
                this.activeMeshes.Remove(mf);
                PoolManager.Current.CustomPool(PlanetFacePoolTag).Push(mf);
                //mf.collider.enabled = false;
                mf.gameObject.SetActive(false);
                //Discard active node
                this.chunkMeshMap.Remove(node);
                //NOTE ** do not need to remove from activeChunks because the runtime algorithm resets this each frame
                //Call highest detail action
                if (node.depth == this.maxDepth) {
                    //Call detailing service if available
                    if (detailService != null)
                        detailService.HideChunkDetails(node);
                }
            }
        }

        private void DiscardAllNodes() {
            foreach (CachedMeshHolder mf in this.activeMeshes) {
                mf.gameObject.SetActive(false);
                //mf.collider.enabled = false;
                PoolManager.Current.CustomPool(PlanetFacePoolTag).Push(mf);
            }
            activeMeshes.Clear();
            chunkMeshMap.Clear();
            activeChunks.Clear();
        }

        /// <summary>
        /// Pop an unused container from the queue, or generate new containers if required
        /// </summary>
        /// <returns></returns>
        private CachedMeshHolder PopMeshContainer() {
            PoolablePool meshPool = PoolManager.Current.CustomPool(PlanetFacePoolTag, 3, 3);
            while (meshPool.Count < 3) {
                GameObject g = new GameObject("Chunk");
                g.transform.SetParent(go.transform);
                g.transform.localPosition = Vector3.zero;
                g.transform.localScale = Vector3.one;

                MeshFilter meshFilter = g.AddComponent<MeshFilter>();
                Mesh m = new Mesh();
                m.name = "Cached Chunk Mesh";
                meshFilter.sharedMesh = m;

                MeshRenderer meshRenderer = g.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = null;
                g.SetActive(false);

                MeshCollider collider = g.AddComponent<MeshCollider>();
                //collider.enabled = false;

                CachedMeshHolder holder = new CachedMeshHolder();
                holder.gameObject = g;
                holder.filter = meshFilter;
                holder.collider = collider;
                holder.renderer = meshRenderer;

                meshPool.Push(holder);
            }

            CachedMeshHolder container = (CachedMeshHolder)meshPool.Pop();
            if (container.gameObject.transform.parent != go.transform) {
                container.gameObject.transform.SetParent(go.transform);
                container.gameObject.transform.localPosition = Vector3.zero;
                container.gameObject.transform.localScale = Vector3.one;
            }
            return container;
        }

        private void ShowNodeMerge(QuadNode<ChunkData> node, MeshData mesh, HashSet<QuadNode<ChunkData>> activeList) {

            if (!chunkMeshMap.ContainsKey(node)) {
                //Buffer
                CachedMeshHolder container = PopMeshContainer();
                MeshFilter filter = container.filter;
                filter.sharedMesh = mesh.mesh;
                container.collider.sharedMesh = filter.sharedMesh;
                container.collider.enabled = true;

                container.renderer.sharedMaterial = textureService.GetMaterialFor(this, node, filter.sharedMesh);

                if (node.value.bounds == null) {
                    node.value.bounds = new Sphere(Vector3.zero, 1);
                    node.value.bounds.center = filter.sharedMesh.bounds.center;
                    node.value.bounds.radius = Mathf.Sqrt(
                        filter.sharedMesh.bounds.extents.x * filter.sharedMesh.bounds.extents.x +
                        filter.sharedMesh.bounds.extents.y * filter.sharedMesh.bounds.extents.y +
                        filter.sharedMesh.bounds.extents.z * filter.sharedMesh.bounds.extents.z
                    );
                }

                //Show node
                filter.gameObject.SetActive(true);

                //Call highest detail action
                if (node.depth == this.maxDepth) {
                    //Call listeners if exists
                    foreach (System.Action<QuadNode<ChunkData>> fn in this.listeners) {
                        fn.Invoke(node);
                    }
                    //Call detailing service if available
                    if (detailService != null)
                        detailService.ShowChunkDetails(node, filter.sharedMesh);
                }

                //Add me if I don't already exist
                this.activeMeshes.Add(container);
                this.chunkMeshMap[node] = container;
            }
            if (!activeList.Contains(node))
                activeList.Add(node);
        }

        private void ShowNodeSplit(QuadNode<ChunkData> parent, MeshData[] meshes, HashSet<QuadNode<ChunkData>> activeList) {
            for (int i = 0; i < 4; i++) {
                QuadNode<ChunkData> child = parent[(Quadrant)i];
                if (!chunkMeshMap.ContainsKey(child)) {
                    //Buffer
                    CachedMeshHolder container = PopMeshContainer();
                    MeshFilter filter = container.filter;
                    filter.sharedMesh = meshes[i].mesh;
                    container.collider.sharedMesh = filter.sharedMesh;
                    container.collider.enabled = true;

                    container.renderer.sharedMaterial = textureService.GetMaterialFor(this, child, filter.sharedMesh);

                    if (child.value.bounds == null) {
                        child.value.bounds = new Sphere(Vector3.zero, 1);
                        child.value.bounds.center = filter.sharedMesh.bounds.center;
                        child.value.bounds.radius = Mathf.Sqrt(
                            filter.sharedMesh.bounds.extents.x * filter.sharedMesh.bounds.extents.x +
                            filter.sharedMesh.bounds.extents.y * filter.sharedMesh.bounds.extents.y +
                            filter.sharedMesh.bounds.extents.z * filter.sharedMesh.bounds.extents.z
                        );
                    }

                    //Show node
                    filter.gameObject.SetActive(true);

                    //Call highest detail action
                    if (child.depth == this.maxDepth) {
                        //Call listeners if exists
                        foreach (System.Action<QuadNode<ChunkData>> fn in this.listeners) {
                            fn.Invoke(child);
                        }
                        //Call detailing service if available
                        if (detailService != null)
                            detailService.ShowChunkDetails(child, filter.sharedMesh);
                    }

                    //Add me if I don't already exist
                    this.activeMeshes.Add(container);
                    this.chunkMeshMap[child] = container;
                }
                if (!activeList.Contains(child))
                    activeList.Add(child);
            }
        }

        private void ShowNode(QuadNode<ChunkData> node, HashSet<QuadNode<ChunkData>> activeList) {
            if (!chunkMeshMap.ContainsKey(node)) {
                //Buffer
                CachedMeshHolder container = PopMeshContainer();
                MeshFilter filter = container.filter;
                container.collider.sharedMesh = filter.sharedMesh;
                container.collider.enabled = true;

                //Populate mesh
                filter.sharedMesh = meshService.Make(
                    node.range.a, node.range.b, node.range.d, node.range.c,
                    node.value.faceRegion,
                    this.radius).mesh;
                //filter.sharedMesh = SubPlane.Make(node.range.a, node.range.b, node.range.d, node.range.c, resolution); 

                container.renderer.sharedMaterial = textureService.GetMaterialFor(this, node, filter.sharedMesh);

                //Set chunk data if it was never computed before
                if (node.value.bounds == null) {
                    node.value.bounds = new Sphere(Vector3.zero, 1);
                    node.value.bounds.center = filter.sharedMesh.bounds.center;
                    /*node.value.bounds.radius = Mathf.Max (
						filter.sharedMesh.bounds.extents.x,
						filter.sharedMesh.bounds.extents.y,
						filter.sharedMesh.bounds.extents.z
					);*/
                    node.value.bounds.radius = Mathf.Sqrt(
                        filter.sharedMesh.bounds.extents.x * filter.sharedMesh.bounds.extents.x +
                        filter.sharedMesh.bounds.extents.y * filter.sharedMesh.bounds.extents.y +
                        filter.sharedMesh.bounds.extents.z * filter.sharedMesh.bounds.extents.z
                    );
                }

                //Show node
                filter.gameObject.SetActive(true);

                //Call highest detail action
                if (node.depth == this.maxDepth) {
                    //Call listeners if exists
                    foreach (System.Action<QuadNode<ChunkData>> fn in this.listeners) {
                        fn.Invoke(node);
                    }
                    //Call detailing service if available
                    if (detailService != null)
                        detailService.ShowChunkDetails(node, filter.sharedMesh);
                }

                //Add me if I don't already exist
                this.activeMeshes.Add(container);
                this.chunkMeshMap[node] = container;
            }
            if (!activeList.Contains(node))
                activeList.Add(node);
        }

        /// <summary>
        /// Test if a quadnode needs to be split IE viewport is too close
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool canSplit(Vector3 camera, QuadNode<ChunkData> node) {
            if (node.value.bounds == null)
                return false;
            float distance = Vector3.Distance(camera, transform.TransformPoint(node.value.bounds.center));
            return (distance - node.value.bounds.radius) < node.value.breakpoint;
        }

        /// <summary>
        /// Loops function for each of the active quadtree nodes
        /// </summary>
        /// <param name="fn">Fn.</param>
        public void ForEachActiveLOD(System.Action<QuadNode<ChunkData>> fn) {
            foreach (QuadNode<ChunkData> node in activeChunks) {
                fn.Invoke(node);
            }
        }

        /// <summary>
		/// Loops function for each of the active mesh filter components
		/// </summary>
		/// <param name="fn">Fn.</param>
		public void ForEachActiveMesh(System.Action<MeshFilter> fn) {
            foreach (CachedMeshHolder container in this.activeMeshes) {
                fn.Invoke(container.filter);
            }
        }

        /// <summary>
		/// Loops function for each of the active mesh collider components
		/// </summary>
		/// <param name="fn">Fn.</param>
		public void ForEachActiveCollider(System.Action<MeshCollider> fn) {
            foreach (CachedMeshHolder container in this.activeMeshes) {
                fn.Invoke(container.collider);
            }
        }

        /// <summary>
		/// Loops function for each of the active gameobjects
		/// </summary>
		/// <param name="fn">Fn.</param>
		public void ForEachActiveGameobject(System.Action<GameObject> fn) {
            foreach (CachedMeshHolder container in this.activeMeshes) {
                fn.Invoke(container.gameObject);
            }
        }

    }

    public class Planet {

        public PlanetConfig config { get; private set; }

        public PlanetFace topFace { get; private set; }
        public PlanetFace bottomFace { get; private set; }
        public PlanetFace leftFace { get; private set; }
        public PlanetFace rightFace { get; private set; }
        public PlanetFace frontFace { get; private set; }
        public PlanetFace backFace { get; private set; }

        public Planet(PlanetConfig config) {
            this.config = config;

        }

        /// <summary>
        /// Force planet to update all LOD levels
        /// </summary>
        /// <param name="camera"></param>
        public void ForceUpdateLODs(Vector3 camera) {
            topFace.ForceUpdateLODs(camera);
            bottomFace.ForceUpdateLODs(camera);
            leftFace.ForceUpdateLODs(camera);
            rightFace.ForceUpdateLODs(camera);
            frontFace.ForceUpdateLODs(camera);
            backFace.ForceUpdateLODs(camera);
        }

        /// <summary>
        /// Cause the planet to perform a stepped update of active LOD levels
        /// </summary>
        /// <param name="camera"></param>
        public void UpdateLODs(Vector3 camera) {
            topFace.UpdateLODs(camera);
            bottomFace.UpdateLODs(camera);
            leftFace.UpdateLODs(camera);
            rightFace.UpdateLODs(camera);
            frontFace.UpdateLODs(camera);
            backFace.UpdateLODs(camera);
        }

        public void UpdateMaterial(GameObject center, bool forceRefresh = false) {
            if (this.config.textureService != null) {
                if (forceRefresh == true) {
                    this.config.textureService.Init(this.config);
                }
                this.config.textureService.SetPlanetCenter(center.transform.position);
            }
        }

        public void RenderOn(GameObject go) {
            //Create ranges for cubic faces
            float rad = 1;
            Zone3 topRange = new Zone3(new Vector3(-rad, rad, rad), new Vector3(rad, rad, rad), new Vector3(rad, rad, -rad), new Vector3(-rad, rad, -rad));
            Zone3 bottomRange = new Zone3(new Vector3(-rad, -rad, -rad), new Vector3(rad, -rad, -rad), new Vector3(rad, -rad, rad), new Vector3(-rad, -rad, rad));

            Zone3 frontRange = new Zone3(new Vector3(rad, rad, rad), new Vector3(-rad, rad, rad), new Vector3(-rad, -rad, rad), new Vector3(rad, -rad, rad));
            Zone3 backRange = new Zone3(new Vector3(-rad, rad, -rad), new Vector3(rad, rad, -rad), new Vector3(rad, -rad, -rad), new Vector3(-rad, -rad, -rad));

            Zone3 rightRange = new Zone3(new Vector3(rad, rad, -rad), new Vector3(rad, rad, rad), new Vector3(rad, -rad, rad), new Vector3(rad, -rad, -rad));
            Zone3 leftRange = new Zone3(new Vector3(-rad, rad, rad), new Vector3(-rad, rad, -rad), new Vector3(-rad, -rad, -rad), new Vector3(-rad, -rad, rad));

            if (config.generationService)
                config.generationService.Init();

            UpdateMaterial(go, true);

            this.topFace = new PlanetFace(PlanetFaceDirection.Top, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, topRange, config.textureService);
            this.topFace.go.name = "Top";
            this.topFace.transform.SetParent(go.transform);
            this.topFace.transform.localPosition = Vector3.zero;

            this.bottomFace = new PlanetFace(PlanetFaceDirection.Bottom, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, bottomRange, config.textureService);
            this.bottomFace.go.name = "Bottom";
            this.bottomFace.transform.SetParent(go.transform);
            this.bottomFace.transform.localPosition = Vector3.zero;

            this.leftFace = new PlanetFace(PlanetFaceDirection.Left, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, leftRange, config.textureService);
            this.leftFace.go.name = "Left";
            this.leftFace.transform.SetParent(go.transform);
            this.leftFace.transform.localPosition = Vector3.zero;

            this.rightFace = new PlanetFace(PlanetFaceDirection.Right, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, rightRange, config.textureService);
            this.rightFace.go.name = "Right";
            this.rightFace.transform.SetParent(go.transform);
            this.rightFace.transform.localPosition = Vector3.zero;

            this.backFace = new PlanetFace(PlanetFaceDirection.Back, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, backRange, config.textureService);
            this.backFace.go.name = "Back";
            this.backFace.transform.SetParent(go.transform);
            this.backFace.transform.localPosition = Vector3.zero;

            this.frontFace = new PlanetFace(PlanetFaceDirection.Front, config.generationService, config.detailService, config.radius, config.highestQualityAtDistance, config.lodDepth, frontRange, config.textureService);
            this.frontFace.go.name = "Front";
            this.frontFace.transform.SetParent(go.transform);
            this.frontFace.transform.localPosition = Vector3.zero;
        }

    }

}