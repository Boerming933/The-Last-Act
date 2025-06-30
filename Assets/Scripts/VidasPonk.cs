using UnityEngine;

public class VidasPonk : MonoBehaviour
{
    public event System.Action<float> OnCambioDeFase; //
    public event System.Action<float> OnRecibirDaño; //
    public float vidaMaxima = 480f;
    public float vidaActual;
    public Triangulardo jefe;

    public delegate void CambioDeFaseDelegate(float vidaRestante);
    //public event CambioDeFaseDelegate OnCambioDeFase;
    public bool invulnerable = false;

    void Awake()
    {
        vidaActual = vidaMaxima;
        jefe = GetComponent<Triangulardo>();
    }

    public void RecibirDaño(float cantidad)
    {
        if (invulnerable || vidaActual <= 0) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Max(vidaActual, 0);
        Debug.Log("Ponk recibe daño. Vida restante: " + vidaActual);
        
        // jefe.VerificarCambioDeFase(vidaActual);
        OnRecibirDaño?.Invoke(vidaActual);  //
        OnCambioDeFase?.Invoke(vidaActual); //

        if (vidaActual <= 0)
        {
            Muerte();
        }
    }

    private void Muerte()
    {
        Debug.Log("Ponk ha sido derrotado!");
        // Animación de muerte, desactivación, etc
        jefe.Muerte(true);
    }
}
