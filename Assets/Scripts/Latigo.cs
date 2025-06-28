using UnityEngine;

public class Latigo : MonoBehaviour
{
    
    public VidasPonk ponk;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Triangulardo")
        {
            ponk.RecibirDaño(40);
            Debug.Log("💥 Hiciste clic. Daño aplicado: " + 40);            
        }
    }
}
