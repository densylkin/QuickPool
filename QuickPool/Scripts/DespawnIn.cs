using UnityEngine;
using System.Collections;

public class DespawnIn : MonoBehaviour 
{
    public float time;

    public void OnSpawn()
    {
        StartCoroutine("Destroy");
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(time);
        ObjectsPool.Despawn(gameObject);
    }
}
