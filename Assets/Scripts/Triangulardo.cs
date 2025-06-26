using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
<<<<<<< Updated upstream
using UnityEngine;

public class Triangulardo : MonoBehaviour
{   
    public float speed;
    private float Horizontal;
    private Rigidbody2D rb;
    public GameObject Ataque;
    private bool isAttacking;
=======
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
    private float xPosLastFrame;

    private Transform objetivoActual;
    public Transform jugador;
    private bool persiguiendoJugador = false;
    private bool estaDisparando = false;
    private bool vieneDePersecucion = false;
    private bool muerte = false;

    private Rigidbody2D rb;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;

    private Animator animator;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    private float tiempoUltimoAtaque;
>>>>>>> Stashed changes
    public bool Stunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
<<<<<<< Updated upstream
=======
        objetivoActual = puntoIzquierda;
        animator = GetComponent<Animator>();
        StartCoroutine(ComportamientoBoss());
        vidasPonk = GetComponent<VidasPonk>();
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
<<<<<<< Updated upstream
    {   
        if(isAttacking)
        {
            return;
        }
        if(Stunning)
        {
            return;
        }
        float tolerance = 0.01f;

        if(transform.position.x >= 7.7f - tolerance)
=======
    {
        if (Stunning)
        {
            return;
        }
        if (vidasPonk != null)
        {
            vidasPonk.OnCambioDeFase += VerificarCambioDeFase;
        }
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        if (faseActual != FaseJefe.Fase1 && faseActual != FaseJefe.Fase3 && !estaDisparando && !persiguiendoJugador && objetivoActual == null && !vieneDePersecucion)
>>>>>>> Stashed changes
        {
            Horizontal = -1 * speed;
            transform.localScale = new Vector3(-2.2654f,2.2654f,1f);
            StartCoroutine(Attack());
        }
<<<<<<< Updated upstream
        else if (transform.position.x <= -7.7f + tolerance)
        {
            Horizontal = 1 * speed;
            transform.localScale = new Vector3(2.2654f,2.2654f,1f);
            StartCoroutine(Attack());
=======

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
        if (Stunning)
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
>>>>>>> Stashed changes
        }
    }

    private IEnumerator Attack()
    {   
        isAttacking = true;
        Vector3 direction, origin;
        origin = new Vector3(transform.position.x, -3.47f, 1f);
        if(transform.localScale.x == 2.2654f)
        {
            direction = Vector3.right;
            origin = new Vector3(transform.position.x + 1f, -3.47f, 1f);
        }
<<<<<<< Updated upstream
=======
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
        animator.Play("Normal");

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
                    vieneDePersecucion = true; // para bloquear otros comportamientos
                    rb.linearVelocity = Vector2.zero;
                    animator.SetTrigger("Martillazo");
                    foreach (CuerdaScript cuerda in cuerdaScript)
                    {
                        cuerda.activada = true;
                    }

                    yield return new WaitForSeconds(1f); // tiempo de animación

                    vieneDePersecucion = false;

                    break;
            }
        }
    }

    void Patrullar()
    {
        persiguiendoJugador = false;

        if (Vector2.Distance(transform.position, puntoIzquierda.position) < Vector2.Distance(transform.position, puntoDerecha.position))
            objetivoActual = puntoDerecha;
>>>>>>> Stashed changes
        else
        {
            direction = Vector3.left;
            origin = new Vector3(transform.position.x - 1f, -3.47f, 1f);
        }
        yield return new WaitForSeconds(0.5f);
        GameObject BossAttack = Instantiate(Ataque, origin, quaternion.identity);
        BossAttack.GetComponent<AtaqueScript>().SetDirection(direction);
        yield return new WaitForSeconds(0.5f);
        rb.linearVelocity = new Vector2(Horizontal, rb.linearVelocityY);
        isAttacking = false;
        yield return new WaitForSeconds(5f);
        Destroy(BossAttack);
    }

    public IEnumerator Stun()
    {
        Stunning = true;
        yield return new WaitForSeconds(2f);
        Stunning = false;
    }
<<<<<<< Updated upstream
    
    private void FixedUpdate()
    {   
        if(isAttacking)
        {
            return;
        }
        if(Stunning)
        {
            return;
        }
        rb.linearVelocity = new Vector2(Horizontal, rb.linearVelocityY);
=======

    public bool IsStunned()
    {
        return Stunning;
>>>>>>> Stashed changes
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
