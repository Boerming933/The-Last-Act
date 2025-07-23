using UnityEngine;

public class VisualBridge : MonoBehaviour
{
    public Triangulardo triangulardo;
    public Circulin Estian;

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

    public void Terminar_Presentacion()
    {
        triangulardo?.TerminarPresentacion();
        Estian.FinPresentacion();
    }

    public void Evento_SpawnOnda()
    {
        triangulardo.SpawnOnda();
        Debug.Log("[VisualBridge] Event SpawnWave recibido");
    }

    public void Evento_FinOnda()
    {
        triangulardo.FinOnda();
        Debug.Log("[VisualBridge] Event EndWaveAttack recibido");
    }

}
