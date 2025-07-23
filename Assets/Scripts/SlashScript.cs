using UnityEngine;

public class SlashScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.CompareTag("Triangulardo"))
        {
            VidasPonk vida = other.GetComponent<VidasPonk>();
            if (vida != null)
            {
                vida.RecibirDaño(3f);
                Debug.Log("Golpe Recibido, el daño fue de " + 3);
            }
        }
    }    
}
