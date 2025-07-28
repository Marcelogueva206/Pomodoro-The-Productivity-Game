using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class EstadisticasManager : MonoBehaviour
{

    // Singleton Instance
    public static EstadisticasManager Instance { get; private set; }
    [SerializeField] private GameObject MotivadorPrefab;

    public string keyCantidadMotivadores = "CantidadMotivadores"; //No cambiar de nombres, ya que estoy usando otra istancia sin referencia

    #region Sistema de integración de motivadores
    [SerializeField] private List<Dinosaurio> CaracteresMotivadoresEnSistema;
    public List<Dinosaurio> getCaracteresMotivadoresEnSistema()
    {
        return CaracteresMotivadoresEnSistema;
    }

   public TMP_Text textoRangoPerteneciente;
   public TMP_Text textoTiempoPromedioProductivo;
    public UnityEngine.UI.Image medallaImagenUI;


    [Header("Imagenes de las medallas")]
    public Sprite medallaPlatino;
    public Sprite medallaOro;
    public Sprite medallaPlata;
    public Sprite medallaBronce;
    public Sprite SinMedalla;

    public void ActualizarTextoRangoYPrimedioUI()
    {
        TimeSpan promedioActual = TimeSpan.FromHours(FindObjectOfType<ProductividadManager>().CalcularPromedio());

        float RangoPorcentaje = CalcularTop((float)promedioActual.TotalHours);
        string RangoTexto = "Top " + RangoPorcentaje.ToString("F1") + "%";

        // Determinar el rango de medalla basado en el promedio diario
        if (RangoPorcentaje <= 5f)
        {
            medallaImagenUI.sprite = medallaPlatino;// Platino
        }
        else if (RangoPorcentaje <= 10f)
        {
            medallaImagenUI.sprite = medallaOro;// Oro
        }
        else if (RangoPorcentaje <= 30f)
        {
            medallaImagenUI.sprite = medallaPlata;// Plata
        }
        else if (RangoPorcentaje <= 50f)
        {
            medallaImagenUI.sprite = medallaBronce;
        }
        else
        {
            medallaImagenUI.sprite = SinMedalla;
        }

        // Mostrar la medalla y el promedio diario en el texto de rango
        textoRangoPerteneciente.text = RangoTexto;
        textoTiempoPromedioProductivo.text = FormatearTiempo(promedioActual);
    }

    private float CalcularTop(float horas)
    {
        return 100f * Mathf.Exp(-0.6f * horas);
    }
    public static string FormatearTiempo(TimeSpan tiempo)
    {
        int horas = tiempo.Hours;
        int minutos = tiempo.Minutes;

        return $"{horas} hora{(horas != 1 ? "s" : "")} con {minutos} minuto{(minutos != 1 ? "s" : "")}";
    }
    public void AñadirCaracterMotivadorAlSistema(Dinosaurio motivador)
    {

        if (!CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Add(motivador);
            //StartCoroutine(GestorMetas.Instance.EsperarParaIniciarGeneraciónDeMetas());
        }

        GuardarInformarciónMotivadores();
    }
    public void EliminarCaracterMotivadorDelSistema(Dinosaurio motivador)
    {
        if (CaracteresMotivadoresEnSistema.Contains(motivador))
        {
            CaracteresMotivadoresEnSistema.Remove(motivador);
            //StartCoroutine(GestorMetas.Instance.EsperarParaIniciarGeneraciónDeMetas());
        }

    }
    #endregion


    public DateTime fechaUltimaVezSesionIniciada;

    private const string KeyUltimaFecha = "UltimaFecha";

    void OnApplicationQuit()
    {
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
        PlayerPrefs.SetString("UltimaActualizacion", DateTime.Now.ToString()); //HAY UN ERROR SOBRE LAS FECHAS QUE DEBES CORREGIR

    }
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
        //foreach (Dinosaurio motivador in CaracteresMotivadoresEnSistema)
        //{
        //    motivador.AplicarDepresionPorTiempo(ObtenerTiempoTranscurrido());
        //}

        PlayerPrefs.Save(); // Asegura que los datos se guarden en disco

        // Ejemplo de uso:
        // Registrar 1 hora productiva para hoy
        GuardarTiempoTotalProductivoPorVida();

        // Registrar 3 horas productivas para otro día
        GuardarTiempoTotalProductivoPorVida();

        // Obtener el promedio como TimeSpan
        //TimeSpan promedio = ObtenerPromedioProductividad();
        //Debug.Log("Promedio de tiempo productivo: " + promedio);

        // Reiniciar datos si es necesario
        // ReiniciarDatosProductividad();


        GuardarTiempoTotalProductivoPorVida();
        
    }
    private float intervaloGuardado = 30f; // Ajustable: 30s - 60s
    private float tiempoTranscurrido = 0f;

    private void Update()
    {
        tiempoTranscurrido += Time.deltaTime;

        if (tiempoTranscurrido >= intervaloGuardado)
        {
            GuardarInformarciónMotivadores();
            tiempoTranscurrido = 0f;
            
         
        }


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


        // Recuperar el tiempo total en horas desde PlayerPrefs
        CargarPromedioProductivoPorVida();

    }

    private void CargarPromedioProductivoPorVida()
    {
        float tiempoTotal = PlayerPrefs.GetFloat(TotalTiempoKey, 0f);

        // Crear un TimeSpan a partir del tiempo total en horas
        TiempoTotalProductivoPorVida = TimeSpan.FromSeconds((double)tiempoTotal);

        DiasEnTotalPorVida = PlayerPrefs.GetInt(DiasKey);

        if (ComprobarEsOtroDia())
        {
            DiasEnTotalPorVida++;
        }
    }

    [HideInInspector] public static TimeSpan TiempoTempoProductivoPromedio = new TimeSpan(0, 15, 0);
    [HideInInspector] public static TimeSpan TiempoTotalProductivoPorVida = new TimeSpan(0, 0, 0);
    private const string TotalTiempoKey = "TotalTiempoProductividad"; // Clave para el total de tiempo en PlayerPrefs
    public int DiasEnTotalPorVida = 0;
    private const string DiasKey = "DiasProductividad"; // Clave para la cantidad de días registrados en PlayerPrefs
    [HideInInspector] public static TimeSpan TiempoTotalProductivoHoy = new TimeSpan(0, 0, 0);
    public void GuardarTiempoTotalProductivoPorVida()
    {
        // Obtener los valores actuales guardados
        double totalTiempo = TiempoTotalProductivoPorVida.Seconds;
        int diasRegistrados = DiasEnTotalPorVida;

      

        // Guardar los valores actualizados
        PlayerPrefs.SetFloat(TotalTiempoKey, (float)totalTiempo);
        PlayerPrefs.SetInt(DiasKey, diasRegistrados);
        PlayerPrefs.Save();
    }
    //public TimeSpan ObtenerPromedioProductividad()
    //{
    //    double totalTiempo = PlayerPrefs.GetFloat(TotalTiempoKey, 0f);
    //    int diasRegistrados = PlayerPrefs.GetInt(DiasKey, 0);

    //    if (diasRegistrados == 0)
    //        return TimeSpan.Zero; // Si no hay días registrados, el promedio es 0

    //    double promedioHoras = totalTiempo / diasRegistrados;
    //    return TimeSpan.FromHours(promedioHoras);
    //}
    public void RegistrarTempoTerminado(Tempos tempoTerminado)
    {
        if(tempoTerminado.tiposTempos == TiposTempos.productivo)
        {
            TiempoTempoProductivoPromedio = (TiempoTempoProductivoPromedio + tempoTerminado.TiempoTotal) / 2f;
            TiempoTotalProductivoHoy += tempoTerminado.TiempoTotal;
            TiempoTotalProductivoPorVida += tempoTerminado.TiempoTotal;
            FindObjectOfType<ProductividadManager>().RegistrarHoras(tempoTerminado.TiempoTotal.TotalHours);
        } 
        ActualizarTextoRangoYPrimedioUI();


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
            motivadorDataPorGuardar.especie = motivador._Especie.ToString();
            motivadorDataPorGuardar.Emocionalidad = motivador.Emocionalidad;
            motivadorDataPorGuardar.rareza = motivador.Rareza.ToString();
            
      
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
                            exigenciaTiempoProductivoDataPorGuardar.completado = exigenciaTiempoProductivo.Completado; // Guardamos si la exigencia esta completada o no

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
                        componenteDinosaurioNuevo._Especie = (Especie)System.Enum.Parse(typeof(Especie), motivadorCargado.especie);
                        componenteDinosaurioNuevo.Rareza = (Rareza)System.Enum.Parse(typeof(Rareza), motivadorCargado.rareza);

                        if (componenteDinosaurioNuevo.gameObject.transform.position.z < 1)
                        {
                            componenteDinosaurioNuevo.gameObject.transform.position += new Vector3(0, 0, 1);
                        }
                        componenteDinosaurioNuevo.ComprobarEliminarPorDepresion(); // comprobamos si el motivador nuevo debe ser eliminado por depresión, caso contrario no debería eliminarse
                        componenteDinosaurioNuevo.AplicarDepresionPorTiempo(ObtenerTiempoTranscurrido()); //aplicamos la depresión por tiempo al motivador nuevo
                    }

                    if (ComprobarEsOtroDia()) //comprubea si es otro día para no cargarlo. caso contrario no debería cargar los respectivos datos
                    {
                        componenteDinosaurioNuevo.ReiniciarExigencias();

                    }else
                    {
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
                                        ExigenciasTiempoProductivoDinosaurioNuevo.Completado = exigenciaCargada.completado; // Guardamos si la exigencia esta completada o no

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
                                        ExigenciasTiempoProductivoDinosaurioNuevo.Completado = exigenciaCargada.completado; // Guardamos si la exigencia esta completada o no
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
                                        ExigenciasTiempoProductivoDinosaurioNuevo.Completado = exigenciaCargada.completado; // Guardamos si la exigencia esta completada o no
                                    }
                                    break;



                            }
                        }

                    }
                   

                
                }

               

            }


        }



    }
    private void GuardarUltimaActualizacion()
    {
        // Guarda la hora actual como la última vez que se cerró la aplicación
        PlayerPrefs.SetString("UltimaActualizacion", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
    public bool ComprobarEsOtroDia()
    {
        //hecho para ejectuarse una vez
        string ultimaActualizacionStr = PlayerPrefs.GetString("UltimaActualizacion", DateTime.Now.ToString());
        DateTime ultimaActualizacion = DateTime.Parse(ultimaActualizacionStr);

        if (ultimaActualizacion.Month == DateTime.Now.Month && ultimaActualizacion.Year == DateTime.Now.Year)
        {
            if (DateTime.Now.Day - ultimaActualizacion.Day == 0)
            {
                return false;
            }

        }

        return true;

    }
    public float ObtenerTiempoTranscurrido()
    {
        // Recupera la última vez que se guardó el tiempo, si no existe, devuelve 0 segundos
        string ultimaActualizacionStr = PlayerPrefs.GetString("UltimaActualizacion", DateTime.Now.ToString());
        DateTime ultimaActualizacion = DateTime.Parse(ultimaActualizacionStr);

        // Calcula el tiempo transcurrido
        TimeSpan tiempoTranscurrido = DateTime.Now - ultimaActualizacion;

        return (float)tiempoTranscurrido.TotalSeconds;
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
    public string especie;
    public string rareza;
    public List<ExigenciaTiempoProductivoData> Exigencias = new List<ExigenciaTiempoProductivoData>();
}
[System.Serializable]
public class ExigenciaData
{
    public int dificultadValor;
    public bool completado;


}


[System.Serializable]
public class ExigenciaTiempoProductivoData : ExigenciaData
{

    public float MetaTiempoProductivo = 0;
    public float ProgresoMeta = 0;
    


}
#endregion