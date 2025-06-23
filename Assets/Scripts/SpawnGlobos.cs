using System.Collections;
using UnityEngine;

public class SpawnGlobos : MonoBehaviour
{
    public GameObject[] globoPrefab;
    public Transform spawnIzquierda;
    public Transform spawnDerecha;
    public bool activo = false;

    void Start()
    {
        StartCoroutine(SpawnearGlobos());
    }

    IEnumerator SpawnearGlobos()
    {
        while (true)
        {
            if (!activo) { yield return null; continue; }
            float tiempoEspera = Random.Range(2, 5);
            yield return new WaitForSeconds(tiempoEspera);

            if (!activo) continue;

            // Decide lado (izquierda o derecha)
            bool Lado = Random.value > 0.5f;
            Transform spawnPoint = Lado ? spawnIzquierda : spawnDerecha;
            int i = Random.Range(0, globoPrefab.Length);
            Instantiate(globoPrefab[i], spawnPoint.position, Quaternion.identity);
        }
    }
}
