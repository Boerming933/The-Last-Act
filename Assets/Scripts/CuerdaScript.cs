using System.Collections;
using UnityEngine;

public class CuerdaScript : MonoBehaviour
{

    public float distanciaActivacion = 2f; // Distancia máxima para activarla
    public Transform jugador;
    public bool activada = true;
    public PlataformaScript[] plataformas;
    public VidasPonk VidasPonk;

    [SerializeField] private float velocidadCaida = 5f;
    [SerializeField] private Vector3 posicionFinal;
    private bool cayendo = false;

    [SerializeField] private AudioClip Polea;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;
        posicionFinal = transform.position;
        transform.position += Vector3.up * 10f;
    }

    void Update()
    {
        if (cayendo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidadCaida * Time.deltaTime);

            if (Vector3.Distance(transform.position, posicionFinal) <= 0.01f)
            {
                cayendo = false;
            }

            return; // para evitar que la lógica de activación corra mientras cae
        }
        if (!activada) return;
        if (jugador != null)
        {
            if (Vector2.Distance(transform.position, jugador.position) <= distanciaActivacion)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ControladorSonidos.instance.ReproducirSonido(Polea);
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
            }
        }
    }

    public void DesactivarPlataformas()
    {
        foreach (PlataformaScript plataforma in plataformas)
        {
            if (plataforma != null)
                plataforma.Desactivar();
        }
    }
    public void LlamarDesdeTecho()
    {
        cayendo = true;
    }
}
