using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuletaRecompensa : MonoBehaviour
{
    public static RuletaRecompensa Instancia { get; private set; } // Instancia estática de la clase

    public Image ruedaImage; // La imagen de la rueda
    public Button girarButton; // El botón para girar
    public float velocidadGiro = 1000f; // Velocidad con la que gira la rueda
    public float tiempoGiro = 3f; // Tiempo en el que la rueda va a girar
    public bool girando = false;
    private string resultado = "";
   

    [SerializeField]
    private string[] premios = new string[16] {
        "Premio 1", "Premio 2", "Premio 3", "Premio 4",
        "Premio 5", "Premio 6", "Premio 7", "Premio 8",
        "Premio 9", "Premio 10", "Premio 11", "Premio 12",
        "Premio 13", "Premio 14", "Premio 15", "Premio 16"
    };

    void Awake()
    {
        // Verifica si ya existe una instancia y destruye el objeto si ya existe
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject); // Destruye el objeto duplicado
            return;
        }
        Instancia = this; // Asigna la instancia
        DontDestroyOnLoad(gameObject); // No destruye el objeto al cargar nuevas escenas
    }

    void Start()
    {
        girarButton.onClick.AddListener(IniciarGiro);
    }

    void IniciarGiro()
    {
        SistemaRecompensa.Instancia.ActualizarMostrarIntentosUI();
        if (SistemaRecompensa.Instancia.intentosRuleta > 0)
        {
            if (!girando)
            {
                StartCoroutine(GirarRuleta());
                SistemaRecompensa.Instancia.intentosRuleta--;
                SistemaRecompensa.Instancia.GuardarRecompensas();
                    
                SistemaRecompensa.Instancia.ActualizarMostrarIntentosUI();
            }
        }


    }

    public string GetResultado()
    {
        return resultado;
    }
    IEnumerator GirarRuleta()
    {
        resultado = "";
        girando = true;

        // Define un giro aleatorio entre 1 y 10 vueltas completas
        float giroAleatorio = Random.Range(1f, 10f);
        float giroFinal = giroAleatorio * 360f;

        float tiempoTranscurrido = 0f;
        float giroActual = 0f;

        // Gira la rueda por un tiempo determinado
        while (tiempoTranscurrido < tiempoGiro)
        {
            float deltaTime = Time.deltaTime;
            giroActual = Mathf.Lerp(0f, giroFinal, tiempoTranscurrido / tiempoGiro);
            ruedaImage.transform.rotation = Quaternion.Euler(0f, 0f, giroActual);
            tiempoTranscurrido += deltaTime;

            yield return null;
        }

        // Ajusta la posición final al rango de 0° a 360°
        float anguloFinal = giroFinal % 360f;

        // Determina el segmento ganador
        int segmentoGanador = CalcularSegmento(anguloFinal);
        Debug.Log($"¡resultado: {premios[segmentoGanador]}!");
        resultado = premios[segmentoGanador];
        girando = false;
        SistemaRecompensa.Instancia.ActualizarMostrarResultadoUI();
        SistemaRecompensa.Instancia.ActualizarMostrarIntentosUI();
        if (resultado == "Adopción")
        {
            VentanaAdopcion.Instance.GanarUnaAdopción();
        }
    }

    // Método para calcular el segmento ganador
    int CalcularSegmento(float angulo)
    {
        // Asegúrate de que el ángulo esté en el rango 0°-360°
        angulo = angulo % 360f;
        if (angulo < 0f)
        {
            angulo += 360f;
        }

        // Determina el número de segmentos
        int numeroDeSegmentos = premios.Length;

        // Calcula el tamaño de cada segmento
        float tamañoSegmento = 360f / numeroDeSegmentos;

        // Calcula el índice del segmento ganador
        return Mathf.FloorToInt(angulo / tamañoSegmento);
    }
}