using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowEffect : MonoBehaviour
{
    [SerializeField] public float speed = 1f;  // Velocidad del cambio de color
    [SerializeField] public float intensity = 0.5f;  // Intensidad del efecto (0 = sin efecto, 1 = full arcoíris)
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField]private Color baseColor = Color.white;  // Color base del sprite
    private Dinosaurio motivador;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        motivador = gameObject.GetComponent<Dinosaurio>();
    }

    void Update()
    {
        if(motivador.GetRereza() == Rareza.Legendaria)
        {
            float t = Time.time * speed;

            // Generar colores con una transición suave
            float r = Mathf.Sin(t) * 0.5f + 0.5f;
            float g = Mathf.Sin(t + 2f) * 0.5f + 0.5f;
            float b = Mathf.Sin(t + 4f) * 0.5f + 0.5f;
            Color rainbowColor = new Color(r, g, b);

            // Mezclar el color base con el efecto arcoíris según la intensidad
            spriteRenderer.color = Color.Lerp(baseColor, rainbowColor, intensity);
        }

       
    }
}
