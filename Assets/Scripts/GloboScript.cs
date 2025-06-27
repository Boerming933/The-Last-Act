using System.Collections;
using UnityEngine;

public class GloboScript : MonoBehaviour
{
    public enum TipoGlobo { Rojo, Verde, Amarillo }
    public TipoGlobo tipoGlobo;

    public float radioExplosion = 1.5f;
    private Vector3 posicionInicial;
    public float tiempoDeVida = 15f;
    private float tiempoActual = 0f;
    private float tiempo;
    private bool bajaEnOnda;
    public float velocidadHorizontal = 2f;
    public float amplitudVertical = 1f;
    public float frecuenciaVertical = 0.5f;
    private float direccion;
    private bool yaHizoDaño = false;
    private bool explotando = false;
    private Animator animator;

    void Start()
    {
        posicionInicial = transform.position;
        tiempo = Random.Range(0f, 2f * Mathf.PI);
        bajaEnOnda = Random.value < 0.4f;
        direccion = transform.position.x < 0 ? 1f : -1f;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        tiempo += Time.deltaTime * frecuenciaVertical;
        tiempoActual += Time.deltaTime;

        float desplazamientoX = direccion * velocidadHorizontal * Time.deltaTime;
        float desplazamientoY = bajaEnOnda ? Mathf.Sin(tiempo) * amplitudVertical : 0f;

        transform.position = new Vector3(
            transform.position.x + desplazamientoX,
            posicionInicial.y + desplazamientoY,
            transform.position.z
        );

        if (tiempoActual >= tiempoDeVida)
        {
            Destroy(gameObject);
        }
    }


    void Explotar()
    {
        if (explotando) return;
        explotando = true;
        if (yaHizoDaño) return;
        Collider2D[] afectados = Physics2D.OverlapCircleAll(transform.position, radioExplosion);
        foreach (Collider2D col in afectados)
        {
            VidasPj vida = col.GetComponent<VidasPj>();
            if (vida != null)
            {
                vida.Hit(0.5f);
                yaHizoDaño = true;
            }
        }
        switch (tipoGlobo)
        {
            case TipoGlobo.Rojo:
                animator.SetTrigger("Rojo");
                break;
            case TipoGlobo.Verde:
                animator.SetTrigger("Verde");
                break;
            case TipoGlobo.Amarillo:
                animator.SetTrigger("Amarillo");
                break;
        }
        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Latigo") || other.CompareTag("Circulin"))
        {
            Explotar();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}
