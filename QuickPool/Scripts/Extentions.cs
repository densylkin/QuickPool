using UnityEngine;
using System.Collections;

namespace QuickPool
{
    public static class Extentions
    {
        public static GameObject Spawn(this GameObject prefabToSpawn, Vector3 pos, Quaternion rot)
        {
            return ObjectsPool.Spawn(prefabToSpawn, pos, rot);
        }

        public static void Despawn(this GameObject objToDespawn)
        {
            ObjectsPool.Despawn(objToDespawn);
        }
    }
}