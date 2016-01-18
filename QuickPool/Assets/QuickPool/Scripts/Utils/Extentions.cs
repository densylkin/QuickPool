using UnityEngine;
using System.Collections;

namespace QuickPool
{
    public static class Extentions
    {
        public static GameObject Spawn(this GameObject prefabToSpawn, Vector3 pos, Quaternion rot)
        {
            return PoolsManager.Spawn(prefabToSpawn, pos, rot);
        }

        public static T Spawn<T>(this GameObject prefabToSpawn, Vector3 pos, Quaternion rot) where T : Component
        {
            return PoolsManager.Spawn(prefabToSpawn, pos, rot).GetComponent<T>();
        }

        public static void Despawn(this GameObject objToDespawn)
        {
            PoolsManager.Despawn(objToDespawn);
        }
    }
}