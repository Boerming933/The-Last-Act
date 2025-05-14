using UnityEngine;

public class CuerdaScript : MonoBehaviour
{

    private GameObject jefe;
    public float distanciaActivacion = 2f; // Distancia máxima para activarla
    private Transform jugador;

    [Header("Plataformas que se activan")]
    public PlataformaScript[] plataformas; // Asigná las 3 plataformas desde el Inspector

    void Start()
    {
        jefe = GameObject.FindGameObjectWithTag("Triangulardo");
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;
    }

    void Update()
    {
        if (jugador != null)
        {
            if (Vector2.Distance(transform.position, jugador.position) <= distanciaActivacion)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ActivarPlataformas();
                }
            }
        }
    }

    private void ActivarPlataformas()
    {
        foreach (PlataformaScript plataforma in plataformas)
        {
            if (plataforma != null)
                plataforma.Activar();
        }
    }
}
