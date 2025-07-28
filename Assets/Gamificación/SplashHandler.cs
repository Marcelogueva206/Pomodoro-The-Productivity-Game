using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashHandler : MonoBehaviour
{
    public GameObject splashUI;
    public AudioSource splashSound;
    public float splashDuration = 5f;
    public float soundOffset = 2f; // Cuánto antes del final suena el sonido

    void Start()
    {
        StartCoroutine(HandleSplash());
    }

    System.Collections.IEnumerator HandleSplash()
    {
        // Esperar hasta que falte poquito
        yield return new WaitForSeconds(splashDuration - soundOffset);

        // Reproducir el sonido
        ControladorSonidos.Instancia.ReproducirSonidoIntroduccion();

        // Esperar lo que falta
        yield return new WaitForSeconds(soundOffset);

        // Ocultar splash
        splashUI.SetActive(false);
    }
}