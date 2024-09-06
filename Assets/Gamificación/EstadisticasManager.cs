using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{

    private void Awake()
    {
        PomodoroSistema.TemposTerminado += RegistrarTempoTerminado;
    }



    [HideInInspector] public static TimeSpan TiempoTempoProductivoPromedio = new TimeSpan(0,15,0);


    [HideInInspector] public static TimeSpan TiempoTotalProductivoDiarioPromedio = new TimeSpan(4,0,0);
    [HideInInspector] public static TimeSpan TiempoTotalProductivoHoy = new TimeSpan(0,0,0);
  
  public void RegistrarTempoTerminado(Tempos tempoTerminado)
    {
        TiempoTempoProductivoPromedio = (TiempoTempoProductivoPromedio + tempoTerminado.TiempoTotal)/2;
        TiempoTotalProductivoHoy += tempoTerminado.TiempoTotal;



    } 





}
