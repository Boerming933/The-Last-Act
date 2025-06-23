using UnityEngine;

public class LevelPhaseManager : MonoBehaviour
{
    public CañonScript[] cañones;
    public SpawnGlobos spawnGlobos;
    public GameObject jefePonk;
    private float tiempoFase;
    private int faseActual = 0;
    private VidasPonk vidasPonk;
    void Start()
    {
        jefePonk.SetActive(false);
        vidasPonk = jefePonk.GetComponent<VidasPonk>();
        ActivarFasePelotas();
        vidasPonk.invulnerable = true;
    }

    void Update()
    {
        tiempoFase += Time.deltaTime;

        switch (faseActual)
        {
            case 0: // Fase preliminar: solo pelotas
                if (tiempoFase >= 60f)
                {
                    ActivarFaseGlobos();
                }
                break;

            case 1: // Fase preliminar: solo globos
                if (tiempoFase >= 60f)
                {
                    ActivarFaseConPonk();
                }
                break;

            case 2: // Fase 1 de Ponk: pelotas + jefe
                if (vidasPonk != null && vidasPonk.vidaActual <= 280f)
                {
                    ActivarFase2Ponk();
                }
                break;

            case 3: // Fase 2 de Ponk: globos + jefe
                if (vidasPonk != null && vidasPonk.vidaActual <= 80f)
                {
                    ActivarFaseFinalPonk();
                }
                break;

            case 4:
                // Fase final: solo jefe
                break;
        }
    }

    void ActivarFasePelotas()
    {
        tiempoFase = 0;
        faseActual = 0;
        ActivarPelotas(true);
        ActivarGlobos(false);
        jefePonk.SetActive(false);
        Debug.Log("FASE 0: Solo pelotas");
    }

    void ActivarFaseGlobos()
    {
        tiempoFase = 0;
        faseActual = 1;
        ActivarPelotas(false);
        ActivarGlobos(true);
        jefePonk.SetActive(false);
        Debug.Log("FASE 1: Solo globos");
    }

    void ActivarFaseConPonk()
    {
        tiempoFase = 0;
        faseActual = 2;
        ActivarPelotas(true);
        ActivarGlobos(false);
        jefePonk.SetActive(true);
        vidasPonk.invulnerable = false;
        Debug.Log("FASE 2: Ponk + pelotas");
    }

    void ActivarFase2Ponk()
    {
        faseActual = 3;
        ActivarPelotas(false);
        ActivarGlobos(true);
        Debug.Log("FASE 3: Ponk + globos");
    }

    void ActivarFaseFinalPonk()
    {
        faseActual = 4;
        ActivarPelotas(false);
        ActivarGlobos(false);
        Debug.Log("FASE 4: Solo Ponk (Rage)");
    }

    void ActivarPelotas(bool estado)
    {
        foreach (CañonScript cañon in cañones)
        {
            cañon.activo = estado;
        }
    }

    void ActivarGlobos(bool estado)
    {
        if (spawnGlobos != null)
            spawnGlobos.activo = estado;
    }
}
