using System.Collections;
using UnityEngine;

public class PlataformaScript : MonoBehaviour
{
    public Triangulardo jefe;
    public float velocidadCaida = 5f;
    public float distanciaCaida = 2f;
    private Vector3 posicionInicial;
    public bool activada = false;
    private bool bajando = false;
    private bool subiendo = false;
    private float alturaMinima;
    private BoxCollider2D boxCollider;

    [SerializeField] private AudioClip ImpactoTrapecio;

    void Start()
    {
        posicionInicial = transform.position;
        alturaMinima = posicionInicial.y - distanciaCaida;
        jefe = GetComponent<Triangulardo>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!activada) return;

        if (bajando)
        {
            boxCollider.isTrigger = true;
            transform.position += Vector3.down * velocidadCaida * Time.deltaTime;

            if (transform.position.y <= alturaMinima)
            {
                transform.position = new Vector3(transform.position.x, alturaMinima, transform.position.z);
                bajando = false;
                subiendo = true;
            }
        }
        else if (subiendo)
        {
            transform.position += Vector3.up * velocidadCaida * Time.deltaTime;

            if (transform.position.y >= posicionInicial.y)
            {
                transform.position = posicionInicial;
                subiendo = false;
                boxCollider.isTrigger = false;
                activada = false;
            }
        }
    }

    public void Activar()
    {
        if (!activada)
        {
            activada = true;
            bajando = true;            
        }

    }

    public void Desactivar()
    {
        activada = false;
        bajando = false;            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!bajando || !other.CompareTag("Triangulardo")) return;

        var jefeHit = other.GetComponent<Triangulardo>();
        if (jefeHit != null && !jefeHit.IsStunned())
        {
            ControladorSonidos.instance.ReproducirSonido(ImpactoTrapecio,1f);
            jefeHit.Stun(5f);
        }
            
        StartCoroutine(RebotePlataforma());
    }
    
    private IEnumerator RebotePlataforma()
    {
        bajando = false;
        yield return new WaitForSeconds(1f);
        subiendo = true;
    }
}
