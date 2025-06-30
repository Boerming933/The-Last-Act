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

    public Transform puntoIzquierda;
    public Transform puntoDerecha;
    public float distanciaMinima = 0.1f;
    public float tiempoPersecucionMin = 2f;
    public float tiempoPersecucionMax = 5f;

    private Transform objetivoActual;
    public Transform jugador;
    public GameObject Hitbox;
    public Transform Referencia;
    public GameObject visual;
    private bool persiguiendoJugador = false;
    private bool estaDisparando = false;
    private bool vieneDePersecucion = false;
    private bool muerte = false;
    private bool Atacando = false;
    private bool HitboxActiva = false;
    private bool DañoPosible = true;


    private Rigidbody2D rb;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    [SerializeField] private VisualBridge puenteVisual;
    [SerializeField] private Animator animVisual;

    [SerializeField] private float tiempoEntreAtaques = 2f;
    private float tiempoUltimoAtaque;
    public bool Stunning;
    private VidasPonk vidasPonk;
    private enum FaseJefe { Fase1, Fase2, Fase3 }
    private FaseJefe faseActual = FaseJefe.Fase1;
    public CuerdaScript[] cuerdaScript;

    void Start()
    {
        puenteVisual.triangulardo = this;
        jugador = GameObject.FindGameObjectWithTag("Circulin").transform;
        rb = GetComponent<Rigidbody2D>();
        objetivoActual = puntoIzquierda;
        StartCoroutine(ComportamientoBoss());
        vidasPonk = GetComponent<VidasPonk>();
        if (vidasPonk != null) //
        {
            vidasPonk.OnCambioDeFase += VerificarCambioDeFase;
        }
    }

    void Update()
    {
        if (Stunning || muerte || Atacando)
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
        if (Stunning || Atacando)
        {
            return;
        }
        // Girar el jefe para mirar al jugador
        // if (jugador != null)
        // {
        //     float direccion = jugador.position.x - transform.position.x;
        //     float signo = direccion < 0 ? 1 : -1;
        //     transform.localScale = new Vector3(signo * 1f, 1f, 1f);
        // }

        if (jugador != null) //
        {
            Vector3 escala = transform.localScale;

            if (jugador.position.x < transform.position.x)
                escala.x = Mathf.Abs(escala.x); // mirar a la izquierda
            else
                escala.x = -Mathf.Abs(escala.x);  // mirar a la derecha

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

    void LateUpdate()
    {
        Hitbox.transform.position = Referencia.position;

        float distancia = Vector2.Distance(Referencia.position, Jugador.transform.position);
        float margenActivacion = 1.5f;

        if (distancia <= margenActivacion && Atacando)
        {
            ActivarHitbox();
        }
    }

    public void ActivarHitbox()
    {
        if (HitboxActiva || !DañoPosible) return;
        HitboxActiva = true;
        Hitbox.SetActive(true);
        StartCoroutine(DesactivarHitboxPronto());
    }

    public void ReinicioDaño()
    {
        Hitbox.GetComponent<HitboxMartillo>().ReiniciarDaño();
    }

    public void FrenarDaño()
    {
        DañoPosible = false;
    }

    IEnumerator DesactivarHitboxPronto()
    {
        yield return new WaitForSeconds(0.1f);
        Hitbox.SetActive(false);
        HitboxActiva = false;
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
            Vector3 direccion = transform.localScale.x > 0 ? Vector3.left : Vector3.right;

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
                    Debug.Log("Entró en el switch");
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
                    Debug.Log("Entró en el switch 3");
                    yield return StartCoroutine(PerseguirJugador());
                    vieneDePersecucion = true; //
                    rb.linearVelocity = Vector2.zero;
                    Atacando = true;
                    DañoPosible = true;
                    animVisual.SetTrigger("Martillazo");
                    foreach (CuerdaScript cuerda in cuerdaScript)
                    {
                        cuerda.activada = true;
                    }
                    Debug.Log("Deberia de estar frenado");
                    yield return new WaitForSeconds(10f); // 
                    vieneDePersecucion = false; //
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
        vieneDePersecucion = false;
    }

    public void FinMartillazoAnim()
    {
        if (!muerte && !Stunning)
        {
            vieneDePersecucion = false;
            animVisual.ResetTrigger("Martillazo");
            // StopAllCoroutines(); // por seguridad
            // StartCoroutine(ComportamientoBoss());
            Debug.Log("FSM reanudado después del Martillazo");
            Atacando = false;
        }
    }

    private void Golpe()
    {
        if (estaDisparando || muerte || Stunning) return;
        
        Atacando = true;
        DañoPosible = true;
        animVisual.SetTrigger("Martillazo");

        rb.linearVelocity = Vector2.zero;   
    }

    private void OnDrawGizmos() // Para ver el gizmo
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }

    public void Stun(float duration)
    {
        if (Stunning) return;        
        animVisual.speed = 0f;
        StopAllCoroutines();
        Stunning = true;
        animVisual.SetTrigger("Stunned");
        rb.linearVelocity = Vector2.zero;
        vidasPonk.invulnerable = false;
        StartCoroutine(RecoverFromStun(duration));
    }

    private IEnumerator RecoverFromStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        vidasPonk.invulnerable = true;
        Stunning = false;
        animVisual.speed = 1f;
        animVisual.ResetTrigger("Stunned");
        foreach (CuerdaScript cuerda in cuerdaScript)
        {
            cuerda.activada = false;
            cuerda.DesactivarPlataformas();
        }
        StartCoroutine(ComportamientoBoss());
        tiempoPersecucionMax = 10;
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
        StopAllCoroutines();
        StartCoroutine(RecoverFromStun(0f));
        muerte = Muerte;
        Stunning = Muerte;
        animVisual.SetBool("Muerte", true);
    }

    public void MuerteEstian(bool Muerte)
    {
        muerte = Muerte;
        Stunning = Muerte;
        animVisual.speed = 0f;
        StopAllCoroutines();
    }
}
