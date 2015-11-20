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

        if (targetPool == null)
            return null;

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

        if (ObjectsPool.Instance.spawnDespawnMessages)
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

        if (targetPool == null)
            return null;

        GameObject obj = targetPool.GetItem();

        if (obj == null)
            return null;

        obj.transform.position = pos;
        obj.transform.rotation = rot;

        if (ObjectsPool.Instance.spawnDespawnMessages)
            obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
        return obj;
    }

    /// <summary>
    /// Hides object
    /// </summary>
    /// <param name="target">Target</param>
    public static void Despawn(GameObject target)
    {
        if(ObjectsPool.Instance.spawnDespawnMessages)
            target.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

        Pool targetPool = ObjectsPool.Instance.pools.Where(pool => pool.spawned.Contains(target)).FirstOrDefault();

        targetPool.PushItem(target);
    }
}