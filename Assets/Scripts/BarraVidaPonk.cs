using UnityEngine;
using UnityEngine.UI;

public class BarraVidaPonk : MonoBehaviour
{
    public Image image;
    public VidasPonk vidasPonk;
    private float VidaMaxima;

    void OnRecibirDaño(float vidaRestante)
    {
        image.fillAmount = vidaRestante / VidaMaxima;
    }
    void Start()
    {
        VidaMaxima = vidasPonk.vidaMaxima;
        image.fillAmount = 1f;        // barra llena al inicio
        // Te suscribís al evento de daño para actualizar fresh
        vidasPonk.OnRecibirDaño += OnRecibirDaño;
    }
    void OnDestroy()
    {
        vidasPonk.OnRecibirDaño -= OnRecibirDaño;
    }
}
