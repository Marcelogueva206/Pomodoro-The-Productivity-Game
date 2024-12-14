using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{

    // Singleton Instance
    public static EstadisticasManager Instance { get; private set; }

    #region Sistema de integración de motivadores
    [SerializeField] private List<Dinosaurio> CaracteresMotivadoresEnSistema;
    public List<Dinosaurio> getCaracteresMotivadoresEnSistema()
    {
        return CaracteresMotivadoresEnSistema;
    }
    public void AñadirCaracterMotivadorAlSistema(Dinosaurio motivador)
    {
        if (!CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Add(motivador);
        }
    }
    public void EliminarCaracterMotivadorDelSistema(Dinosaurio motivador)
    {
        if (CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Remove(motivador);
        }
    }
    #endregion


    public DateTime fechaUltimaVezSesionIniciada;

    private const string KeyUltimaFecha = "UltimaFecha";


    private void Start()
    {
        DateTime fechaActual = DateTime.Now;

        if (PlayerPrefs.HasKey(KeyUltimaFecha))
        {
            string ultimaFecha = PlayerPrefs.GetString(KeyUltimaFecha);
            Debug.Log("Última vez que se abrió la aplicación: " + ultimaFecha);
        }else
        {
            Debug.Log("Es la primera vez que abres la aplicación.");
        }

        PlayerPrefs.SetString(KeyUltimaFecha, fechaActual.ToString("yyyy-MM-dd HH:mm:ss"));
        PlayerPrefs.Save(); // Asegura que los datos se guarden en disco

        Debug.Log("Fecha actual guardada: " + fechaActual.ToString("yyyy-MM-dd HH:mm:ss"));
    }


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
