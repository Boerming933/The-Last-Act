using UnityEngine;

public class IndicadoresPonk : MonoBehaviour
{
    [SerializeField] private VidasPonk vidasPonk;
    public Animator medidorAnimator;
    public Animator cartelMedioAnimator;
    public Animator cartelBajoAnimator;
    private enum FaseJefe { Fase1, Fase2, Fase3 }
    private FaseJefe faseActual = FaseJefe.Fase1;

    void Awake()
    {
        Debug.Log($"medidorAnimator = {medidorAnimator}", this);
        Debug.Log($"cartelMedioAnimator = {cartelMedioAnimator}", this);
        Debug.Log($"cartelBajoAnimator = {cartelBajoAnimator}", this);
        // Si no asignaste vidasPonk en el Inspector, lo busco dinámicamente
        if (vidasPonk == null)
            vidasPonk = FindAnyObjectByType<VidasPonk>();
        
        if (vidasPonk == null)
            Debug.LogError("IndicadoresPonk: no encontré VidasPonk en la escena.");
    }
    void OnEnable()
    {
        vidasPonk.OnCambioDeFase += ReaccionarAFase;
        vidasPonk.OnRecibirDaño  += ReaccionarADaño;
    }
    void OnDisable()
    {
        vidasPonk.OnCambioDeFase -= ReaccionarAFase;
        vidasPonk.OnRecibirDaño  -= ReaccionarADaño;
    }

    private void ReaccionarAFase(float vidaRestante)
    {
        Debug.Log("ReaccionarAFase ejecutado - Vida: " + vidaRestante);
        if (faseActual == FaseJefe.Fase1 && vidaRestante <= 280f)
        {
            faseActual = FaseJefe.Fase2;
            medidorAnimator.SetTrigger("Cambio");

            Debug.Log("→ Cambio a Fase 2");
        }
        else if (faseActual == FaseJefe.Fase2 && vidaRestante <= 80f)
        {
            faseActual = FaseJefe.Fase3;
            // Animación del cartel de abajo
            cartelMedioAnimator.SetTrigger("Cambio");

            Debug.Log("→ Cambio a Fase 3");
        }

        // Muerte
        if (vidaRestante <= 0f)
        {
            cartelBajoAnimator.SetTrigger("Muerte");
        }
    }

    private void ReaccionarADaño(float vidaActual)
    {
        Debug.Log("ReaccionarADaño ejecutado - Fase actual: " + faseActual);
        switch (faseActual)
        {
            case FaseJefe.Fase1:
                medidorAnimator.SetTrigger("Daño");
                break;
            case FaseJefe.Fase2:
                cartelMedioAnimator.SetTrigger("Daño");
                break;
            case FaseJefe.Fase3:
                cartelBajoAnimator.SetTrigger("Daño");
                break;
        }
    }
}
