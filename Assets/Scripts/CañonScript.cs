using System.Collections;
using UnityEngine;

public class CañonScript : MonoBehaviour
{
    [SerializeField] private GameObject[] pelotaPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int direccion;
    [SerializeField] private int diferencia;
    public bool activo = false;
    
    [SerializeField] private AudioClip Pelota;
    
    private void Start()
    {
        StartCoroutine(SpawnPelotasRoutine());
    }

    private IEnumerator SpawnPelotasRoutine()
    {
        while (true)
        {
            if (!activo) { yield return null; continue; }
            
            float waitTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(waitTime + diferencia);

            
            int cantidad = Random.Range(1, 4);
            for (int i = 0; i < cantidad; i++)
            {
                if (activo) SpawnPelota();
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    private void SpawnPelota()
    {
        ControladorSonidos.instance.ReproducirSonido(Pelota,1f);
        int i = Random.Range(0, pelotaPrefab.Length); 
        GameObject pelota = Instantiate(pelotaPrefab[i], spawnPoint.position, Quaternion.identity); 
        pelota.GetComponent<PelotaScript>().SetDirection(direccion);
    }
}
