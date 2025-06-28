using System.Collections;
using UnityEngine;

public class CuerdaScript : MonoBehaviour
{

    public float distanciaActivacion = 2f; // Distancia máxima para activarla
    public Transform jugador;
    public bool activada = true;
    public PlataformaScript[] plataformas;
    public VidasPonk VidasPonk;

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
                    StartCoroutine(Activado());
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
            {
                plataforma.Activar();
                activada = true;
            }
        }
    }

    public IEnumerator Activado()
    {
        activada = false;
        yield return new WaitForSeconds(30f);
        activada = true;
    }
}
