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

    public RectTransform cuerpoPrimeraMeta;
    public RectTransform cuerpoSegundaMeta;
    public RectTransform cuerpoTerceraMeta;

    [SerializeField] private RectTransform cuerpoMarca0;
    [SerializeField] private RectTransform cuerpoMarca10;
    [SerializeField] private RectTransform cuerpoMarca20;
    [SerializeField] private RectTransform cuerpoMarca30;
    [SerializeField] private RectTransform cuerpoMarca40;
    [SerializeField] private RectTransform cuerpoMarca50;
    [SerializeField] private RectTransform cuerpoMarca60;
    [SerializeField] private RectTransform cuerpoMarca70;
    [SerializeField] private RectTransform cuerpoMarca80;
    [SerializeField] private RectTransform cuerpoMarca90;
    [SerializeField] private RectTransform cuerpoMarca100;
    public void ActualizarMarcas()
    {

        ActualizarMarca(cuerpoPrimeraMeta, GestorMetas.Instance.MetaMinima.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoSegundaMeta, GestorMetas.Instance.MetaDeIntermedio.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoTerceraMeta, GestorMetas.Instance.MetaDeSuperacion.PorcentajeRequeridoMeta);

        ActualizarMarca(cuerpoMarca0, GestorMetas.Instance.Marca0Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca10, GestorMetas.Instance.Marca10Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca20, GestorMetas.Instance.Marca20Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca30, GestorMetas.Instance.Marca30Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca40, GestorMetas.Instance.Marca40Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca50, GestorMetas.Instance.Marca50Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca60, GestorMetas.Instance.Marca60Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca70, GestorMetas.Instance.Marca70Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca80, GestorMetas.Instance.Marca80Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca90, GestorMetas.Instance.Marca90Porciento.PorcentajeRequeridoMeta);
        ActualizarMarca(cuerpoMarca100,GestorMetas.Instance.Marca100Porciento.PorcentajeRequeridoMeta);
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
        //foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
        //{

        //  MetaRecompensa.textoCuerpoMeta.text = MetaRecompensa.PorcentajeRequeridoMeta.ToString();

        //}
    }
    void Update()
    {
        progresoUsuario = Gamificacion.Instance.ProgresoTotalMetaPor / 100f;
        Debug.Log(progresoUsuario);
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
        foreach (MarcaBasicaRecompensa marca in GestorMetas.Instance.marcasSimples)
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
        foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
        {

            if (MetaRecompensa.PorcentajeRequeridoMeta < progresoUsuario)
            {
                MetaRecompensa.MarcaCompletada(true);
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
        TextoPPsAcumulados.text = Gamificacion.Instance.TiempoPorAdquirir.ToString();
        TextoPPsTotales.text = Gamificacion.Instance.TiempoTotal.ToString();
        sliderBarraProgreso.value = progresoUsuario;
        sliderBarraProgresoAcumulado.value = progresoUsuario + (Contador.PuntuacionTempo / GestorMetas.Instance.GetMetaSuperaciónValor());
    }



}







