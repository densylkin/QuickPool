using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectsPool))]
public class DemoScene : MonoBehaviour 
{
    private ObjectsPool poolObj;

    public GameObject prefabCube;
    public GameObject prefabCapsule;
    public GameObject prefabSphere;

    public GameObject[] prefabs;

    private void Start()
    {
        poolObj = ObjectsPool.Instance;
        StartCoroutine(RandomSpawning());
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200));
        foreach(var pool in poolObj.pools)
        {
            GUILayout.Label("Spawned: " + pool.spawnedCount + "/" + pool.totalCount);
        }
        GUILayout.EndVertical();
    }

    private IEnumerator RandomSpawning()
    {
        while(true)
        {
            for (int i = 0; i < prefabs.Length; i++ )
            {
                Vector3 randomPos = Random.insideUnitSphere * 10;
                ObjectsPool.Spawn(prefabs[i], randomPos, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
