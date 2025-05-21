using System.Collections;
using UnityEngine;

public class CañonScript : MonoBehaviour
{
    [SerializeField] private GameObject pelotaPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int direccion;
    [SerializeField] private int diferencia;
    
    private void Start()
    {
        StartCoroutine(SpawnPelotasRoutine());
    }

    private IEnumerator SpawnPelotasRoutine()
    {
        while (true)
        {
            // Espera entre 3 y 7 segundos
            float waitTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(waitTime + diferencia);

            // Spawnea entre 1 y 3 pelotas
            int cantidad = Random.Range(1, 4);
            for (int i = 0; i < cantidad; i++)
            {
                SpawnPelota();
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    private void SpawnPelota()
    {
        GameObject pelota = Instantiate(pelotaPrefab, spawnPoint.position, Quaternion.identity);
        pelota.GetComponent<PelotaScript>().SetDirection(direccion);
    }
}
