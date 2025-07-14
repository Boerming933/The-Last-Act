using UnityEngine;

public class VisualBridge : MonoBehaviour
{
    public Triangulardo triangulardo;

    public void Evento_FinMartillazoAnim()
    {
        triangulardo?.FinMartillazoAnim();
    }

    public void Evento_ReiniciarDaño()
    {
        triangulardo?.ReinicioDaño();
    }

    public void Evento_FrenarDaño()
    {
        triangulardo?.FrenarDaño();
    }

    public void Evento_SonidoMartillo()
    {
        triangulardo?.SonidoMartillo();
    }
}
