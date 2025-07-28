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
   public TMP_Text MostrarNumeroAdopcionDisponibleUI;          // Texto para la rareza del premio
    public TMP_InputField nombreInputField; // InputField para colocar el nombre del premio
    public Button confirmarButton; // Botón para confirmar y cerrar la ventana
    public int AdopcionesDisponibles = 1;
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
        confirmarButton.onClick.AddListener(IntentarAdoptar);
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
    /// 

    public void TryAdoptarSoborno(int precio)
    {
        if (Gamificacion.Instance.TryConsumirTiempo(precio))
        {
            EjecutarAdopción();

        }
    }
    private void IntentarAdoptar()
    {
        if(AdopcionAbierta == false)
        {
            string nombreElegidoMotivador = nombreInputField.text;

        

            int cantidadMotivadores = EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count;
            int precioSoborno = (int) (50f * Mathf.Pow(1.3f, cantidadMotivadores)); // Aumenta un 20% aprox por motivador


            if (string.IsNullOrEmpty(nombreElegidoMotivador))
            {
                LogicaVentanaConfirmacion.Instance.ShowPopup("Debes poner un nombre al motivador", "No colocaste ningún tipo de nombre al motivador. Escribe un nombre para poder reclamar al motivador.", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
                return;
            }else if (AdopcionesDisponibles == 0)
            {
                LogicaVentanaConfirmacion.Instance.ShowPopup("No posees ninguna adopción disponible", $"Completa las estrellas dirias o paga {precioSoborno} minutos",  () => TryAdoptarSoborno(precioSoborno), () => Debug.Log("Cancelado"));
                return;
            }

            EjecutarAdopción();

        }
        else
        {
            Debug.Log("Ya tienes una adopción abieta");
        }
       
    }

    private void EjecutarAdopción()
    {
        if(AdopcionesDisponibles != 0)
        {
            AdopcionesDisponibles--;
        }
        AdopcionAbierta = true;
        animacionesHuevo.GetComponent<Animator>().SetBool("HuevoAbierto", true);
        animacionesEfectos.GetComponent<Animator>().SetInteger("Rareza", (int)AdopcionManager.Instance.adoptadoRareza);
        animacionesAdoptado.gameObject.SetActive(true);
        animacionesAdoptado.GetComponent<Animator>().SetInteger("Rareza", (int)AdopcionManager.Instance.adoptadoRareza);
        animacionesAdoptado.GetComponent<Animator>().SetInteger("Especie", (int)AdopcionManager.Instance.adoptadoEspecie);
        ActualizarMostrarNumeroAdopcionDisponibleUI();
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
    private const string TutorialAdopcionKey = "AdopcionTutorialEntregada";

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
        if (!PlayerPrefs.HasKey(TutorialAdopcionKey))
        {
            // Primera vez: entrega la adopción de regalo
            AdopcionesDisponibles = 1;
            PlayerPrefs.SetInt(AdopcionesKey, AdopcionesDisponibles);
            PlayerPrefs.SetInt(TutorialAdopcionKey, 1);
            PlayerPrefs.Save();
        }
        else if (PlayerPrefs.HasKey(AdopcionesKey))
        {
            AdopcionesDisponibles = PlayerPrefs.GetInt(AdopcionesKey);
        }
        else
        {
            AdopcionesDisponibles = 0; // Si por alguna razón no hay registro, no regales más
        }
    }

  

    public void CerrarVentanaAdopcion()
    {
        gameObject.SetActive(false);
        GuardarNumeroAdopcionesDisponibles();
        if (AdopcionAbierta)
        {
            AdopcionManager.Instance.CrearMotivador(nombreInputField.text);
            animacionesAdoptado.gameObject.SetActive(false);
            AdopcionAbierta = false;
            EstadisticasManager.Instance.GuardarInformarciónMotivadores();
            animacionesEfectos.GetComponent<Animator>().Play("Vacio", -1, 0f);
            animacionesEfectos.GetComponent<Animator>().Update(0f);



            Invoke(nameof(RegenerarMetasConDelay), 0.1f);
            ContadorProgreso.Instance.GuardarProgreso();
        }
   
    }


    private void RegenerarMetasConDelay()
    {
        GestorMetas.Instance.EjecutarRegenerarMetas();
    }

}