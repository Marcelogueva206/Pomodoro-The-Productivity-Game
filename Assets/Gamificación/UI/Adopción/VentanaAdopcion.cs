using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;

public class VentanaAdopcion : MonoBehaviour
{
    public static VentanaAdopcion Instance { get; private set; } // Singleton Instance

    [Header("UI Elements")]
    public TMP_Text tipoElementoText;    // Texto para el tipo de elemento
    public TMP_Text rarezaText;          // Texto para la rareza del premio
    public TMP_InputField nombreInputField; // InputField para colocar el nombre del premio
    public Button confirmarButton; // Botón para confirmar y cerrar la ventana


    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Configurar el botón para llamar al método ConfirmarRecompensa
        confirmarButton.onClick.AddListener(ConfirmarRecompensa);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Configura los textos de la ventana con los datos del premio.
    /// </summary>
    /// <param name="tipo">El tipo de elemento del premio.</param>
    /// <param name="rareza">La rareza del premio.</param>
    public void ConfigurarVentana(string tipo, string rareza)
    {
     
        tipoElementoText.text = tipo;
        rarezaText.text = rareza;

        // Limpiar el campo de texto del InputField
        nombreInputField.text = "";
    }

    /// <summary>
    /// Lógica para confirmar el premio y cerrar la ventana.
    /// </summary>
    private void ConfirmarRecompensa()
    {
        string nombreElegidoMotivador = nombreInputField.text;

        if (string.IsNullOrEmpty(nombreElegidoMotivador))
        {
            LogicaVentanaConfirmacion.Instance.ShowPopup("Debes poner un nombre al motivador", "No colocaste ningún tipo de nombre al motivador. Escribe un nombre para poder reclamar al motivador.", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
            return;
        }

        AdopcionManager.Instance.CrearMotivador(nombreElegidoMotivador);

        // Ocultar la ventana o realizar otras acciones necesarias
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Método para mostrar la ventana y asignar el premio (se llamará desde otro script).
    /// </summary>
    /// <param name="tipo">El tipo de elemento.</param>
    /// <param name="rareza">La rareza del elemento.</param>
    public void MostrarVentanaAdopcion(Especie especie, Rareza rareza)
    {
        ConfigurarVentana(especie.ToString(), rareza.ToString());
        gameObject.SetActive(true);
    }
}