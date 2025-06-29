using System.Collections;
using UnityEngine;

public class HitboxMartillo : MonoBehaviour
{
    public GameObject Estian;
    public float Daño = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Daño != 1)
        {
            if (other.CompareTag("Circulin"))
            {
                other.GetComponent<VidasPj>()?.Hit(2f);
                Daño = 1;
            }
        }
    }
    public void ReiniciarDaño()
    {
        Daño = 0;
    }

}
