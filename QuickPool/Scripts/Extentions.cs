using UnityEngine;
using System.Collections;

public static class Extentions
{
    public static void Spawn(this GameObject prefabToSpawn, Vector3 pos, Quaternion rot)
    {
        ObjectsPool.Spawn(prefabToSpawn, pos, rot);
    }

    public static void Despawn(this GameObject objToDespawn)
    {
        ObjectsPool.Despawn(objToDespawn);
    }
}