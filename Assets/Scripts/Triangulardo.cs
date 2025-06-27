using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Triangulardo : MonoBehaviour
{   
    public float speed;
    private float Horizontal;
    private Rigidbody2D rb;
    public GameObject Ataque;
    private bool isAttacking;
    public bool Stunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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
        {
            Horizontal = -1 * speed;
            transform.localScale = new Vector3(-2.2654f,2.2654f,1f);
            StartCoroutine(Attack());
        }
        else if (transform.position.x <= -7.7f + tolerance)
        {
            Horizontal = 1 * speed;
            transform.localScale = new Vector3(2.2654f,2.2654f,1f);
            StartCoroutine(Attack());
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
    }
}
