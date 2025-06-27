using System;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Triangulardo : MonoBehaviour
{
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public GameObject Jugador;
    public GameObject OndaPrefab;
    public Transform Origen;
    private SpriteRenderer mySpriteRenderer;

    public Transform puntoIzquierda;
    public Transform puntoDerecha;
    public float distanciaMinima = 0.1f;
    public float tiempoPersecucionMin = 2f;
    public float tiempoPersecucionMax = 5f;

    private Transform objetivoActual;
    public Transform jugador;
    private bool persiguiendoJugador = false;
    private bool estaDisparando = false;
    private bool vieneDePersecucion = false;
    private bool muerte = false;
    private bool Atacando = false;


    private Rigidbody2D rb;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;

    private Animator animator;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    private float tiempoUltimoAtaque;
    public bool Stunning;
    private VidasPonk vidasPonk;
    private enum FaseJefe { Fase1, Fase2, Fase3 }
    private FaseJefe faseActual = FaseJefe.Fase1;
    public CuerdaScript[] cuerdaScript;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;        
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        objetivoActual = puntoIzquierda;
        animator = GetComponent<Animator>();
        StartCoroutine(ComportamientoBoss());
        vidasPonk = GetComponent<VidasPonk>();
    }

    void Update()
    {
        if (Stunning || muerte )//|| Atacando)
        {
            return;
        }
        //if (vidasPonk != null)
        //{

        //    vidasPonk.OnCambioDeFase += VerificarCambioDeFase;
        //}

        if (faseActual != FaseJefe.Fase1 && faseActual != FaseJefe.Fase3 && !estaDisparando && !persiguiendoJugador && objetivoActual == null && !vieneDePersecucion)
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
        if (Stunning )//|| Atacando)
        {
            return;
        }
        // Girar el jefe para mirar al jugador
        if (jugador != null)
        {
            if (jugador.position.x >= transform.position.x)
            {
                mySpriteRenderer.flipX = true;
            }
            else
            {
                mySpriteRenderer.flipX = false;
            }
        }

        float velocidadActual = persiguiendoJugador ? velocidadPersecucion : velocidadPatrulla;

        if (persiguiendoJugador && jugador != null)
        {
            float distanciaDeseada = 4f;
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

    public void VerificarCambioDeFase(float vidaRestante)
    {
        if (faseActual == FaseJefe.Fase1 && vidaRestante <= 280f)
        {
            faseActual = FaseJefe.Fase2;
            Debug.Log("Fase 2 activada: ataques combinados");
        }
        if (faseActual == FaseJefe.Fase2 && vidaRestante <= 80f)
        {
            faseActual = FaseJefe.Fase3;
            Debug.Log("RAGE ACTIVADO... HUYE!!");
            tiempoPersecucionMax = 16;
            tiempoPersecucionMin = 15;
            vidasPonk.invulnerable = true;
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
        while (!muerte)
        {
            switch (faseActual)
            {
                case FaseJefe.Fase1:
                    yield return StartCoroutine(PerseguirJugador());
                    break;
                case FaseJefe.Fase2:
                    int eleccion = UnityEngine.Random.Range(0, 2); // 0 = patrulla, 1 = persecución

                    if (eleccion == 0)
                    {
                        Patrullar();
                        yield return new WaitUntil(() => objetivoActual == null);
                        yield return new WaitForSeconds(4f);
                    }
                    else
                    {
                        yield return StartCoroutine(PerseguirJugador());
                    }
                    break;
                case FaseJefe.Fase3:
                    yield return StartCoroutine(PerseguirJugador());
                    rb.linearVelocity = Vector2.zero;
                    //Atacando = true;
                    animator.SetTrigger("Martillazo");
                    foreach (CuerdaScript cuerda in cuerdaScript)
                    {
                        cuerda.activada = true;
                    }
                    break;
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
    }

    public void FinMartillazoAnim()
    {
        if (!muerte && !Stunning)
        {
            vieneDePersecucion = false;

            animator.ResetTrigger("Martillazo");
            StopAllCoroutines(); // por seguridad
            StartCoroutine(ComportamientoBoss());
            Debug.Log("FSM reanudado después del Martillazo");
           // Atacando = false;
        }
    }

    public void TestEvent()
    {
        Debug.Log("🎯 ¡Evento recibido correctamente!");
    }

    private void Golpe()
    {
        if (estaDisparando || muerte || Stunning) return;
        
        //Atacando = true;
        animator.SetTrigger("Martillazo");

        rb.linearVelocity = Vector2.zero;
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Circulin"))
            {
                colisionador.transform.GetComponent<VidasPj>().Hit(1f);
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
        if (Stunning) return;
        StopAllCoroutines();
        Stunning = true;
        animator.SetTrigger("Stunned");
        rb.linearVelocity = Vector2.zero;
        vidasPonk.invulnerable = false;
        StartCoroutine(RecoverFromStun(duration));
    }

    private IEnumerator RecoverFromStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        vidasPonk.invulnerable = true;
        Stunning = false;
        StartCoroutine(ComportamientoBoss());
        tiempoPersecucionMin = 10;
        tiempoPersecucionMin = 9;
    }

    public bool IsStunned()
    {
        return Stunning;
    }

    public void Comienzo()
    {
        Debug.Log("Comportamiento del jefe iniciado manualmente");
        StartCoroutine(ComportamientoBoss());
    }

    public void Muerte(bool Muerte)
    {
        muerte = Muerte;
        Stunning = Muerte;
        StopAllCoroutines();
        animator.SetBool("Muerte", true);
    }

    public void MuerteEstian(bool Muerte)
    {
        muerte = Muerte;
        Stunning = Muerte;
        StopAllCoroutines();
    }
}
