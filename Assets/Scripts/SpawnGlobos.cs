using System.Collections;
using UnityEngine;

public class SpawnGlobos : MonoBehaviour
{
    public GameObject globoPrefab;
    public Transform spawnIzquierda;
    public Transform spawnDerecha;

    void Start()
    {
        StartCoroutine(SpawnearGlobos());
    }

    IEnumerator SpawnearGlobos()
    {
        while (true)
        {
            float tiempoEspera = Random.Range(2, 5);
            yield return new WaitForSeconds(tiempoEspera);

            // Decide lado (izquierda o derecha)
            bool Lado = Random.value > 0.5f;
            Transform spawnPoint = Lado ? spawnIzquierda : spawnDerecha;

            Instantiate(globoPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
