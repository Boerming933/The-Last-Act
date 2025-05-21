using UnityEngine;

public class PelotaScript : MonoBehaviour
{
    public float horizontalSpeed;
    public float verticalBounceForce;
    public float tiempoDeVida = 10f;
    public LayerMask groundLayer;
    private bool yaHizoDaño = false;
    private Rigidbody2D rb;
    private float tiempoActual = 0f;

    void Update()
    {
        tiempoActual += Time.deltaTime;

        if (tiempoActual >= tiempoDeVida)
        {
            Destroy(gameObject);
        }

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
            // Reacciona a la colisión con el látigo
            Destroy(gameObject); // o aplicar daño, efectos, etc.
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
}
