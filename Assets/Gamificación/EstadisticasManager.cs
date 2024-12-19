using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{

    // Singleton Instance
    public static EstadisticasManager Instance { get; private set; }
    [SerializeField]private GameObject MotivadorPrefab;

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

        PlayerPrefs.SetInt("CantidadMotivadores", CaracteresMotivadoresEnSistema.Count);
        GuardarInformarciónMotivadores();
    }
    public void EliminarCaracterMotivadorDelSistema(Dinosaurio motivador)
    {
        if (CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Remove(motivador);
        }
        PlayerPrefs.SetInt("CantidadMotivadores", CaracteresMotivadoresEnSistema.Count);
        GuardarInformarciónMotivadores();
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

        PlayerPrefs.GetInt("CantidadMotivadores", 0);
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



    public void GuardarInformarciónMotivadores()
    {
        ListaMotivadoresData data = new ListaMotivadoresData();
        data.motivadores = CaracteresMotivadoresEnSistema;

        string json = JsonUtility.ToJson(data, true);

        System.IO.File.WriteAllText("lista_motivadores_data", json);
    }

    public void CargarInformaciónMotivadores()
    {
        List <Dinosaurio> motivadoresCargados = new List <Dinosaurio>();
        if (System.IO.File.Exists("lista_motivadores_data"))
        {
            string json = System.IO.File.ReadAllText("lista_motivadores_data");
            ListaMotivadoresData lista = JsonUtility.FromJson<ListaMotivadoresData>(json);
            motivadoresCargados = lista.motivadores;
        }
        CaracteresMotivadoresEnSistema.Clear();
        foreach (Dinosaurio motivadorCargado in motivadoresCargados)
        {
            GameObject MotivadorNuevo= Instantiate(MotivadorPrefab);
            Dinosaurio componenteDinosaurioNuevo = MotivadorNuevo.GetComponent<Dinosaurio>();
            if (componenteDinosaurioNuevo != null)
            {
                // Copia las propiedades del motivador cargado al componente del prefab
                componenteDinosaurioNuevo.Nombre = motivadorCargado.Nombre;
                componenteDinosaurioNuevo.Emocionalidad = motivadorCargado.Emocionalidad;
                componenteDinosaurioNuevo.exigencias = motivadorCargado.exigencias;
                componenteDinosaurioNuevo.gameObject.transform.position = motivadorCargado.transform.position;

                // Copia otras propiedades necesarias aquí
            } 


        }

        

    }
}


#region Data
[System.Serializable]
public class ListaMotivadoresData
{
    public List<Dinosaurio> motivadores;
}
#endregion