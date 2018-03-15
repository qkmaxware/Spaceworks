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

        public bool Empty() {
            return pool.Count < 1;
        }

        public virtual T Pop() {
            if (Empty()) {
                return null;
            }
            return pool.Dequeue();
        }

        public virtual void Push(T obj) {
            if(obj != null)
                pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Pool for objects that implement the IPoolable interface
    /// </summary>
    public class PoolablePool: AbstractPool<IPoolable> {
        
        public override IPoolable Pop() {
            IPoolable g = base.Pop();
            if (g != null)
                g.OnCreate();
            return g;
        }

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
        private int bufferSize = 3;
        private Transform parent;
        private GameObject prefab;

        public GameObjectPool(GameObject poolable, int initialSize, int bufferSize = 3, Transform parent = null) {
            this.prefab = poolable;
            this.bufferSize = bufferSize;
            this.parent = parent;
            ExpandPool(initialSize);
        }

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

    public class PoolManager : MonoBehaviour {

        private readonly Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        private readonly Dictionary<string, PoolablePool> customPools = new Dictionary<string, PoolablePool>();

        private static PoolManager instance;
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

        void Awake() {
            Current = this;
        }

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

        public static GameObjectPool DefaultInstancePool(GameObject prefab, int size = 1, int buffer = 3) {
            return Current.InstancePool(prefab, size, buffer);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
            GameObjectPool p = DefaultInstancePool(prefab);
            GameObject instance = p.Pop();
            instance.SetActive(true);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            return instance;
        }

        public static void Destroy(GameObject go) {
            GameObjectPool p = DefaultInstancePool(go);
            p.Push(go);
        }

    }

}
