using UnityEngine;

public class ControladorSonidos : MonoBehaviour
{
    public static ControladorSonidos instance;
    public AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirSonido(AudioClip sonido)
    {
        audioSource.PlayOneShot(sonido);
    }
    public void DetenerSonido()
    {
        audioSource.Stop();
    }
}
