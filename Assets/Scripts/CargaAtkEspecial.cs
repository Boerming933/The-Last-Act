using Unity.VisualScripting;
using UnityEngine;

public class CargaAtkEspecial : MonoBehaviour
{
    public Circulin Personaje;
    public float cargas = 0;

    float cooldown = 0.2f;
    float timer = 0f;

    public bool puedeGanarCargas = true;

    public RectTransform barraCarga;

    public Latigo latigo;

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Triangulardo") && timer <= 0f && puedeGanarCargas == true)
        {
            timer = cooldown;

            cargas = cargas + 1;
            Debug.Log("Carga: " + cargas);
            AumentarCarga();
        }

        if (cargas >= 10)
        {
            puedeGanarCargas = false;
            Personaje.puedeCargado = true;
        }
        else
        {
            puedeGanarCargas = true;
            Personaje.puedeCargado = false;
        }

        if (other.CompareTag("Triangulardo"))
        {
            latigo.latigo.SetActive(false);
        }
    }

    void AumentarCarga()
    {
        if (barraCarga != null)
        {
            Vector3 escala = barraCarga.localScale;
            escala.y += 5.4f;
            barraCarga.localScale = escala;
        }
    }
}
