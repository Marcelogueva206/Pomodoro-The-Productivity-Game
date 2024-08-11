using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;



public delegate void ProgresoMetaUsuario(float progreso, float puntuacion);
public class ContadorProgreso : MonoBehaviour
{
    public event ProgresoMetaUsuario PremioPorProgresoUsuario = (progreso,puntacion) =>  {

        Debug.Log("Progre META:" + progreso +"%");
    };


    private float progresoUsuario;
    private float progresoUsuarioAcumulado;
    [SerializeField] private GameObject barraProgreso;
    [HideInInspector] private Slider sliderBarraProgreso;
    [SerializeField] private Slider sliderBarraProgresoAcumulado;
    [SerializeField] private RectTransform markerSegundaEstrella; // El marcador que se moverá
    [SerializeField] private RectTransform markerPrimeraEstrella; // El marcador que se moverá
    [SerializeField] private bool[] RecompensasObtenidasProgresoMeta = new bool[] {false, false,false,false,false,false,false,false,false,false};
    [SerializeField] private float PorcentajeSegundaEstrella = 50f; // Porcentaje objetivo para la marca
    [SerializeField] private float PorcentajePrimeraEstrella = 10f; // Porcentaje objetivo para la marca

    [SerializeField] private TextMeshProUGUI TextoPPsAcumulados;
    [SerializeField] private TextMeshProUGUI TextoPPsTotales;
    public void ActualizarMarcas()
    {
      
        ActualizarMarca(markerSegundaEstrella, PorcentajeSegundaEstrella);
        ActualizarMarca(markerPrimeraEstrella, PorcentajePrimeraEstrella);
    }

    private void ActualizarMarca(RectTransform marca, float porcentaje)
    {
        marca.GetComponentInChildren< TextMeshProUGUI >().text = porcentaje.ToString();
        // Asegúrate de que el valor esté entre 0 y 1
        float normalizedPercentage = Mathf.Clamp(porcentaje / 100f, 0f, 1f);

        // Calcula la posición del marcador en el ancho de la barra, calibrado para empezar desde 0%
        float markerPositionX = normalizedPercentage * sliderBarraProgreso.GetComponent<RectTransform>().rect.width;

        // Actualiza la posición del marcador, considerando el ancho de la barra
        marca.anchoredPosition = new Vector2(markerPositionX - (sliderBarraProgreso.GetComponent<RectTransform>().rect.width * 0.5f), marca.anchoredPosition.y);
    }

    void OnValidate()
    {
        if (sliderBarraProgreso != null && markerSegundaEstrella != null)
        {
            ActualizarMarcas();
        }
    }

    private void Start()
    {
        Time.timeScale = 3.0f; //BORAR ANTE TODO
    }
    private void Awake()
    {
        sliderBarraProgreso = barraProgreso.GetComponent<Slider>();

    }
    void Update()
    {
        progresoUsuario = Gamificacion.ProgresoTotalMeta / 100;

        MostrarProgresoUI();
        ActualizarMarcas();



        for (int i = 0; i < 10; i++)
        {
            if (progresoUsuario > 10*i)
            {
                InvocarEventoParaRecompensa(i);

            }
            else
            {
                return;
            }
        }   

    }

    private void InvocarEventoParaRecompensa(int index)
    {
        if (RecompensasObtenidasProgresoMeta[0] == false)
        {
            PremioPorProgresoUsuario.Invoke(progresoUsuario, Gamificacion.ProductivityPointsDaily);
            RecompensasObtenidasProgresoMeta[0] = true;
        }
    }

    private void MostrarProgresoUI()
    {
        TextoPPsAcumulados.text = Gamificacion.ProductivityPointsHolded.ToString();
        TextoPPsTotales.text = Gamificacion.ProductivityPointsTotal.ToString();
        sliderBarraProgreso.value = progresoUsuario;
        sliderBarraProgresoAcumulado.value = progresoUsuario+(Contador.PuntuacionTempo/Gamificacion.ProductivityPointsMaxGoal);
    }



}
