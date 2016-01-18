using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuickPool
{
    public class PoolsManager : Singleton<PoolsManager>
    {
        private static PoolsManager instance;
        public static PoolsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<PoolsManager>();
                if (instance == null)
                    instance = new GameObject("Pool manager").AddComponent<PoolsManager>();

                return instance;
            }
        }

        public List<Pool> pools = new List<Pool>();
        public bool debugMessages = true;
        public bool spawnDespawnMessages = true;

        public Pool this[string name]
        {
            get { return pools.Find(pool => pool.poolName == name); }
            
        }

        public List<GameObject> Prefabs
        {
            get
            {
                return pools.Select(pool => pool.Prefab).ToList();
            }
        }

        /// <summary>
        /// Initialization
        /// </summary>
        private void Start()
        {
            //pools.ForEach(pool => pool.PreInstantiate());
            for (int i = 0; i < pools.Count; i++)
                pools[i].Initialize();
        }

        public static Pool CreatePool(GameObject prefabs)
        {
            var pool = new Pool(prefabs);
            RegisterPool(pool);
            return pool;
        }

        /// <summary>
        /// Spawns an object from specified pool
        /// </summary>
        /// <param name="name">Pool name</param>
        /// <param name="pos">Target position</param>
        /// <param name="rot">Target rotation</param>
        /// <returns></returns>
        public static GameObject Spawn(string name, Vector3 pos, Quaternion rot)
        {
            Pool targetPool = PoolsManager.Instance[name];

            if (targetPool == null)
                return null;

            GameObject obj = targetPool.Pop();

            if (obj == null)
            {
                if (PoolsManager.Instance.debugMessages)
                    Debug.Log("No such object left");
                return null;
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;

            if (PoolsManager.Instance.spawnDespawnMessages)
                obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);

            return obj;
        }

        /// <summary>
        /// Spawns an object from specified pool
        /// </summary>
        /// <param name="prefab">Pool name</param>
        /// <param name="pos">Target position</param>
        /// <param name="rot">Target rotation</param>
        /// <returns></returns>
        public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            Pool targetPool = PoolsManager.Instance.pools.Where(pool => pool.Prefab == prefab).FirstOrDefault();

            if (targetPool == null)
                return null;

            GameObject obj = targetPool.Pop();

            if (obj == null)
                return null;

            obj.transform.position = pos;
            obj.transform.rotation = rot;

            if (PoolsManager.Instance.spawnDespawnMessages)
                obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
            return obj;
        }

        /// <summary>
        /// Hides object
        /// </summary>
        /// <param name="target">Target</param>
        public static void Despawn(GameObject target)
        {
            if (PoolsManager.Instance.spawnDespawnMessages)
                target.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

            Pool targetPool = PoolsManager.Instance.pools.Where(pool => pool.spawned.Contains(target)).FirstOrDefault();

            targetPool.Despawn(target);
        }

        public static void DespawnAll()
        {
            for (int i = 0; i < PoolsManager.Instance.pools.Count; i++)
                PoolsManager.Instance.pools[i].DespawnAll();
        }

        public static void RegisterPool(Pool target)
        {
            if (!Instance.pools.Contains(target))
                Instance.pools.Add(target);
        }

        public static void RemovePool(string name)
        {
            var pool = Instance[name];
            if (pool != null)
                Instance.pools.Remove(pool);
        }
    }
}