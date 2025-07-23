using System;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Triangulardo : MonoBehaviour
{
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public GameObject Jugador;
    public GameObject OndaPrefab;
    public Transform Origen;
    private int totalWavesToSpawn;
    private int wavesSpawned;
    private bool cycleDone;

    public Transform puntoIzquierda;
    public Transform puntoDerecha;
    public float distanciaMinima = 0.1f;
    public float tiempoPersecucionMin = 2f;
    public float tiempoPersecucionMax = 5f;
    int n = 1;

    private Transform objetivoActual;
    public Transform jugador;
    public GameObject Hitbox;
    public Transform Referencia;
    public GameObject visual;
    private bool persiguiendoJugador = false;
    private bool estaDisparando = false;
    private bool vieneDePersecucion = false;
    private bool Presentando = true;
    private bool muerte = false;
    private bool Atacando = false;
    private bool Patrullando;
    private bool HitboxActiva = false;
    private bool DañoPosible = true;


    private Rigidbody2D rb;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    [SerializeField] private VisualBridge puenteVisual;
    [SerializeField] private Animator animVisual;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private AudioClip FraseRepetitivo;
    [SerializeField] private AudioClip FraseMartillo;
    [SerializeField] private AudioClip FraseMitadVida;
    [SerializeField] private AudioClip Campana;
    [SerializeField] private AudioClip Martillo;
    [SerializeField] private AudioClip Ovacion;
    [SerializeField] private AudioClip AtaqueOndas;

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
        if (Stunning || muerte || Atacando || Presentando)
        {
            return;
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
        if (Stunning || Atacando || Presentando)
        {
            return;
        }

        if (jugador != null && !Patrullando) //
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

    public void TerminarPresentacion()
    {
        Presentando = false;
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
            ControladorSonidos.instance.ReproducirSonido(FraseMitadVida);
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
            // StopAllCoroutines();
            Patrullando = false;
            objetivoActual = null;
            persiguiendoJugador = true;
            // StartCoroutine(ComportamientoBoss());
        }
    }

    public void SpawnOnda()
    {
        ControladorSonidos.instance.ReproducirSonido(AtaqueOndas);
        Vector3 direccion = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
        Vector3 PositionOnda = new Vector3(Origen.position.x, Origen.position.y - 1f, Origen.position.z);
        GameObject Onda = Instantiate(OndaPrefab, PositionOnda, Quaternion.identity);
        Onda.GetComponent<AtaqueScript>().SetDirection(direccion);
        Destroy(Onda, 5f);
    }

    public void FinOnda()
    {
        cycleDone = true;
    }

    public IEnumerator WaveAttackRoutine(int count)
    {
        estaDisparando = true;
        Patrullando = false;
        for (int i = 0; i < count; i++)
        {
            cycleDone = false;

            animVisual.SetTrigger("Onda");

            yield return new WaitUntil(() => cycleDone);
            // opcional: si quieres un pequeño respiro
            // yield return new WaitForSeconds(0.1f);
            animVisual.ResetTrigger("Onda");
        }
        
        estaDisparando = false;
    }

    // private IEnumerator ShootOnda()
    // {
    //     ControladorSonidos.instance.ReproducirSonido(FraseRepetitivo);
    //     estaDisparando = true;
    //     rb.linearVelocity = Vector2.zero; // por si acaso

    //     /// yield return new WaitForSeconds(1f);
    //     Patrullando = false;
    //     animVisual.SetBool("Onda", true);

    //     int cantidadOndas = 3;
    //     float tiempoEntreOndas = 1f;

    //     for (int i = 0; i < cantidadOndas; i++)
    //     {

            // Vector3 direccion = transform.localScale.x > 0 ? Vector3.left : Vector3.right;

            // Vector3 PositionOnda = new Vector3(Origen.position.x, Origen.position.y - 1f, Origen.position.z);

            // GameObject Onda = Instantiate(OndaPrefab, PositionOnda, Quaternion.identity);
            // Onda.GetComponent<AtaqueScript>().SetDirection(direccion);
            // Destroy(Onda, 5f);
    //         if (i == 2)
    //         {
    //             yield return new WaitForSeconds(0.1f);
    //         }
    //         else
    //         {
    //             yield return new WaitForSeconds(tiempoEntreOndas);
    //         }

    //     }
    //     animVisual.SetBool("Onda", false);

    //     yield return new WaitForSeconds(0.5f); // espera extra después de disparar

    //     estaDisparando = false;
    // }

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
                        yield return StartCoroutine(WaveAttackRoutine(3));
                    }
                    else
                    {
                        if (!Patrullando)
                        {
                            yield return StartCoroutine(PerseguirJugador());
                        }
                    }
                    break;
                case FaseJefe.Fase3:
                    Debug.Log("Entró en el switch 3");
                    Patrullar();
                    yield return new WaitUntil(() => objetivoActual == null);
                    StartCoroutine(WaveAttackRoutine(3));
                    //En vez de "Martillazo debe ser "Ondas" o el nombre del trigger que pusiste e indicarle que sea en Loop
                    foreach (CuerdaScript cuerda in cuerdaScript)
                    {
                        cuerda.activada = true;
                    }
                    Debug.Log("Deberia de estar frenado");
                    yield return new WaitForSeconds(10f);
                    animVisual.ResetTrigger("Onda");
                    // Acá frenas la animacion de "Ondas" e inicias las de siempre
                    break;
            }
        }
    }

    void Patrullar()
    {
        persiguiendoJugador = false;
        Patrullando = true;

        if (Vector2.Distance(transform.position, puntoIzquierda.position) < Vector2.Distance(transform.position, puntoDerecha.position))
            objetivoActual = puntoDerecha;
        else
            objetivoActual = puntoIzquierda;
        Debug.Log("Nuevo objetivo de patrulla: " + objetivoActual.name);

        Vector3 scale = transform.localScale;
        scale.x = objetivoActual.position.x <= transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
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
    public void SonidoMartillo()
    {
        if (n == 1)
        {
            ControladorSonidos.instance.ReproducirSonido(FraseMartillo);
        }
        n++;
        ControladorSonidos.instance.ReproducirSonido(Martillo);
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
        animVisual.ResetTrigger("Onda");
        StopAllCoroutines();
        Stunning = true;
        animVisual.SetBool("Stun",true);
        rb.linearVelocity = Vector2.zero;
        vidasPonk.invulnerable = false;
        StartCoroutine(RecoverFromStun(duration));
    }

    private IEnumerator RecoverFromStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        vidasPonk.invulnerable = true;
        Stunning = false;
        animVisual.SetBool("Stun",false);
        foreach (CuerdaScript cuerda in cuerdaScript)
        {
            cuerda.activada = false;
            cuerda.DesactivarPlataformas();
        }
        ResetBossState();
        StartCoroutine(ComportamientoBoss());
        tiempoPersecucionMax = 10;
        tiempoPersecucionMin = 9;
    }
    private void ResetBossState()
    {
        Atacando            = false;
        persiguiendoJugador = false;
        Patrullando         = false;
        vieneDePersecucion  = false;
        objetivoActual      = null;
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
        ControladorSonidos.instance.ReproducirSonido(Campana);
        ControladorSonidos.instance.ReproducirSonido(Ovacion);
        muerte = Muerte;
        Stunning = Muerte;
        animVisual.SetTrigger("Muerte");
    }

    public void MuerteEstian(bool Muerte)
    {
        muerte = Muerte;
        Stunning = Muerte;
        animVisual.speed = 0f;
        StopAllCoroutines();
    }
}