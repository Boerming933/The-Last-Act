using UnityEngine;

public class AtaqueScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 Direction;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Direction * speed;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }
}
