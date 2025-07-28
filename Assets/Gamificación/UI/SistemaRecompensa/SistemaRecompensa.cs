using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SistemaRecompensa : MonoBehaviour
{
    // Campo estático para almacenar la instancia única
    private static SistemaRecompensa _instancia;
    public TextMeshProUGUI TextoMostrarIntentos;
    public TextMeshProUGUI TextoMostrarResultado;
    // Propiedad pública para acceder a la instancia
    public static SistemaRecompensa Instancia
    {
        get
        {
            if (_instancia == null)
            {
                Debug.LogError("SistemaRecompensa no está inicializado en la escena.");
            }
            return _instancia;
        }
    }
   public int intentosRuleta = 0;
  

    public void ActualizarMostrarIntentosUI()
    {
        TextoMostrarIntentos.text = intentosRuleta.ToString();
    }

    public void ActualizarMostrarResultadoUI()
    {
        if(RuletaRecompensa.Instancia.GetResultado() == "Adopción")
        {
            TextoMostrarResultado.text = "¡Ganaste!";
            ControladorSonidos.Instancia.ReproducirSonidoGanarRuleta();
        }
        else
        {
            TextoMostrarResultado.text = "¡Perdiste!";
        }



        

        
    }

   public void IntentoRuletaGanado()
    {
        intentosRuleta++;
        GuardarRecompensas();
    }

    private void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            Debug.LogWarning("Hay más de un SistemaRecompensa en la escena. Eliminando el duplicado.");
            Destroy(this.gameObject);
            return;
        }

        _instancia = this;

        // Opcional: Haz que el objeto persista entre escenas
        DontDestroyOnLoad(this.gameObject);
    }


 
    void Start()
    {
        CargarRecompensas();
        CerrarVentanaSistemaRecompensa();
    }


    void Update()
    {
     
    }

    public void AbrirVentanaSistemaRecompensa()
    {
        gameObject.SetActive(true);
        ActualizarMostrarIntentosUI();
    }

    public void BotonAbrirVentanaSistemaRecompensa()
    {
        LogicaVentanaConfirmacion.Instance.ShowPopup("Ruleta", "Toca las estrellas completadas o alimenta a tus mascotas para reclamar boletos.", () => { AbrirVentanaSistemaRecompensa(); }, () => { Debug.Log("Cancelado"); });
    }



    public void CerrarVentanaSistemaRecompensa()
    {
        if(RuletaRecompensa.Instancia.girando == false)
        {
            gameObject.SetActive(false);
        }
        
    }
    private string keyIntentosRuleta = "intentosRuleta";
    public void GuardarRecompensas()
    {
            PlayerPrefs.SetInt(keyIntentosRuleta, intentosRuleta);   
    }

    private void CargarRecompensas()
    {
        if (PlayerPrefs.HasKey(keyIntentosRuleta))
        {
             intentosRuleta = PlayerPrefs.GetInt(keyIntentosRuleta);
        }
        else
        {
            intentosRuleta = 0;
        }
    }

}
