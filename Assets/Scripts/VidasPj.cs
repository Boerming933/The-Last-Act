using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class VidasPj : MonoBehaviour
{
    public float Vidas = 6;
    public GameObject BotonReiniciar;
    public Triangulardo Ponk;
    public LevelPhaseManager levelManager;
    public Circulin Estian;
    private float Daño;
    private bool IsDashing = false;
    public List<GameObject> IndicadoresDeVida; //Aqui van los 5 corazones de la UI

    public void Hit(float daño)
    {
        if (IsDashing) return;
        Vidas = Vidas - daño;
        Daño = daño;

        ActualizarIndicadoresDeVida();

        if (Vidas <= 0)
        {
            levelManager.ActivarFinal();
            Ponk.MuerteEstian(true);
            Estian.Muerte(true);
            StartCoroutine(ReiniciarNivel());
        }
    }

    public void InDash(bool Dash)
    {
        IsDashing = Dash;
    }
    
    public void ActualizarIndicadoresDeVida()
    {
        if (IndicadoresDeVida.Count > 0)
        {
            if (Daño == 1)
            {
                int ultimoIndice = IndicadoresDeVida.Count - 1;
                IndicadoresDeVida[ultimoIndice].SetActive(false);
                IndicadoresDeVida.RemoveAt(ultimoIndice);
                if (IndicadoresDeVida.Count > 0)
                {
                    IndicadoresDeVida[ultimoIndice - 1].SetActive(true);
                }
            }
            else if (Daño == 2)
            {
                for (int n = 0; n < 2; n++)
                {
                    int ultimoIndice = IndicadoresDeVida.Count - 1;
                    IndicadoresDeVida[ultimoIndice].SetActive(false);
                    IndicadoresDeVida.RemoveAt(ultimoIndice);
                    if (IndicadoresDeVida.Count > 0)
                    {
                        IndicadoresDeVida[ultimoIndice - 1].SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator ReiniciarNivel()
    {
        yield return new WaitForSeconds(10f);
        ReiniciarEscena();
    }
    
    public void ActivarBotonReinicio()
    {
        BotonReiniciar.SetActive(true);
        BotonReiniciar.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ReiniciarEscena);
    }

    public void ReiniciarEscena()
    {
        ControladorSonidos.instance.DetenerSonido();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
