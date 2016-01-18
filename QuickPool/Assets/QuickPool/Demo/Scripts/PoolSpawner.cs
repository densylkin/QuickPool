using UnityEngine;
using System.Collections;
using QuickPool;

public class PoolSpawner : MonoBehaviour 
{
    public Pool pool = new Pool()
    {
        size = 100,
        allowGrowth = true
    };
    public float delay = 0.1f;

    private void Start()
    {
        PoolsManager.RegisterPool(pool);
        pool.Initialize();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            pool.Spawn(Random.insideUnitSphere * 25, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
    }
}
