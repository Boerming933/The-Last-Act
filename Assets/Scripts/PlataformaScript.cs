using UnityEngine;

public class PlataformaScript : MonoBehaviour
{
    public GameObject jefe;
    public float velocidadCaida = 5f;
    public float distanciaCaida = 2f;
    private Vector3 posicionInicial;
    private bool bajando = false;
    private bool subiendo = false;
    private float alturaMinima;
    private BoxCollider2D boxCollider;

    void Start()
    {
        posicionInicial = transform.position;
        alturaMinima = posicionInicial.y - distanciaCaida;
        jefe = GameObject.FindGameObjectWithTag("Triangulardo");
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
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
            }
        }
    }

    public void Activar()
    {
        bajando = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Triangulardo"))
        {
            if (jefe == null)
            {
                jefe = GameObject.FindGameObjectWithTag("Triangulardo");
            }
            Triangulardo jefeScript = jefe.GetComponent<Triangulardo>();

            if (!jefeScript.IsStunned()) // usamos un getter para seguridad
            {
                jefeScript.Stun(10f);
            }

        }
    }
}
