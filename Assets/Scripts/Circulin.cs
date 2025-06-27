using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Circulin : MonoBehaviour
{
    public Animator anim;
    public float meleeSpeed;
    public float damage;
    float timeUntilMelee;

    public Transform Mira;
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower;
    private float dashingTime = 0.1f;
    private float dashingWait = 0.1f;
    private float dashingCooldown = 2f;
    public float JumpForce, speed;

    private Rigidbody2D Rigidbody2D;
    private TrailRenderer trailRenderer;
    private float Horizontal;
    private bool Grounded;
    public bool InJalon;
    private bool Stun = true;
    private bool muerte = false;
    public VidasPonk ponk;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if (muerte)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0)) // 0 = botón izquierdo del mouse
        {
            if (ponk != null)
            {
                ponk.RecibirDaño(40);
                Debug.Log("💥 Hiciste clic. Daño aplicado: " + 40);
            }
        }
        if (isDashing)
        {
            return;
        }
        if (InJalon)
        {
            return;
        }

        if (timeUntilMelee <= 0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Attack");
                timeUntilMelee = meleeSpeed;
            }
        }
        else
        {
            timeUntilMelee -= Time.deltaTime;
        }

        Horizontal = Input.GetAxisRaw("Horizontal") * speed;

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - 0.43f, transform.position.z);
        Debug.DrawRay(origin, Vector3.down * 1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, 1f);
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            Grounded = true;
        }
        else Grounded = false;

        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Jalon());
        }
        Mira.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        float angle = Mathf.Atan2(Mira.position.y - transform.position.y, Mira.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private IEnumerator Dash()
    {
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
        }
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        Rigidbody2D.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashingWait);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public IEnumerator Jalon()
    {
        InJalon = true;
        yield return new WaitForSeconds(3f);
        InJalon = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Triangulardo")
        {
            //other.GetComponent<Triangulardo>().TakeDamage();
            Debug.Log("Enemy Hit");
        }
    }

    private void FixedUpdate()
    {
        if (muerte)
        {
            return;
        }
        if (isDashing)
        {
            return;
        }
        if (InJalon)
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
        muerte = Muerte;
        StopAllCoroutines();
    }
}
