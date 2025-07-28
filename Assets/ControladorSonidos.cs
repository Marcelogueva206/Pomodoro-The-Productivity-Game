using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorSonidos : MonoBehaviour
{

    public static ControladorSonidos Instancia;

    public AudioClip sonidoBoleto;
    public AudioClip sonidoAdopcion;
    public AudioClip sonidoIntroduccion;
    public AudioClip sonidoGanarRuleta;
    public AudioClip sonidoComer1;
    public AudioClip sonidoComer2;
    public AudioClip sonidoVender;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas si deseas
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirSonidoBoleto()
    {
        audioSource.PlayOneShot(sonidoBoleto);
    }

    public void ReproducirSonidoAdopcion()
    {
        audioSource.PlayOneShot(sonidoAdopcion);
    }

    public void ReproducirSonidoIntroduccion()
    {
        audioSource.PlayOneShot(sonidoIntroduccion);
    }

    public void ReproducirSonidoGanarRuleta()
    {
        audioSource.PlayOneShot(sonidoGanarRuleta);
    }

    public void ReproducirSonidoComer()
    {
        ReproducirSonidoAleatorio(new AudioClip[] { sonidoComer1, sonidoComer2 });
    }

    public void ReproducirSonidoVender()
    {
        audioSource.PlayOneShot(sonidoVender);
    }

    public void ReproducirSonidoAleatorio(params AudioClip[] sonidos)
    {
        if (sonidos.Length == 0)
        {
            Debug.LogWarning("No se han pasado sonidos al método.");
            return;
        }

        int indiceAleatorio = Random.Range(0, sonidos.Length); // desde 0 hasta sonidos.Length - 1
        audioSource.clip = sonidos[indiceAleatorio];
        audioSource.Play();
    }
}
