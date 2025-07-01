using UnityEngine;

public class AtaqueScript : MonoBehaviour
{
    public float Speed;
    private Vector3 Direction;
    private Rigidbody2D rigi2D;


    void Start()
    {
        rigi2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rigi2D.linearVelocity = Direction * Speed;
        if (Direction == Vector3.right)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Golpeo");
        
        Circulin Jugador = collision.GetComponent<Circulin>();
        VidasPj vidasPj = collision.GetComponent<VidasPj>();
        if (Jugador != null)
        {
            Jugador.Empuje(12f, 36f, transform.position);
            vidasPj.Hit(1f);
        }
    }
}