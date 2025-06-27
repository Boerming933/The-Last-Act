using UnityEngine;

public class AtaqueScript : MonoBehaviour
{
    public float Speed;
    private Vector2 Direction;
    private Rigidbody2D rigi2D;


    void Start()
    {
        rigi2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rigi2D.linearVelocity = Direction * Speed;
    }

    public void SetDirection(Vector2 direction)
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
            vidasPj.Hit(0.5f);
        }
    }
}
