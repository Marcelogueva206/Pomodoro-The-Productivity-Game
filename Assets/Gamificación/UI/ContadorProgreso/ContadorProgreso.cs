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
    private static ContadorProgreso _instance;
    public static ContadorProgreso Instance
    {
        get
        {
            if (_instance == null)
            {
                // Busca una instancia existente en la escena
                _instance = FindObjectOfType<ContadorProgreso>();

                if (_instance == null)
                {
                    // Crea una nueva instancia si no existe
                    GameObject singletonObj = new GameObject(nameof(ContadorProgreso));
                    _instance = singletonObj.AddComponent<ContadorProgreso>();
                }
            }
            return _instance;
        }
    }

    public float ProgresoUsuario { get => progresoUsuario; set { /*VerificarLogroDeEstrellas()*/; VerificarLogrosDeMarcas(); progresoUsuario = value; } }

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
        if (GestorMetas.Instance == null)
        {
            Debug.LogError("GestorMetas.Instance no está inicializado.");
            return;
        }

        if (GestorMetas.Instance.MetaMinima == null || GestorMetas.Instance.MetaDeIntermedio == null || GestorMetas.Instance.MetaDeSuperacion == null)
        {
            Debug.LogError("Una o más metas no están inicializadas.");
            return;
        }

        if (TodosLosElementosCargadosParActualizarMarcas())
        {
           

            if (GestorMetas.Instance.MetaMinima != null && GestorMetas.Instance.MetaDeIntermedio != null && GestorMetas.Instance.MetaDeSuperacion != null)
            {
                ActualizarMarca(cuerpoPrimeraMeta, GestorMetas.Instance.MetaMinima.PorcentajeRequeridoMeta, GestorMetas.Instance.MetaMinima.PuntuacionRequerida);
                ActualizarMarca(cuerpoSegundaMeta, GestorMetas.Instance.MetaDeIntermedio.PorcentajeRequeridoMeta, GestorMetas.Instance.MetaDeIntermedio.PuntuacionRequerida);
                ActualizarMarca(cuerpoTerceraMeta, GestorMetas.Instance.MetaDeSuperacion.PorcentajeRequeridoMeta, GestorMetas.Instance.MetaDeSuperacion.PuntuacionRequerida);

                ActualizarMarca(cuerpoMarca0, GestorMetas.Instance.Marca0Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca0Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca10, GestorMetas.Instance.Marca10Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca10Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca20, GestorMetas.Instance.Marca20Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca20Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca30, GestorMetas.Instance.Marca30Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca30Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca40, GestorMetas.Instance.Marca40Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca40Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca50, GestorMetas.Instance.Marca50Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca50Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca60, GestorMetas.Instance.Marca60Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca60Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca70, GestorMetas.Instance.Marca70Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca70Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca80, GestorMetas.Instance.Marca80Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca80Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca90, GestorMetas.Instance.Marca90Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca90Porciento.PuntuacionRequerida);
                ActualizarMarca(cuerpoMarca100, GestorMetas.Instance.Marca100Porciento.PorcentajeRequeridoMeta, GestorMetas.Instance.Marca100Porciento.PuntuacionRequerida);
            }
        }
        else
        {
            Debug.Log("elementos de para acutalizar marcas aún no cargados");
        }
        
      
       
    }

    private void ActualizarMarca(RectTransform marca, float porcentaje, float cantidad)
    {
        
        TextMeshProUGUI textoMarca = marca.GetComponentInChildren<TextMeshProUGUI>();

        // Solo actualiza el texto si el componente existe
        if (textoMarca != null)
        {
            textoMarca.text = Mathf.CeilToInt(cantidad).ToString(); // Siempre muestra la cantidad
        }

        if (porcentaje < 100f)
        {
            // Asegúrate de que el valor esté entre 0 y 1
            float normalizedPercentage = Mathf.Clamp(porcentaje / 100f, 0f, 1f);

            // Calcula la posición del marcador en el ancho de la barra, calibrado para empezar desde 0%
            float markerPositionX = normalizedPercentage * sliderBarraProgreso.GetComponent<RectTransform>().rect.width;

            // Actualiza la posición del marcador, considerando el ancho de la barra
            marca.anchoredPosition = new Vector2(markerPositionX - (sliderBarraProgreso.GetComponent<RectTransform>().rect.width * 0.5f), marca.anchoredPosition.y);
        }


    }
       

    private void Awake()
    {
        // Asegúrate de que solo exista una instancia
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); // Destruye duplicados
            return;
        }

        _instance = this;
 
        sliderBarraProgreso = barraProgreso.GetComponent<Slider>();

        PomodoroSistema.TemposTerminado += ActualizarProgresoEnEvento;
        PomodoroSistema.TemposTerminado += ActualizarMostrarReclamarRecompensa;
       

    }

    private void Start()
    {
        if (!ComprobarEsOtroDia())
        {
            IntentarCargarProgreso();
        }

        StartCoroutine(EsperarYActualizarRecompensas());
       
    }
    void Update()
    {
        ActualizarProgresoEnEvento(null);
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

    }

    public void ActualizarProgresoEnEvento(Tempos tempos)
    {

        ProgresoUsuario = (float)Gamificacion.Instance.ProgresoTotalMetaDiarioPor / 100f;
    }

    //void OnApplicationQuit()
    //{
    //    IntentarGuardarProgreso();
    //}
    //void OnApplicationPause()
    //{
    //    IntentarGuardarProgreso();
    //}


    private void VerificarLogrosDeMarcas()
    {
        foreach (MarcaBasicaRecompensa marca in GestorMetas.Instance.marcasSimples)
        {
            if (marca.PorcentajeRequeridoMeta < progresoUsuario)
            {
                marca.SetCompletado(true);
            }
            else
            {
                return;
            }
        }
    }

    public void IntentarGuardarProgreso(Tempos tempo)
    {
        StartCoroutine(EsperarYGuardarProgreso());

    }

    private IEnumerator EsperarYGuardarProgreso()
    {
        // Esperar hasta que GestorMetas.Instance y sus metas estén inicializados
        while (GestorMetas.Instance == null ||
               GestorMetas.Instance.MetaMinima == null ||
               GestorMetas.Instance.MetaDeIntermedio == null ||
               GestorMetas.Instance.MetaDeSuperacion == null)
        {
            yield return null; // Esperar un frame
        }

        // Una vez inicializado, ejecutar GuardarProgreso
        GuardarProgreso();
    }

    public void GuardarProgreso()
    {
        foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
        {
            if (MetaRecompensa.GetCompletado() == true)
            {
                PlayerPrefs.SetInt($"Completado" + MetaRecompensa.tiposMetas.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("Completado" + MetaRecompensa.tiposMetas.ToString(), 0);
            }

            if (MetaRecompensa.GetReclamado() == true)
            {
                PlayerPrefs.SetInt($"Reclamado" + MetaRecompensa.tiposMetas.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("Reclamado" + MetaRecompensa.tiposMetas.ToString(), 0);
            }
        }


    }

    public void ActualizarMostrarReclamarRecompensa(Tempos tempos) ///
    {
        foreach(RectTransform cuerpoMarcadorMeta in new RectTransform[] { cuerpoPrimeraMeta , cuerpoSegundaMeta, cuerpoTerceraMeta })
        {
            cuerpoMarcadorMeta.gameObject.GetComponent<LogicaMarcaMeta>().IntentarMostrarReclamarRecompensa();
          

        }

    }

    private IEnumerator EsperarYActualizarRecompensas()
    {
        // Esperar hasta que GestorMetas.Instance y las metas estén inicializadas
        while (GestorMetas.Instance == null ||
               GestorMetas.Instance.MetaMinima == null ||
               GestorMetas.Instance.MetaDeIntermedio == null ||
               GestorMetas.Instance.MetaDeSuperacion == null)
        {
            yield return null; // Esperar un frame
        }

        yield return new WaitForSeconds(1f);
        // Ejecutar el método una vez que las metas estén inicializadas
        ActualizarMostrarReclamarRecompensa(null);
    }

    public void IntentarCargarProgreso()
    {
        StartCoroutine(EsperarYCargarProgreso());
    }

    private IEnumerator EsperarYCargarProgreso()
    {
        // Esperar hasta que GestorMetas.Instance y sus metas estén inicializados
        while (GestorMetas.Instance == null ||
               GestorMetas.Instance.MetaMinima == null ||
               GestorMetas.Instance.MetaDeIntermedio == null ||
               GestorMetas.Instance.MetaDeSuperacion == null)
        {
            yield return null; // Esperar un frame
        }

        // Una vez inicializado, ejecutar CargarProgreso
        CargarProgreso();
    }
    public void CargarProgreso()
    {
        foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
        {

            // Verificar y asignar valor para "Completado"
            bool valorGuardadoCompletado = false;
            string keyCompletado = $"Completado" + MetaRecompensa.tiposMetas.ToString();

            if (PlayerPrefs.HasKey(keyCompletado))
            {
                valorGuardadoCompletado = PlayerPrefs.GetInt(keyCompletado) == 1;
            }
            else
            {
                // Valor por defecto si la clave no existe
                valorGuardadoCompletado = false;
            }

            MetaRecompensa.SetCompletado(valorGuardadoCompletado);

            // Verificar y asignar valor para "Reclamado"
            bool valorGuardadoReclamado = false;
            string keyReclamado = $"Reclamado" + MetaRecompensa.tiposMetas.ToString();

            if (PlayerPrefs.HasKey(keyReclamado))
            {
                valorGuardadoReclamado = PlayerPrefs.GetInt(keyReclamado) == 1;
            }
            else
            {
                // Valor por defecto si la clave no existe
                valorGuardadoReclamado = false;
            }

            MetaRecompensa.SetReclamado(valorGuardadoReclamado);

        }
        MostrarProgresoUI();
        ContadorProgreso.Instance.ActualizarMostrarReclamarRecompensa(null);
    }
    public void VerificarLogroDeEstrellas()
    {

        Debug.Log("Verififación de estrellas...");
        ActualizarProgresoEnEvento(null);

        if (GestorMetas.Instance.MetaMinima != null&& GestorMetas.Instance.MetaDeIntermedio != null&& GestorMetas.Instance.MetaDeSuperacion != null)
        {
            //Debug.Log($"Progreso usuario {progresoUsuario}");
            //foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
            //{
            //    Debug.Log($"Porcentaje requerido: {MetaRecompensa.PorcentajeRequeridoMeta} para la meta {MetaRecompensa.tiposMetas}  ");
            //    if ((MetaRecompensa.PorcentajeRequeridoMeta / 100f) < progresoUsuario)
            //    {
            //        Debug.Log("Gestor: " + GestorMetas.Instance.MetaMinima.GetHashCode());
            //        Debug.Log("Local : " + MetaRecompensa.GetHashCode());

            //        MetaRecompensa.SetCompletado(true);
            //    }
            //    else
            //    {
            //        return;
            //    }


            //}
         

            foreach (var MetaRecompensa in new[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
            {
                bool logroCumplido = (MetaRecompensa.PorcentajeRequeridoMeta / 100f) <= progresoUsuario;

                Debug.Log($"Meta {MetaRecompensa.tiposMetas} - progreso usuario: {progresoUsuario} - necesita: {MetaRecompensa.PorcentajeRequeridoMeta / 100f} - cumplido: {logroCumplido}");

                MetaRecompensa.SetCompletado(logroCumplido);
            }
        }
        else
        {
            Debug.Log("Verififación de estrellas ha fallado");
        }

        ActualizarMostrarReclamarRecompensa(null);

    }

    //private void InvocarEventoParaRecompensa(int index, float progresoLogrado)
    //{
    //    if (RecompensasObtenidasProgresoMeta[index] == false)
    //    {
    //        PremioPorProgresoUsuario.Invoke(progresoLogrado);
    //        RecompensasObtenidasProgresoMeta[index] = true;
    //    }
    //}

    public void MostrarProgresoUI()
    {
        TextoPPsAcumulados.text = ((int)Gamificacion.Instance.TiempoAcumuladoHoy).ToString();
        TextoPPsTotales.text = ((int)Gamificacion.Instance.TiempoTotalAcumulado).ToString();
        sliderBarraProgreso.value = ProgresoUsuario;
        Debug.Log($"PuntuacionTempo: {Contador.PuntuacionTempo}");
        Debug.Log($"MetaSuperacion: {GestorMetas.Instance.GetMetaSuperaciónValor()}");


        float metaSuperacion = GestorMetas.Instance.GetMetaSuperaciónValor();
        float progresoExtra = 0f;

        if (metaSuperacion != 0f&& Contador.FaseActual != Contador.FasesContador.Inicio)
        {
            progresoExtra = Contador.PuntuacionTempo / metaSuperacion;
        }

        sliderBarraProgresoAcumulado.value = ProgresoUsuario + progresoExtra;

        //sliderBarraProgresoAcumulado.value = ProgresoUsuario + (Contador.PuntuacionTempo / GestorMetas.Instance.GetMetaSuperaciónValor());
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
    private bool TodosLosElementosCargadosParActualizarMarcas()
    {
        var gm = GestorMetas.Instance;

        return gm != null &&
               gm.MetaMinima != null &&
               gm.MetaDeIntermedio != null &&
               gm.MetaDeSuperacion != null &&
               gm.Marca0Porciento != null &&
               gm.Marca10Porciento != null &&
               gm.Marca20Porciento != null &&
               gm.Marca30Porciento != null &&
               gm.Marca40Porciento != null &&
               gm.Marca50Porciento != null &&
               gm.Marca60Porciento != null &&
               gm.Marca70Porciento != null &&
               gm.Marca80Porciento != null &&
               gm.Marca90Porciento != null &&
               gm.Marca100Porciento != null;
    }

}







