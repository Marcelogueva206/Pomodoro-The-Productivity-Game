using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{

    // Singleton Instance
    public static EstadisticasManager Instance { get; private set; }



    private void Awake()
    {
        #region ExtraA
        // Implementación del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Elimina duplicados.
            return;
        }
        Instance = this; 
        #endregion


        PomodoroSistema.TemposTerminado += RegistrarTempoTerminado;

        #region ExtraB
        DontDestroyOnLoad(gameObject); //evita que se destruya entre otras escenas 
        #endregion
    }

    [HideInInspector] public static TimeSpan TiempoTempoProductivoPromedio = new TimeSpan(0, 15, 0);
    [HideInInspector] public static TimeSpan TiempoTotalProductivoDiarioPromedio = new TimeSpan(4, 0, 0);
    [HideInInspector] public static TimeSpan TiempoTotalProductivoHoy = new TimeSpan(0, 0, 0);


    public void RegistrarTempoTerminado(Tempos tempoTerminado)
    {
        TiempoTempoProductivoPromedio = (TiempoTempoProductivoPromedio + tempoTerminado.TiempoTotal) / 2;
        TiempoTotalProductivoHoy += tempoTerminado.TiempoTotal;



    }




    private void OnDestroy()
    {
        // Desuscribir el evento para evitar referencias huérfanas.  
        PomodoroSistema.TemposTerminado -= RegistrarTempoTerminado;
    }




}
