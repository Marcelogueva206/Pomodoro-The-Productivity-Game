using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{

    // Singleton Instance
    public static EstadisticasManager Instance { get; private set; }
    [SerializeField] private GameObject MotivadorPrefab;

    public string keyCantidadMotivadores = "CantidadMotivadores";

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
            StartCoroutine(GestorMetas.Instance.EsperarParaIniciarGeneraciónDeMetas());
        }
        GuardarInformarciónMotivadores();

    }
    public void EliminarCaracterMotivadorDelSistema(Dinosaurio motivador)
    {
        if (CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Remove(motivador);
            StartCoroutine(GestorMetas.Instance.EsperarParaIniciarGeneraciónDeMetas());
        }
        GuardarInformarciónMotivadores();

    }
    #endregion


    public DateTime fechaUltimaVezSesionIniciada;

    private const string KeyUltimaFecha = "UltimaFecha";


    private void Start()
    {
        //if (!PlayerPrefs.HasKey(keyCantidadMotivadores))
        //{
        //    PlayerPrefs.SetInt(keyCantidadMotivadores, 0);
        //    PlayerPrefs.Save();
        //}


        DateTime fechaActual = DateTime.Now;

        if (PlayerPrefs.HasKey(KeyUltimaFecha))
        {
            string ultimaFecha = PlayerPrefs.GetString(KeyUltimaFecha);
            Debug.Log("Última vez que se abrió la aplicación: " + ultimaFecha);
        }
        else
        {
            Debug.Log("Es la primera vez que abres la aplicación.");
        }

        PlayerPrefs.SetString(KeyUltimaFecha, fechaActual.ToString("yyyy-MM-dd HH:mm:ss"));

        //Debug.Log("Cnaitdad de motivadores:"+PlayerPrefs.GetInt(keyCantidadMotivadores));
        CargarInformaciónMotivadores();
        PlayerPrefs.Save(); // Asegura que los datos se guarden en disco
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
        PlayerPrefs.SetInt(keyCantidadMotivadores, CaracteresMotivadoresEnSistema.Count);
        ListaMotivadoresData DataPorGuardar = new ListaMotivadoresData();

        foreach (Dinosaurio motivador in CaracteresMotivadoresEnSistema) // para cada motivador existente en la aplciación
        {
            DinosaurioData motivadorDataPorGuardar = new DinosaurioData(); // creamos un cartucho para guardar la información
            motivadorDataPorGuardar.Nombre = motivador.Nombre;
            motivadorDataPorGuardar.Emocionalidad = motivador.Emocionalidad;
            motivadorDataPorGuardar.position = motivador.gameObject.transform.position;
            DataPorGuardar.motivadoresData.Add(motivadorDataPorGuardar);

            if(motivador.exigencias != null)
            {
                foreach (Exigencia exigencia in motivador.exigencias) // para cada exigencia que tenga el motivador
                {
                   
                    if (exigencia is ExigenciaTiempoProductivo) //si la exigencia que estamos guardando es de tipo Tiempo productivo
                    {
                        ExigenciaTiempoProductivo exigenciaTiempoProductivo = exigencia as ExigenciaTiempoProductivo; // Convertimos la exigencia original
                        if (exigenciaTiempoProductivo != null)
                        {
                            // Creamos una instancia específica para este tipo
                            ExigenciaTiempoProductivoData exigenciaTiempoProductivoDataPorGuardar = new ExigenciaTiempoProductivoData();

                            exigenciaTiempoProductivoDataPorGuardar.ProgresoMeta = exigenciaTiempoProductivo.ProgresoMeta;
                            exigenciaTiempoProductivoDataPorGuardar.MetaTiempoProductivo = exigenciaTiempoProductivo.metaTiempoProductivo;
                            exigenciaTiempoProductivoDataPorGuardar.dificultadValor = (int)exigenciaTiempoProductivo.dificultad;

                            // Agregamos al motivador
                            motivadorDataPorGuardar.Exigencias.Add(exigenciaTiempoProductivoDataPorGuardar);
                        }

                    }
                    else // solo soportar guardar para un el tipo ExigenciaTiempoProductivoData
                    {
                        //ExigenciaData exigenciaPorGuardar = new ExigenciaData(); //creamos un cartucho para guardarlo

                        //exigenciaPorGuardar.dificultadValor = (int)exigencia.dificultad;

                        //motivadorDataPorGuardar.Exigencias.Add(exigenciaPorGuardar);
                    }



                }
            }
            
        }

        string json = JsonUtility.ToJson(DataPorGuardar, true);

        System.IO.File.WriteAllText("lista_motivadores_data", json);
        PlayerPrefs.Save();
    }

    public void CargarInformaciónMotivadores()
    {
        if (PlayerPrefs.GetInt(keyCantidadMotivadores) != 0)
        {
            List<DinosaurioData> motivadoresCargados = new List<DinosaurioData>();
            if (System.IO.File.Exists("lista_motivadores_data"))
            {
                string json = System.IO.File.ReadAllText("lista_motivadores_data");
                ListaMotivadoresData lista = JsonUtility.FromJson<ListaMotivadoresData>(json);
                motivadoresCargados = lista.motivadoresData;
            }
            CaracteresMotivadoresEnSistema.Clear();
            foreach (DinosaurioData motivadorCargado in motivadoresCargados) // para cada motivador guardado en los archivos
            {
                GameObject MotivadorNuevo = Instantiate(MotivadorPrefab); // creamos un motivador nuevo
                Dinosaurio componenteDinosaurioNuevo = MotivadorNuevo.GetComponent<Dinosaurio>(); //extraemos su compoenente
                if (componenteDinosaurioNuevo != null) // confirmamos que este componeente del nuevo moivador existe existe
                {
                    if (motivadorCargado != null)
                    {
                        // Le brindamos todas las propiedades del nuevo motivador del motivador guaraddo en archivos
                        componenteDinosaurioNuevo.Nombre = motivadorCargado.Nombre;
                        componenteDinosaurioNuevo.Emocionalidad = motivadorCargado.Emocionalidad;
                        componenteDinosaurioNuevo.gameObject.transform.position = motivadorCargado.position;
                    }           
                    List<Exigencia> ExigenciasDinosaurioNuevo = componenteDinosaurioNuevo.exigencias;

                    foreach (ExigenciaData exigenciaCargada in motivadorCargado.Exigencias) // para cada exigencia guradado en el motivador guardado en archivos
                    {
                        switch (exigenciaCargada.dificultadValor) // comprobamos que tipo de dificultad tiene para saignarle a su respectivo exigencia del nuevo motivador cargado
                        {

                            case (int)Exigencia.Dificultad.facil:
                                if (exigenciaCargada is ExigenciaTiempoProductivoData) // la exigencia guardad esde tipo tiempo productivo?
                                {
                                    ExigenciaTiempoProductivo ExigenciasTiempoProductivoDinosaurioNuevo = ExigenciasDinosaurioNuevo[0] as ExigenciaTiempoProductivo; //convertimos la exigencia del dinosaurio nuevo en una exigencia especificamente del tipo de tiempo productivo
                                    ExigenciaTiempoProductivoData ExigenciaCargadaTiempoProductivo = exigenciaCargada as ExigenciaTiempoProductivoData;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.dificultad = (Exigencia.Dificultad)ExigenciaCargadaTiempoProductivo.dificultadValor;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.metaTiempoProductivo = ExigenciaCargadaTiempoProductivo.MetaTiempoProductivo;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.progresoMeta = ExigenciaCargadaTiempoProductivo.ProgresoMeta;
                                    
                                }           

                                break;
                            case (int)Exigencia.Dificultad.moderado:
                                if (exigenciaCargada is ExigenciaTiempoProductivoData) // la exigencia guardad esde tipo tiempo productivo?
                                {
                                    ExigenciaTiempoProductivo ExigenciasTiempoProductivoDinosaurioNuevo = ExigenciasDinosaurioNuevo[1] as ExigenciaTiempoProductivo; //convertimos la exigencia del dinosaurio nuevo en una exigencia especificamente del tipo de tiempo productivo
                                    ExigenciaTiempoProductivoData ExigenciaCargadaTiempoProductivo = exigenciaCargada as ExigenciaTiempoProductivoData;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.dificultad = (Exigencia.Dificultad)ExigenciaCargadaTiempoProductivo.dificultadValor;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.metaTiempoProductivo = ExigenciaCargadaTiempoProductivo.MetaTiempoProductivo;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.progresoMeta = ExigenciaCargadaTiempoProductivo.ProgresoMeta;
                                }
                                break;
                            case (int)Exigencia.Dificultad.dificil:
                                if (exigenciaCargada is ExigenciaTiempoProductivoData) // la exigencia guardad esde tipo tiempo productivo?
                                {
                                    ExigenciaTiempoProductivo ExigenciasTiempoProductivoDinosaurioNuevo = ExigenciasDinosaurioNuevo[2] as ExigenciaTiempoProductivo; //convertimos la exigencia del dinosaurio nuevo en una exigencia especificamente del tipo de tiempo productivo
                                    ExigenciaTiempoProductivoData ExigenciaCargadaTiempoProductivo = exigenciaCargada as ExigenciaTiempoProductivoData;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.dificultad = (Exigencia.Dificultad)ExigenciaCargadaTiempoProductivo.dificultadValor;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.metaTiempoProductivo = ExigenciaCargadaTiempoProductivo.MetaTiempoProductivo;
                                    ExigenciasTiempoProductivoDinosaurioNuevo.progresoMeta = ExigenciaCargadaTiempoProductivo.ProgresoMeta;
                                }
                                break;



                        }
                    }

                
                }

               

            }


        }



    }

    private void OnApplicationQuit()
    {
        GuardarInformarciónMotivadores();
    }

}


#region Data
[System.Serializable]
public class ListaMotivadoresData
{
    [SerializeField] public List<DinosaurioData> motivadoresData = new List<DinosaurioData>();
}

[System.Serializable]
public class DinosaurioData
{
    public string Nombre;
    public float Emocionalidad;
    public Vector3 position;
    public List<ExigenciaTiempoProductivoData> Exigencias = new List<ExigenciaTiempoProductivoData>();
}
[System.Serializable]
public class ExigenciaData
{
    public int dificultadValor;

}


[System.Serializable]
public class ExigenciaTiempoProductivoData : ExigenciaData
{

    public float MetaTiempoProductivo = 0;
    public float ProgresoMeta = 0;


}
#endregion