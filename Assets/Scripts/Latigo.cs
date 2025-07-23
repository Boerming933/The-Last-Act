using UnityEngine;

public class Latigo : MonoBehaviour
{    
    public VidasPonk ponk;
    public GameObject latigo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Triangulardo")
        {
            ponk.RecibirDaño(5);
            
            Debug.Log("💥 Hiciste clic. Daño aplicado: " + 5);            
        }
    }
}
