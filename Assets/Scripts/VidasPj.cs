using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class VidasPj : MonoBehaviour
{
    public int Vidas = 5;
    public GameObject BotonReiniciar;
    public Triangulardo Ponk;
    public LevelPhaseManager levelManager;
    public Circulin Estian;
    public List<GameObject> IndicadoresDeVida; //Aqui van los 5 corazones de la UI

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ataques")
        {
<<<<<<< Updated upstream
            Vidas -= 1;

            ActualizarIndicadoresDeVida();

            if (Vidas <= 0)
            {
                gameObject.SetActive(false);
                ActivarBotonReinicio();
            }
=======
            levelManager.ActivarFinal();
            Ponk.MuerteEstian(true);
            Estian.Muerte(true);
            StartCoroutine(ReiniciarNivel());
>>>>>>> Stashed changes
        }
    }
    
    public void ActualizarIndicadoresDeVida()
    {
        if (IndicadoresDeVida.Count > 0)
        {
            // Desactiva el ultimo (de derecha a izquierda)
            int ultimoIndice = IndicadoresDeVida.Count - 1;
            IndicadoresDeVida[ultimoIndice].SetActive(false);

            // Lo quita de la lista
            IndicadoresDeVida.RemoveAt(ultimoIndice);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
