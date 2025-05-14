using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Triangulardo : MonoBehaviour
{   
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public GameObject Jugador;
    public GameObject OndaPrefab;
    public Transform Origen;

    public Transform puntoIzquierda;
    public Transform puntoDerecha;
    public float velocidadMovimiento = 5f;
    public float distanciaMinima = 0.1f;
    public float tiempoPersecucionMin = 2f;
    public float tiempoPersecucionMax = 5f;

    private Transform objetivoActual;
    public Transform jugador;
    private bool persiguiendoJugador = false;
    private bool estaDisparando = false;
    private bool vieneDePersecucion = false;

    private Rigidbody2D rb;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    
    private Animator animator;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    private float tiempoUltimoAtaque;
    public bool Stunning;
    public float vida = 10f;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;
        rb = GetComponent<Rigidbody2D>();
        objetivoActual = puntoIzquierda;
        animator = GetComponent<Animator>();
        StartCoroutine(ComportamientoBoss());

    }

    void Update()
    {
        if(Stunning)
        {
            return;
        }
        
        if (!estaDisparando && !persiguiendoJugador && objetivoActual == null && !vieneDePersecucion)        
        {
            StartCoroutine(ShootOnda());
        }

         if (persiguiendoJugador && !estaDisparando)
        {
            Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

            
            foreach (Collider2D colisionador in objetos)
            {
                if (colisionador.CompareTag("Circulin") && Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
                {
                    tiempoUltimoAtaque = Time.time;
                    Golpe();
                    break; // Sale del foreach una vez que golpea al jugador
                }
            }
        }

    }

    void FixedUpdate()
    {
        if(Stunning)
        {
            return;
        }
        // Girar el jefe para mirar al jugador
        if (jugador != null)
        {
            Vector3 escala = transform.localScale;

            if (jugador.position.x < transform.position.x)
                escala.x = -Mathf.Abs(escala.x); // mirar a la izquierda
            else
                escala.x = Mathf.Abs(escala.x);  // mirar a la derecha

            transform.localScale = escala;
        }

        float velocidadActual = persiguiendoJugador ? velocidadPersecucion : velocidadPatrulla;

        if (persiguiendoJugador && jugador != null)
        {
            float distanciaDeseada = 2f; 
            float distanciaAlJugador = Vector2.Distance(rb.position, jugador.position);

            if (distanciaAlJugador > distanciaDeseada)
            {
                Vector2 direccion = (jugador.position - transform.position).normalized;
                direccion.y = 0;
                Vector2 destino = rb.position + direccion * velocidadPersecucion * Time.fixedDeltaTime;
                rb.MovePosition(destino);
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // se detiene si está a distancia correcta
            }
        }
        else if (objetivoActual != null && !estaDisparando)        
        {
            Vector2 destino = new Vector2(objetivoActual.position.x, rb.position.y);
            Vector2 nuevaPos = Vector2.MoveTowards(rb.position, destino, velocidadActual * Time.fixedDeltaTime);
            rb.MovePosition(nuevaPos);

            if (Vector2.Distance(rb.position, destino) < distanciaMinima)
            {
                objetivoActual = null;
            }
        }
    }

    private IEnumerator ShootOnda()
    {
        estaDisparando = true;
        rb.linearVelocity = Vector2.zero; // por si acaso

        yield return new WaitForSeconds(1.2f);

        int cantidadOndas = 3;
        float tiempoEntreOndas = 0.85f;

        for (int i = 0; i < cantidadOndas; i++)
        {
            Vector3 direccion = transform.localScale.x > 0 ? Vector3.right : Vector3.left;

            GameObject Onda = Instantiate(OndaPrefab, Origen.position, Quaternion.identity);
            Onda.GetComponent<AtaqueScript>().SetDirection(direccion);
            Destroy(Onda, 5f);

            yield return new WaitForSeconds(tiempoEntreOndas);
        }

        yield return new WaitForSeconds(2f); // espera extra después de disparar

        estaDisparando = false;

    }

    IEnumerator ComportamientoBoss()
    {
        animator.Play("Normal");
        while (true)
        {
            int eleccion = UnityEngine.Random.Range(0,2); // 0 = patrullar, 1 = perseguir

            if (eleccion == 0)
            {
                // Modo Patrullaje
                Patrullar();
                yield return new WaitUntil(() => objetivoActual == null);
                yield return new WaitForSeconds(4);
            }
            else
            {
                // Modo Persecución
                yield return StartCoroutine(PerseguirJugador());
            }
        }
    }

    void Patrullar()
    {
        persiguiendoJugador = false;

        if (Vector2.Distance(transform.position, puntoIzquierda.position) < Vector2.Distance(transform.position, puntoDerecha.position))
            objetivoActual = puntoDerecha;
        else
            objetivoActual = puntoIzquierda;
        Debug.Log("Nuevo objetivo de patrulla: " + objetivoActual.name);
    }

    IEnumerator PerseguirJugador()
    {
        persiguiendoJugador = true;
        float tiempoPersecucion = UnityEngine.Random.Range(tiempoPersecucionMin, tiempoPersecucionMax);
        float contador = 0f;

        while (contador < tiempoPersecucion)
        {
            if (jugador == null) break;
            contador += Time.deltaTime;
            yield return null;
        }

        persiguiendoJugador = false;
        vieneDePersecucion = true;
        rb.linearVelocity = Vector2.zero;
        objetivoActual = null;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
        vieneDePersecucion = false;
    }

    private void Golpe()
    {
        animator.SetTrigger("Martillazo");

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Circulin"))
            {
                colisionador.transform.GetComponent<VidasPj>().Hit();
            }
        }
    }

    private void OnDrawGizmos() // Para ver el gizmo
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }

    public void Stun(float duration)
    {
        if (Stunning)
        {
            return;
        }
        StopAllCoroutines();
        Stunning = true;
        animator.SetTrigger("Stunned");
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(RecoverFromStun(duration));
    }

    IEnumerator RecoverFromStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        Stunning = false;
        StartCoroutine(ComportamientoBoss());
    }

    public bool IsStunned()
    {
        return Stunning;
    }
}
