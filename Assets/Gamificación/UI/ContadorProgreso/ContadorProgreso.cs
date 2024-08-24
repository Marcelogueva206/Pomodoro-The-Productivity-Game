using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;




public delegate void ProgresoMetaUsuario(float progreso);
public delegate void ProgresoMarcaUsuario(MarcaBasicaRecompensa marcaLograda);
public class ContadorProgreso : MonoBehaviour
{
    public event ProgresoMetaUsuario PremioPorProgresoUsuario = (progreso) =>
    {

        Debug.Log("Progreso de META:" + progreso + "%");
    };



    private float progresoUsuario;
    private float progresoUsuarioAcumulado;
    [SerializeField] private GameObject barraProgreso;
    [HideInInspector] private Slider sliderBarraProgreso;
    [SerializeField] private Slider sliderBarraProgresoAcumulado;
    [SerializeField] private TextMeshProUGUI TextoPPsAcumulados;
    [SerializeField] private TextMeshProUGUI TextoPPsTotales;
    public void ActualizarMarcas()
    {

        ActualizarMarca(Gamificacion.PrimeraEstrella.Cuerpo, Gamificacion.PrimeraEstrella.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.SegundaEstrella.Cuerpo, Gamificacion.SegundaEstrella.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.TerceraEstrella.Cuerpo, Gamificacion.TerceraEstrella.PorcentajeRequeridoMeta);

        ActualizarMarca(Gamificacion.Marca0Porciento.Cuerpo, Gamificacion.Marca0Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca10Porciento.Cuerpo, Gamificacion.Marca10Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca20Porciento.Cuerpo, Gamificacion.Marca20Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca30Porciento.Cuerpo, Gamificacion.Marca30Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca40Porciento.Cuerpo, Gamificacion.Marca40Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca50Porciento.Cuerpo, Gamificacion.Marca50Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca60Porciento.Cuerpo, Gamificacion.Marca60Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca70Porciento.Cuerpo, Gamificacion.Marca70Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca80Porciento.Cuerpo, Gamificacion.Marca80Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca90Porciento.Cuerpo, Gamificacion.Marca90Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(Gamificacion.Marca100Porciento.Cuerpo, Gamificacion.Marca100Porciento.PorcentajeRequeridoMeta);
    }

    private void ActualizarMarca(RectTransform marca, float porcentaje)
    {
        marca.GetComponentInChildren<TextMeshProUGUI>().text = porcentaje.ToString();
        // Asegúrate de que el valor esté entre 0 y 1
        float normalizedPercentage = Mathf.Clamp(porcentaje / 100f, 0f, 1f);

        // Calcula la posición del marcador en el ancho de la barra, calibrado para empezar desde 0%
        float markerPositionX = normalizedPercentage * sliderBarraProgreso.GetComponent<RectTransform>().rect.width;

        // Actualiza la posición del marcador, considerando el ancho de la barra
        marca.anchoredPosition = new Vector2(markerPositionX - (sliderBarraProgreso.GetComponent<RectTransform>().rect.width * 0.5f), marca.anchoredPosition.y);
    }

    private void Awake()
    {
        sliderBarraProgreso = barraProgreso.GetComponent<Slider>();

    }

    private void Start()
    {
        foreach (EstrellaRecompensa estrellaRecompensa in new EstrellaRecompensa[] { Gamificacion.PrimeraEstrella, Gamificacion.SegundaEstrella, Gamificacion.TerceraEstrella })
        {

          estrellaRecompensa.textoCuerpoMeta.text = estrellaRecompensa.PorcentajeRequeridoMeta.ToString();

        }
    }
    void Update()
    {
        progresoUsuario = Gamificacion.ProgresoTotalMeta / 100;

        MostrarProgresoUI();
        ActualizarMarcas();

        //for (int i = 0; i < 10; i++)
        //{
        //    if (progresoUsuario * 100 > 10 * (i + 1))
        //    {
        //        InvocarEventoParaRecompensa(i, (i + 1) * 10);

        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

        VerificarLogroDeEstrellas();

        VerificarLogrosDeMarcas();

    }

    private void VerificarLogrosDeMarcas()
    {
        foreach (MarcaBasicaRecompensa marca in Gamificacion.marcasSimples)
        {
            if (marca.PorcentajeRequeridoMeta < progresoUsuario)
            {
                marca.MarcaCompletada(true);
            }
            else
            {
                return;
            }
        }
    }

    private void VerificarLogroDeEstrellas()
    {
        foreach (EstrellaRecompensa estrellaRecompensa in new EstrellaRecompensa[] { Gamificacion.PrimeraEstrella, Gamificacion.SegundaEstrella, Gamificacion.TerceraEstrella })
        {

            if (estrellaRecompensa.PorcentajeRequeridoMeta < progresoUsuario)
            {
                estrellaRecompensa.MarcaCompletada(true);
            }
            else
            {
                return;
            }


        }
    }

    //private void InvocarEventoParaRecompensa(int index, float progresoLogrado)
    //{
    //    if (RecompensasObtenidasProgresoMeta[index] == false)
    //    {
    //        PremioPorProgresoUsuario.Invoke(progresoLogrado);
    //        RecompensasObtenidasProgresoMeta[index] = true;
    //    }
    //}

    private void MostrarProgresoUI()
    {
        TextoPPsAcumulados.text = Gamificacion.ProductivityPointsHolded.ToString();
        TextoPPsTotales.text = Gamificacion.ProductivityPointsTotal.ToString();
        sliderBarraProgreso.value = progresoUsuario;
        sliderBarraProgresoAcumulado.value = progresoUsuario + (Contador.PuntuacionTempo / Gamificacion.ProductivityPointsMaxGoal);
    }



}



public class MarcaBasicaRecompensa
{
    public string Nombre;
    protected bool Completado = false;
    public RectTransform Cuerpo;
    public TextMeshProUGUI textoCuerpoMeta;
    protected float PuntuacionRequerida { get => (PorcentajeRequeridoMeta / 100) * Gamificacion.ProductivityPointsMaxGoal; set { } }
    protected float porcentajeRequeridoMeta;
    public float PorcentajeRequeridoMeta
    {
        get => porcentajeRequeridoMeta; set
        {

            if (value >= 0 && value <= 100)
            {
                porcentajeRequeridoMeta = value;
            }
            else
            {
                // Puedes manejar el caso en que el valor esté fuera del rango, por ejemplo:
                porcentajeRequeridoMeta = 0;
                throw new ArgumentOutOfRangeException("PorcentajeRequeridoMeta", "El valor debe estar entre 0 y 100.");
            }
        }
    }

    public MarcaBasicaRecompensa(string nombre, RectTransform cuerpo, float porcentajeRequerido)
    {
        this.Nombre = nombre;
        Cuerpo = cuerpo;
        PorcentajeRequeridoMeta = porcentajeRequerido;
        textoCuerpoMeta = cuerpo.gameObject.GetComponentInChildren<TextMeshProUGUI>();

    }

    public event ProgresoMarcaUsuario PremioPorMarcaLograda = (marca) =>
    {

        Debug.Log($"¡Marca {marca.Nombre} logarado!:" + marca.PorcentajeRequeridoMeta + "%");
    };


    public void SetPorcentajeRequeridoSegunPuntuacion(float puntuacion)
    {

    }


    public void MarcaCompletada(bool estado)
    {
        Completado = estado;
        if (Completado == true)
        {
            PremioPorMarcaLograda?.Invoke(this);
        }

    }

}



public enum TipoEstrella { EstrellaPorInicio, EstrellaMinima, EstrellaMaxima }
public class EstrellaRecompensa : MarcaBasicaRecompensa
{
    readonly TipoEstrella tipoEstrella;
    
    public EstrellaRecompensa(string nombre, RectTransform cuerpo, float porcentajeRequerido, TipoEstrella tipoEstrella) : base(nombre ,cuerpo, porcentajeRequerido) 
    {
        this.tipoEstrella = tipoEstrella;
    }

  


}