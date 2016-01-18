using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace QuickPool
{

    public enum DespawnMode
    {
        Deactivate,
        Move
    }

    public enum PoolType
    {
        GameObject,
        AudioSource,
        ParticleSystem
    }

    [System.Flags]
    public enum ComponentHandlingType
    {
        playOnSpawn = (1 << 0),
        stopOnDespawn = (1 << 1)
    }

    [System.Serializable]
    public class Pool
    {
        #region PublicFields

        public GameObject prefab = null;
        public GameObject Prefab
        {
            get { return prefab; }
            set
            {
                if (Application.isEditor)
                {
                    prefab = value;
                    if (prefab != null && m_Root != null)
                        m_Root.name = poolName + "_Root";
                }
            }
        }

        public string id;

        public DespawnMode mode = DespawnMode.Deactivate;
        public Vector3 despawnPos = new Vector3(-100, -100, -100);
        public List<GameObject> despawned = new List<GameObject>();
        public List<GameObject> spawned = new List<GameObject>();
        public Transform m_Root;

        public bool allowGrowth = false;
        public bool debugMessages = true;
        public bool useCustomRoot = false;

        public ComponentHandlingType audioSourceHandling;
        public ComponentHandlingType particleSystemHandling;

        public int size;
        public bool playOnSpawn;
        public bool stopOnDespawn;
        #endregion

        #region Properties

        public int spawnedCount { get { return spawned.Count; } }
        public int totalCount { get { return spawned.Count + despawned.Count; } }
        public int leftCount { get { return despawned.Count; } }
        public string poolName { get { return prefab == null ? "None" : prefab.name; } }
        public bool Empty { get { return despawned.Count == 0; } }
        public bool Audio;
        public bool Particles;
        public bool GO { get { return !Audio && !Particles; } }

        public PoolType Type
        {
            get
            {
                if (prefab.GetComponent<AudioSource>() != null)
                    return PoolType.AudioSource;
                else if (prefab.GetComponent<ParticleSystem>() != null)
                    return PoolType.ParticleSystem;
                else
                    return PoolType.GameObject;
            }
        }

        #endregion

#if UNITY_EDITOR
        public bool foldout;
#endif

        public Pool() { }

        /// <summary>
        /// Creates new pool
        /// </summary>
        /// <returns></returns>
        public Pool(GameObject _prefab) 
        {
            if(_prefab != null)
                this.Prefab = _prefab;
            size = 1;
            Audio = Prefab.GetComponent<AudioSource>() != null;
            Particles = Prefab.GetComponent<ParticleSystem>() != null;
        }

        /// <summary>
        /// Preinstantiates all objects
        /// </summary>
        public void Initialize()
        {
            if (prefab == null)
                return;

            if (m_Root == null)
                m_Root = Utils.CreateRoot(prefab.name).transform;

            for (int i = 0; i < size; i++)
            {
                if (totalCount > size)
                    break;

                GameObject obj = GameObject.Instantiate(prefab, m_Root.position, m_Root.rotation) as GameObject;
#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(obj, "instantiated_obj");
#endif
                despawned.Add(obj);
                obj.transform.SetParent(m_Root);
                if (mode == DespawnMode.Deactivate)
                    obj.SetActive(false);
                else
                    obj.transform.position = despawnPos;

                obj.name = poolName;
            }

            //if (ObjectsPool.Instance.debugMessages && debugMessages)
            //    Debug.Log("Pool " + poolName + " spawned");
        }

        /// <summary>
        /// Adds new object to pool if pool is empty
        /// </summary>
        public void AddewObject()
        {
            if (!Empty || !allowGrowth)
                return;

            GameObject obj = GameObject.Instantiate(prefab, m_Root.position, m_Root.rotation) as GameObject;
            despawned.Add(obj);
            obj.transform.SetParent(m_Root);
            obj.SetActive(false);
            obj.name = prefab.name;

            if (PoolsManager.Instance.debugMessages && debugMessages)
                Debug.Log("New object of " + poolName + "added");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public T Spawn<T>(Vector3 pos, Quaternion rot) where T : Component
        {
            return Spawn(pos, rot).GetComponent<T>();
        }

        /// <summary>
        /// Used to spawn objects
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            var obj = Pop();

            if (obj == null)
            {
                if (PoolsManager.Instance.debugMessages)
                    Debug.Log("No such object left");
                return null;
            }

            var objt = obj.transform;
            objt.position = pos;
            objt.rotation = rot;

            if (PoolsManager.Instance.spawnDespawnMessages)
                obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);

            if((audioSourceHandling & ComponentHandlingType.playOnSpawn) == ComponentHandlingType.playOnSpawn)
                if (Audio)
                    obj.GetComponent<AudioSource>().Play();

            if((particleSystemHandling & ComponentHandlingType.playOnSpawn) == ComponentHandlingType.playOnSpawn)
                if (Particles)
                    obj.GetComponent<ParticleSystem>().Play();

            return obj;
        }

        /// <summary>
        /// Used to despawn message
        /// </summary>
        /// <param name="target"></param>
        public void Despawn(GameObject target)
        {
            if (!spawned.Contains(target))
                return;

            Push(target);

            if (PoolsManager.Instance.spawnDespawnMessages)
                target.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

            if ((audioSourceHandling & ComponentHandlingType.stopOnDespawn) == ComponentHandlingType.stopOnDespawn)
                if (Audio)
                    target.GetComponent<AudioSource>().Play();

            if ((particleSystemHandling & ComponentHandlingType.stopOnDespawn) == ComponentHandlingType.stopOnDespawn)
                if (Particles)
                    target.GetComponent<ParticleSystem>().Play();
        }

        /// <summary>
        /// Gets an item from pool
        /// </summary>
        /// <returns></returns>
        public GameObject Pop()
        {
            if (Empty)
            {
                if (allowGrowth)
                {
                    AddewObject();
                    size++;
                }
                else
                    return null;
            }

            GameObject obj = despawned[0];

            if (obj == null)
                return null;

            despawned.Remove(obj);
            spawned.Add(obj);
            if (mode == DespawnMode.Deactivate)
                obj.SetActive(true);
            obj.transform.parent = null;

            return obj;
        }

        /// <summary>
        /// Send item to pool
        /// </summary>
        /// <param name="obj"></param>
        public void Push(GameObject obj)
        {
            if (despawned.Contains(obj) || !spawned.Contains(obj))
                return;

            spawned.Remove(obj);
            despawned.Add(obj);
            if (mode == DespawnMode.Deactivate)
                obj.SetActive(false);
            else
                obj.transform.position = despawnPos;
            obj.transform.parent = m_Root;
        }

        /// <summary>
        /// Clears and deletes pool
        /// </summary>
        public void ClearAndDestroy()
        {
            for (int i = 0; i < despawned.Count; i++)
                Object.DestroyImmediate(despawned[i]);

            despawned.Clear();
        }

        /// <summary>
        /// Despawns all spawned objects
        /// </summary>
        public void DespawnAll()
        {
            for (int i = 0; i < spawned.Count; i++)
                Push(spawned[i]);
        }
    }
}