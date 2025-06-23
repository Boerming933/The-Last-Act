using UnityEngine;

public class CuerdaScript : MonoBehaviour
{

    public float distanciaActivacion = 2f; // Distancia máxima para activarla
    private Transform jugador;
    public bool activada = false;
    public PlataformaScript[] plataformas; 

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;
    }

    void Update()
    {
        if (!activada) return;
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
