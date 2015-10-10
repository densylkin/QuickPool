using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ObjectsPool : Singleton<ObjectsPool>
{
    public List<Pool> pools = new List<Pool>();
    public bool debugMessages = true;
    public bool spawnDespawnMessages = true;
   
#if UNITY_EDITOR
    public bool foldout;
#endif

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        pools.ForEach(pool => pool.PreInstantiate());
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
        Pool targetPool = ObjectsPool.Instance.pools.Where(pool => pool.poolName == name).FirstOrDefault();
        GameObject obj = targetPool.GetItem();

        if (obj == null) 
        {
            if (ObjectsPool.Instance.debugMessages)
                Debug.Log("No such object left");
            return null;
        }

        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.rotation = rot;

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
        Pool targetPool = ObjectsPool.Instance.pools.Where(pool => pool.Prefab == prefab).FirstOrDefault();
        GameObject obj = targetPool.GetItem();

        if (obj == null)
        {
            if (ObjectsPool.Instance.debugMessages)
                Debug.Log("No such object left");
            return null;
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;

        obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
        return obj;
    }

    /// <summary>
    /// Hides object
    /// </summary>
    /// <param name="target">Target</param>
    public static void Despawn(GameObject target)
    {
        target.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

        Pool targetPool = ObjectsPool.Instance.pools.Where(pool => pool.spawned.Contains(target)).FirstOrDefault();

        targetPool.PushItem(target);
    }
}

[System.Serializable]
public class Pool
{
    #region PublicFields

    public GameObject prefab;
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
    public List<GameObject> despawned = new List<GameObject>();
    public List<GameObject> spawned = new List<GameObject>();
    public Transform m_Root;
    public bool allowMore = false;
    public bool debugMessages = true;
    public int size;

    #endregion

    #region Properties

    public int spawnedCount { get { return spawned.Count; } }
    public int totalCount { get { return spawned.Count + despawned.Count; } }
    public int leftCount { get { return despawned.Count; } }
    public string poolName { get { return prefab == null ? "None" : prefab.name;} }
    public bool Empty { get { return !despawned.Any(); } }
    #endregion


#if UNITY_EDITOR
    public bool foldout;
#endif

    /// <summary>
    /// Creates new pool
    /// </summary>
    /// <returns></returns>
    public static Pool CreateNewPool()
    {
        Pool pool = new Pool();

        GameObject root = new GameObject();
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(root, "root");
#endif
        root.name = "New_Root_Object";
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        pool.m_Root = root.transform;
        root.transform.parent = ObjectsPool.Instance.transform;

        return pool;
    }

    /// <summary>
    /// Preinstantiates all objects
    /// </summary>
    public void PreInstantiate()
    {
        for(int i = 0; i < size; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, m_Root.position, m_Root.rotation) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, "instantiated_obj");
#endif
            despawned.Add(obj);
            obj.transform.SetParent(m_Root);
            obj.SetActive(false);
        }

        if (ObjectsPool.Instance.debugMessages && debugMessages)
            Debug.Log("Pool " + poolName + " spawned");
    }

    /// <summary>
    /// Adds new object to pool if pool is empty
    /// </summary>
    public void AddewObject()
    {
        if (!Empty || !allowMore)
            return;

        GameObject obj = GameObject.Instantiate(prefab, m_Root.position, m_Root.rotation) as GameObject;
        despawned.Add(obj);
        obj.transform.SetParent(m_Root);
        obj.SetActive(false);

        if(ObjectsPool.Instance.debugMessages && debugMessages)
            Debug.Log("New object of " + poolName + "added");
    }

    /// <summary>
    /// Gets an item from pool
    /// </summary>
    /// <returns></returns>
    public GameObject GetItem()
    {
        if (Empty)
        {
            if (allowMore)
            {
                AddewObject();
                size++;
            }
            else
                return null;
        }

        GameObject obj = despawned[0];

        despawned.Remove(obj);
        spawned.Add(obj);
        obj.SetActive(true);
        obj.transform.parent = null;

        return obj;
    }

    /// <summary>
    /// Send item to pool
    /// </summary>
    /// <param name="obj"></param>
    public void PushItem(GameObject obj)
    {
        if (despawned.Contains(obj) || !spawned.Contains(obj))
            return;

        spawned.Remove(obj);
        despawned.Add(obj);
        obj.SetActive(false);
        obj.transform.parent = m_Root;
    }

    /// <summary>
    /// Clears and deletes pool
    /// </summary>
    public void ClearAndDestroy()
    {
        despawned.ForEach(go => Object.DestroyImmediate(go));
        despawned.Clear();
    }
}
