using UnityEngine;
using System.Collections;

namespace QuickPool
{
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
            gameObject.Despawn();
        }
    }
}