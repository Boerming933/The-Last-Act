using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Circulin : MonoBehaviour
{
    
    //public Animator anim;
    public float meleeSpeed;
    public float damage;
    float timeUntilMelee;

    [SerializeField] private Animator Estian;
    [SerializeField] private Hook hook;

    public Transform Mira;
    public Transform Ponk;
    public GameObject Brazo;
    public GameObject Latigo;
    public VidasPj vidasPj;
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower;
    private float dashingTime = 0.1f;
    private float dashingWait = 0.1f;
    private float dashingCooldown = 2f;
    public float JumpForce, speed;
    private bool InJump = false;
    private bool canLand = true;

    private Rigidbody2D Rigidbody2D;
    private TrailRenderer trailRenderer;
    private float Horizontal;
    private bool Grounded;
    public bool InJalon;
    private bool GuardadoMuerte = false;
    private bool Stun = true;
    private bool muerte = false;
    private bool atacando = false;
    private bool Enganchado = false;
    private SpriteRenderer mySpriteRenderer;
    public SpriteRenderer BrazoSprite;

    //VARIABLES PAL CARGADO  -Juan
    public CargaAtkEspecial CargaAtkEspecial;
    public bool puedeCargado = false;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private Vector2 frozenPosition;
    public GameObject proyectilPrefab;
    public float velocidadProyectil = 10f;
    private Vector3 mouseWorldPos;
    private Vector2 direccion;
    Camera cam;

    //AUDIOS CARGADOS -Juan
    [SerializeField] private AudioClip ataque;
    [SerializeField] private AudioClip correr;
    [SerializeField] private AudioClip ataqueCargado;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip salto;
    [SerializeField] private AudioClip landeo;
    [SerializeField] private AudioClip Abucheo;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        cam = Camera.main;  //<---- ESTO PAL CARGADO
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {    
        if (muerte || isDashing || InJalon || Enganchado)
        {
            return;
        }
        //ATAQUE CARGADO  -Juan
        if (Input.GetMouseButtonDown(1) && puedeCargado && !isFrozen)
        {
            //pt1 - Barra
            atacando = true;
            Brazo.SetActive(false);
            Estian.SetTrigger("Charge");
            puedeCargado = false;
            CargaAtkEspecial.cargas = 0;
            Vector3 escala = CargaAtkEspecial.barraCarga.localScale;
            escala.y -= 54f;
            CargaAtkEspecial.barraCarga.localScale = escala;

            //pt2 - Ataque
            StartCoroutine(DispararConRetraso(1f));

            //pt3 - Cancelar Movimiento
            isFrozen = true;
            freezeTimer = 2f;
            frozenPosition = Rigidbody2D.position;
            Rigidbody2D.linearVelocity = Vector2.zero;
            Rigidbody2D.gravityScale = 0f;
        }

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            Rigidbody2D.position = frozenPosition;
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
                Rigidbody2D.gravityScale = 3f;
            }
            return;
        }

        if (timeUntilMelee <= 0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ControladorSonidos.instance.ReproducirSonido(ataque);
                //anim.SetTrigger("Attack");
                atacando = true;
                StartCoroutine(FrenarLatigo());
                Latigo.SetActive(true);
                timeUntilMelee = meleeSpeed;
            }
        }
        else
        {
            timeUntilMelee -= Time.deltaTime;
        }

        Horizontal = Input.GetAxisRaw("Horizontal") * speed;
        
        if(!InJump) Estian.SetBool("Corriendo",Horizontal != 0.0f);

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - 0.43f, transform.position.z);
        Debug.DrawRay(origin, Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, 1.1f);

        bool wasGrounded = Grounded;

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            Grounded = true;
            if (!atacando)
            {
                Estian.SetBool("Quieto",true);                
            }

            if (!wasGrounded)
            {
                ControladorSonidos.instance.ReproducirSonido(landeo);
            }
        }
        else Grounded = false;

        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            Jump();
            ControladorSonidos.instance.ReproducirSonido(salto);
            canLand = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Jalon());
        }

        if (!atacando)
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)
            );

            bool RotarX = mouseWorldPos.x < transform.position.x;
            Vector3 scale = transform.localScale;
            if (RotarX)
            {
                mySpriteRenderer.flipX = true;
                BrazoSprite.flipY = true;
            }
            else
            {
                mySpriteRenderer.flipX = false;
                BrazoSprite.flipY = false;
            }
            transform.localScale = scale;

            Vector2 direccion = mouseWorldPos - Brazo.transform.position;
            float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        
            bool mirandoIzquierda = transform.localScale.x < 0;
            if (mirandoIzquierda)
            {
                angle += 180f;
            }

            Brazo.transform.rotation = Quaternion.Euler(0f, 0f, angle);            
        }

        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            ControladorSonidos.instance.ReproducirSonido(dash);
            Estian.SetBool("Dash", true);
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Grounded)
            {
                ControladorSonidos.instance.ReproducirSonido(correr);
            }
        }
        else
        {
            ControladorSonidos.instance.ReproducirSonido(null);
        }
    }

    IEnumerator FrenarLatigo()
    {
        yield return new WaitForSeconds(0.5f);
        Latigo.SetActive(false);
        atacando = false;
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
        InJump = true;
        Estian.SetBool("Quieto",false);
        StartCoroutine(TerminarSalto());
        Estian.SetTrigger("Salto");
    }

    private IEnumerator TerminarSalto()
    {
        yield return new WaitForSeconds(0.4f);
        InJump = false;
    }

    private IEnumerator Dash()
    {
        Brazo.SetActive(false);
        vidasPj.InDash(true);
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;
        float originalGravity = Rigidbody2D.gravityScale;
        Rigidbody2D.gravityScale = 0f;
        if (Horizontal >= 0f)
        {
            Rigidbody2D.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        }
        if (Horizontal < 0f)
        {
            Rigidbody2D.linearVelocity = new Vector2(transform.localScale.x * -1f * dashingPower, 0f);
            mySpriteRenderer.flipX = true;
        }
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        Rigidbody2D.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashingWait);
        Estian.SetBool("Dash", false);  
        isDashing = false;
        vidasPj.InDash(false); 
        yield return new WaitForSeconds(0.3f); 
        if (!muerte) Brazo.SetActive(true);
        yield return new WaitForSeconds(dashingCooldown - 0.3f);    
        canDash = true;
    }

    public IEnumerator Jalon()
    {
        InJalon = true;
        yield return new WaitForSeconds(3f);
        InJalon = false;
    }

    public void Trapecio(bool n)
    {
        Enganchado = n;
        if (!Enganchado)
        {
            Brazo.SetActive(true);
            StartCoroutine(RecGancho());
        }
        else Brazo.SetActive(false);
    }

    private IEnumerator RecGancho()
    {
        yield return new WaitForSeconds(0.6f);
        Estian.SetBool("CaidaG", false);
    }

    private void FixedUpdate()
    {
        if (muerte || isDashing || InJalon)
        {
            return;
        }
        if (Stun == true)
        {
            Rigidbody2D.linearVelocity = new Vector2(Horizontal * speed, Rigidbody2D.linearVelocity.y);

        }
    }

    public void Empuje(float fuerzaHorizontal, float fuerzaVertical, Vector3 atacantePos)
    {
        StartCoroutine(DesactivarMov());

        float direccion = (transform.position.x - atacantePos.x) >= 0 ? 1 : -1;
        Vector2 empuje = new Vector2(direccion * Mathf.Abs(fuerzaHorizontal), Mathf.Abs(fuerzaVertical));

        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.AddForce(empuje, ForceMode2D.Impulse);
    }

    IEnumerator DesactivarMov()
    {
        Stun = false;
        yield return new WaitForSeconds(0.5f);
        Stun = true;
    }

    public void Muerte(bool Muerte)
    {
        StopAllCoroutines();
        hook.Muerte(true);
        Brazo.SetActive(false);
        GuardadoMuerte = Muerte;
        muerte = GuardadoMuerte;
        ControladorSonidos.instance.ReproducirSonido(Abucheo);
        Estian.SetTrigger("Muerte");
    }

    public void MuertePonk(bool PonkMuerte)
    {
        muerte = PonkMuerte;
    }

    IEnumerator DispararConRetraso(float delay)          //PAL CARGADO -Juan
    {
        mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        direccion = (mouseWorldPos - transform.position).normalized;

        yield return new WaitForSeconds(delay);
        for (int n = 0; n <= 9; n++)
        {
            yield return new WaitForSeconds(0.1f);
            ControladorSonidos.instance.ReproducirSonido(ataqueCargado);
            DispararProyectil();
        }
        Estian.SetBool("Cargado", true);
        yield return new WaitForSeconds(1f);
        Estian.SetBool("Cargado", false);
        Brazo.SetActive(true);
        atacando = false;
    }

    void DispararProyectil()             //PAL CARGADO -Juan
    {
        Vector3 Cargado = new Vector3(Ponk.position.x,Random.Range(Ponk.position.y-1.5f,Ponk.position.y+1.5f),Ponk.position.z-2f);
        GameObject proyectil = Instantiate(proyectilPrefab, Cargado, Quaternion.identity);
        Destroy(proyectil, 0.2f);

        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        proyectil.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}