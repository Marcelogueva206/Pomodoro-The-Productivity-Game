using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Animations;

public class VentanaAdopcion : MonoBehaviour
{
    public static VentanaAdopcion Instance { get; private set; } // Singleton Instance

    [Header("UI Elements")]
   public TMP_Text MostrarNumeroAdopcionDisponibleUI;          // Texto para la rareza del premio
    public TMP_InputField nombreInputField; // InputField para colocar el nombre del premio
    public Button confirmarButton; // Botón para confirmar y cerrar la ventana
    public int AdopcionesDisponibles = 0;
    public GameObject animacionesHuevo;
    public GameObject animacionesEfectos;
    public GameObject animacionesAdoptado;
    public bool AdopcionAbierta = false;

    public void ActualizarMostrarNumeroAdopcionDisponibleUI()
    {
        MostrarNumeroAdopcionDisponibleUI.text =  AdopcionesDisponibles.ToString();
    }



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
        confirmarButton.onClick.AddListener(Adoptar);
        animacionesAdoptado.SetActive(false);
        gameObject.SetActive(false);
        CargarNumeroAdopcionesDisponibles();
    }

   
    //public void ConfigurarVentana(string tipo, string rareza)
    //{
    
    //    // Limpiar el campo de texto del InputField
       
    //}

    /// <summary>
    /// Lógica para confirmar el premio y cerrar la ventana.
    /// </summary>
    private void Adoptar()
    {
        if(AdopcionAbierta == false)
        {
            string nombreElegidoMotivador = nombreInputField.text;

            if (AdopcionesDisponibles == 0)
            {
                LogicaVentanaConfirmacion.Instance.ShowPopup("No posees ninguna adoción disponible", "Debes adquirirlas en al ruleta al completar las estrellas dirias", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
                return;
            }


            if (string.IsNullOrEmpty(nombreElegidoMotivador))
            {
                LogicaVentanaConfirmacion.Instance.ShowPopup("Debes poner un nombre al motivador", "No colocaste ningún tipo de nombre al motivador. Escribe un nombre para poder reclamar al motivador.", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
                return;
            }

            AdopcionesDisponibles--;
            AdopcionAbierta = true;
            animacionesHuevo.GetComponent<Animator>().SetBool("HuevoAbierto",true);
            animacionesEfectos.GetComponent<Animator>().SetInteger("Rareza", (int)AdopcionManager.Instance.adoptadoRareza);
            animacionesAdoptado.gameObject.SetActive(true);
            animacionesAdoptado.GetComponent<Animator>().SetInteger("Rareza", (int)AdopcionManager.Instance.adoptadoRareza);
            animacionesAdoptado.GetComponent<Animator>().SetInteger("Especie", (int)AdopcionManager.Instance.adoptadoEspecie);
            ActualizarMostrarNumeroAdopcionDisponibleUI();




        }
        else
        {
            Debug.Log("Ya tienes una adopción abieta");
        }
       
    }








    /// <summary>
    /// Método para mostrar la ventana y asignar el premio (se llamará desde otro script).
    /// </summary>
    /// <param name="tipo">El tipo de elemento.</param>
    /// <param name="rareza">La rareza del elemento.</param>
    public void MostrarVentanaAdopcion(Especie especie, Rareza rareza)
    {
        nombreInputField.text = "";
        gameObject.SetActive(true);
        ActualizarMostrarNumeroAdopcionDisponibleUI();
    }

    /// <summary>
    /// Método para guardar AdopcionesDisponibles en PlayerPrefs.
    /// </summary>
     private const string AdopcionesKey = "AdopcionesDisponibles";
    public void GanarUnaAdopción()
    {
        AdopcionesDisponibles++;
        GuardarNumeroAdopcionesDisponibles();
        ActualizarMostrarNumeroAdopcionDisponibleUI();
    }

    private void GuardarNumeroAdopcionesDisponibles()
    {
        PlayerPrefs.SetInt(AdopcionesKey, AdopcionesDisponibles);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Método para cargar AdopcionesDisponibles desde PlayerPrefs.
    /// </summary>
    private void CargarNumeroAdopcionesDisponibles()
    {
        if (PlayerPrefs.HasKey(AdopcionesKey))
        {
            AdopcionesDisponibles = PlayerPrefs.GetInt(AdopcionesKey);
        }
        else
        {
            AdopcionesDisponibles = 0; // Valor por defecto si no existe la clave
        }
    }


    public void CerrarVentanaAdopcion()
    {
        gameObject.SetActive(false);

        if (AdopcionAbierta)
        {
            AdopcionManager.Instance.CrearMotivador(nombreInputField.text);
            animacionesAdoptado.gameObject.SetActive(false);
        }
    }

}