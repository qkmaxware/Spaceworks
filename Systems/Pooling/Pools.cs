using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Pooling {

    /// <summary>
    /// Interface for poolable objects. Use with classes that extend PoolablePool
    /// </summary>
    public interface IPoolable {
        void OnCreate();
        void OnDestroy();
    }

    /// <summary>
    /// Base class for pools. Pool any object type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractPool<T> where T : class {
        protected Queue<T> pool = new Queue<T>();

        /// <summary>
        /// Check if pool is empty
        /// </summary>
        /// <returns></returns>
        public bool Empty() {
            return pool.Count < 1;
        }

        /// <summary>
        /// Count pooled instances
        /// </summary>
        /// <returns></returns>
        public int Count {
            get {
                return pool.Count;
            }
        }

        /// <summary>
        /// Get an item from the pool if not empty; Else get null.
        /// </summary>
        /// <returns></returns>
        public virtual T Pop() {
            if (Empty()) {
                return null;
            }
            return pool.Dequeue();
        }

        /// <summary>
        /// Add an item to the pool
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Push(T obj) {
            if(obj != null)
                pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Pool for objects that implement the IPoolable interface
    /// </summary>
    public class PoolablePool: AbstractPool<IPoolable> {
        
        /// <summary>
        /// Action to invoke when removed from pool
        /// </summary>
        /// <returns></returns>
        public override IPoolable Pop() {
            IPoolable g = base.Pop();
            if (g != null)
                g.OnCreate();
            return g;
        }

        /// <summary>
        /// Action to call when added to pool
        /// </summary>
        /// <param name="obj"></param>
        public override void Push(IPoolable obj) {
            if (obj != null) {
                obj.OnDestroy();
            }
            base.Push(obj);
        }
    }

    /// <summary>
    /// GameObject pool that instanciates new prefabs as required
    /// </summary>
    public class GameObjectPool : AbstractPool<GameObject> {
        /// <summary>
        /// Number of objects to buffer
        /// </summary>
        private int bufferSize = 3;
        /// <summary>
        /// Where to store buffered instances
        /// </summary>
        private Transform parent;
        /// <summary>
        /// Object to buffer
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// Pool a gameobject
        /// </summary>
        /// <param name="poolable">Object to pool</param>
        /// <param name="initialSize">Size of pool</param>
        /// <param name="bufferSize">Size to maintain as a buffer</param>
        /// <param name="parent">Object to parent inactive references to</param>
        public GameObjectPool(GameObject poolable, int initialSize, int bufferSize = 3, Transform parent = null) {
            this.prefab = poolable;
            this.bufferSize = bufferSize;
            this.parent = parent;
            ExpandPool(initialSize);
        }

        /// <summary>
        /// Get object from pool, expanding the pool if nessesary
        /// </summary>
        /// <returns></returns>
        public override GameObject Pop() {
            if (Empty()) {
                ExpandPool(bufferSize);
            }
            GameObject go = base.Pop();
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            return go;
        }

        /// <summary>
        /// Add object to pool, and deactivate it
        /// </summary>
        /// <param name="go"></param>
        public override void Push(GameObject go) {
            base.Push(go);
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

    /// <summary>
    /// Represents a collection of object pools
    /// </summary>
    public class PoolManager : MonoBehaviour {

        /// <summary>
        /// Pools of Unity3d GameObjects
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        /// <summary>
        /// Generic pools
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<string, PoolablePool> customPools = new Dictionary<string, PoolablePool>();

        private static PoolManager instance;
        /// <summary>
        /// Current static pool manager instance. Created if it doesn't exist
        /// </summary>
        /// <returns></returns>
        public static PoolManager Current {
            get {
                if (instance) {
                    return instance;
                }
                else {
                    GameObject mgr = new GameObject("Pool Manager");
                    instance = mgr.AddComponent<PoolManager>();
                    return instance;
                }
            }
            private set{
                instance = value;
            }
        }

        /// <summary>
        /// Set instance
        /// </summary>
        void Awake() {
            Current = this;
        }

        /// <summary>
        /// Create gameobject pool for a given prefab. Or fetch pool if already exists.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="size"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public GameObjectPool InstancePool(GameObject prefab, int size = 1, int buffer = 3) {
            if (Current.pools.ContainsKey(prefab.name)) {
                return Current.pools[prefab.name];
            }
            else {
                GameObjectPool p = new GameObjectPool(prefab, size, buffer, Current.transform);
                Current.pools[prefab.name] = p;
                return p;
            }
        }

        /// <summary>
        /// Create generic pool by name or fetch pool if it exists.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public PoolablePool CustomPool(string tag, int size = 1, int buffer = 3) {
            if (Current.customPools.ContainsKey(tag)) {
                return Current.customPools[tag];
            }
            else {
                PoolablePool p = new PoolablePool();
                Current.customPools[tag] = p;
                return p;
            }
        }

        /// <summary>
        /// Get a reference to the gameobject pool of the static manager
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="size"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static GameObjectPool DefaultInstancePool(GameObject prefab, int size = 1, int buffer = 3) {
            return Current.InstancePool(prefab, size, buffer);
        }

        /// <summary>
        /// Get a reference to the generic pool of the static manager
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static PoolablePool DefaultCustomPool(string tag, int size = 1, int buffer = 3) {
            return Current.CustomPool(tag, size, buffer);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
            GameObjectPool p = DefaultInstancePool(prefab);
            GameObject instance = p.Pop();
            instance.SetActive(true);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            return instance;
        }

        /// <summary>
        /// Destroy a gameobject, adding it to the static manager's pool
        /// </summary>
        /// <param name="go"></param>
        public static void Destroy(GameObject go) {
            GameObjectPool p = DefaultInstancePool(go);
            p.Push(go);
        }

    }

}
