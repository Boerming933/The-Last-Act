using UnityEngine;

public class PelotaScript : MonoBehaviour
{
    public enum TipoPelota { Amarilla, Roja, Verde }
    public TipoPelota tipoPelota;
    public float horizontalSpeed;
    public float verticalBounceForce;
    public float tiempoDeVida = 10f;
    public LayerMask groundLayer;
    private bool yaHizoDaño = false;
    private Rigidbody2D rb;
    private float tiempoActual = 0f;
    private bool explotando = false;
    private Animator animator;

    void Update()
    {
        tiempoActual += Time.deltaTime;

        if (tiempoActual >= tiempoDeVida)
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Rebote controlado solo contra el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Rebote vertical
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalBounceForce);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Latigo"))
        {
            Explotar();
        }
        if (other.CompareTag("Circulin"))
        {
            if (yaHizoDaño) return;

            VidasPj vida = other.GetComponent<VidasPj>();
            if (vida != null)
            {
                vida.Hit(0.5f);
                yaHizoDaño = true;
            }
        }

    }
    public void SetDirection(int direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(horizontalSpeed * direction, verticalBounceForce - 5);
    }
    
    void Explotar()
    {
        if (explotando) return;
        explotando = true;
        switch (tipoPelota)
        {
            case TipoPelota.Amarilla:
                animator.SetTrigger("Amarilla");
                break;
            case TipoPelota.Roja:
                animator.SetTrigger("Roja");
                break;
            case TipoPelota.Verde:
                animator.SetTrigger("Verde");
                break;
        }
        Destroy(gameObject, 0.6f);
    }
}
