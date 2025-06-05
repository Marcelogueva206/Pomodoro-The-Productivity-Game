using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GestorMetas : MonoBehaviour
{
    // Propiedad estática para acceder a la instancia
    public static GestorMetas Instance { get; private set; }

    // Campo privado para controlar la variable TotalMinutosDeExigencias
    [HideInInspector] private float TotalMinutosDeExigencias = 0;
    [HideInInspector] private float MinimoMinutosDeExigencias = 0;

    public MetaProgresoRecompensa MetaMinima;
    [HideInInspector] private float ValorMetaMinima;
    public MetaProgresoRecompensa MetaDeIntermedio;
    [HideInInspector] private float ValorMetaDeIntermedio;
    [HideInInspector] private Vector3 IncrementoDeMetaIntermedio = new Vector3(5, 15);
    public MetaProgresoRecompensa MetaDeSuperacion;
    [HideInInspector] private float ValorMetaDeSuperacion;
    [HideInInspector] private Vector3 IncrementoDeMetaSuperacion = new Vector3(10, 30);

    public List<MarcaBasicaRecompensa> marcasSimples;

    public MarcaBasicaRecompensa Marca0Porciento;
    public MarcaBasicaRecompensa Marca10Porciento;
    public MarcaBasicaRecompensa Marca20Porciento;
    public MarcaBasicaRecompensa Marca30Porciento;
    public MarcaBasicaRecompensa Marca40Porciento;
    public MarcaBasicaRecompensa Marca50Porciento;
    public MarcaBasicaRecompensa Marca60Porciento;
    public MarcaBasicaRecompensa Marca70Porciento;
    public MarcaBasicaRecompensa Marca80Porciento;
    public MarcaBasicaRecompensa Marca90Porciento;
    public MarcaBasicaRecompensa Marca100Porciento;


    private void Awake()
    {

        Marca0Porciento = new MarcaBasicaRecompensa("Marca del mínimo esfuerzo", 1f);
        Marca10Porciento = new MarcaBasicaRecompensa("Marca del 10%", 10f);
        Marca20Porciento = new MarcaBasicaRecompensa("Marca del 20%", 20f);
        Marca30Porciento = new MarcaBasicaRecompensa("Marca del 30%", 30f);
        Marca40Porciento = new MarcaBasicaRecompensa("Marca del 40%", 40f);
        Marca50Porciento = new MarcaBasicaRecompensa("Marca de la mitad de progreso", 50f);
        Marca60Porciento = new MarcaBasicaRecompensa("Marca del 60%", 60f);
        Marca70Porciento = new MarcaBasicaRecompensa("Marca del 70%", 70f);
        Marca80Porciento = new MarcaBasicaRecompensa("Marca del 80%", 80f);
        Marca90Porciento = new MarcaBasicaRecompensa("Marca del 90%", 90f);
        Marca100Porciento = new MarcaBasicaRecompensa("Marca del éxito", 100f);
        marcasSimples = new List<MarcaBasicaRecompensa> { Marca0Porciento, Marca10Porciento, Marca20Porciento, Marca30Porciento, Marca40Porciento, Marca50Porciento, Marca60Porciento, Marca70Porciento, Marca80Porciento, Marca90Porciento, Marca100Porciento };

        MetaMinima = new MetaProgresoRecompensa("Primera meta de la productividad", 5, TipoMeta.MetaMinima);
        MetaDeIntermedio = new MetaProgresoRecompensa("Segunda meta de la productividad", 10, TipoMeta.MetaIntermedia);
        MetaDeSuperacion = new MetaProgresoRecompensa("Tercera máxima meta de la productividad", 15, TipoMeta.MetaDeSuperación);

        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye instancias adicionales
            return;
        }
        Instance = this;



        DontDestroyOnLoad(gameObject); // Opcional: persiste entre escenas        DontDestroyOnLoad(gameObject); // Opcional: persiste entre escenas
        // Inicializa las metas diarias al iniciar el juego
        //RegenerarMetasDiarias();

       
    }

    public void GuardarMetas()
    {
        PlayerPrefs.SetFloat("MetaMinima", ValorMetaMinima);
        PlayerPrefs.SetFloat("MetaIntermedia", ValorMetaDeIntermedio);
        PlayerPrefs.SetFloat("MetaSuperacion", ValorMetaDeSuperacion);
        //PlayerPrefs.SetString("UltimaActualizacion", DateTime.Now.ToString());
        PlayerPrefs.SetString("UltimaActualizacion", DateTime.Now.ToString()); // <-- aquí
        PlayerPrefs.Save(); // Asegura que los datos se guarden inmediatamente
    }

    public void CargarMetas()
    {
        if (PlayerPrefs.HasKey("MetaMinima"))
        {
            ValorMetaMinima = PlayerPrefs.GetFloat("MetaMinima");
            ValorMetaDeIntermedio = PlayerPrefs.GetFloat("MetaIntermedia");
            ValorMetaDeSuperacion = PlayerPrefs.GetFloat("MetaSuperacion");
        }
        else
        {
            // Si no hay datos guardados, inicializa con valores predeterminados
            ValorMetaMinima = 5;
            ValorMetaDeIntermedio = 10;
            ValorMetaDeSuperacion = 15;
        }
        //

        MetaMinima.SetNombre("Primera meta de la productividad");
        MetaMinima.SetPorcentaje(Mathf.FloorToInt(GetMetaMinimaPor()));
        MetaMinima.SetCompletado(false);

        MetaDeIntermedio.SetNombre("Segunda meta de la productividad");
        MetaDeIntermedio.SetPorcentaje(Mathf.FloorToInt(GetMetaDeIntermedioPor()));
        MetaDeIntermedio.SetCompletado(false);

        MetaDeSuperacion.SetNombre("Tercera máxima meta de la productividad");
        MetaDeSuperacion.SetPorcentaje(Mathf.FloorToInt(GetMetaDeSuperacionPor()));
        MetaDeSuperacion.SetCompletado(false);
    }

    private void Start() //es muy probableque haya un error, ya que los motivadores se implementan en el sistema ambién en el start
    {
        CargarMetas();
      
        // Verifica si las metas deben regenerarse
        if (MetasNoInicializadas() || ComprobarEsOtroDia())
        {
            Debug.Log("Regenerando metas porque no están inicializadas o es un nuevo día.");
            StartCoroutine(EsperarParaIniciarGeneraciónDeMetas());
        }
        else
        {
            Debug.Log("Cargando metas guardadas porque es el mismo día.");
        }



    }
    // Método para regenerar las metas diarias
    public void RegenerarMetasDiarias()
    {
        if (MetasNoInicializadas() || ComprobarEsOtroDia())
        {
            if (EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count > 0)
            {
                TotalMinutosDeExigencias = 0;
                MinimoMinutosDeExigencias = 0;
                foreach (Dinosaurio motivador in EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema())
                {
                    TotalMinutosDeExigencias += motivador.GetTiempoTotalExigido();
                    MinimoMinutosDeExigencias += motivador.GetMinimoSostenible();
                }

                ValorMetaMinima = MinimoMinutosDeExigencias;

                float rangoAleatorioDeIncrementoDeMetaIntermedio = UnityEngine.Random.Range(IncrementoDeMetaIntermedio.x, IncrementoDeMetaIntermedio.y);
                int cantidadDeMotivadores = EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count;

                ValorMetaDeIntermedio = MinimoMinutosDeExigencias + cantidadDeMotivadores * rangoAleatorioDeIncrementoDeMetaIntermedio;

                float rangoAleatorioDeIncrementoDeMetaDeSuperación = UnityEngine.Random.Range(IncrementoDeMetaSuperacion.x, IncrementoDeMetaSuperacion.y);
                ValorMetaDeSuperacion = ValorMetaDeIntermedio + cantidadDeMotivadores * rangoAleatorioDeIncrementoDeMetaDeSuperación;
            }
            else
            {
                Debug.LogWarning("[CargarMetas] No se encontraron caracteresMotivadores. Usando valores predeterminados.");
                ValorMetaMinima = 5;
                ValorMetaDeIntermedio = 10;
                ValorMetaDeSuperacion = 15;
            }

            MetaMinima.SetNombre("Primera meta de la productividad");
            MetaMinima.SetPorcentaje(Mathf.FloorToInt(GetMetaMinimaPor()));
            MetaMinima.SetCompletado(false);

            MetaDeIntermedio.SetNombre("Segunda meta de la productividad");
            MetaDeIntermedio.SetPorcentaje(Mathf.FloorToInt(GetMetaDeIntermedioPor()));
            MetaDeIntermedio.SetCompletado(false);

            MetaDeSuperacion.SetNombre("Tercera máxima meta de la productividad");
            MetaDeSuperacion.SetPorcentaje(Mathf.FloorToInt(GetMetaDeSuperacionPor()));
            MetaDeSuperacion.SetCompletado(false);

            GuardarMetas();
        }
        PlayerPrefs.SetString("UltimaActualizacion", DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();

    }
    
    public MetaProgresoRecompensa GetMetaPorTipo(TipoMeta tipoMeta)
    {
        foreach (MetaProgresoRecompensa MetaRecompensa in new MetaProgresoRecompensa[] { GestorMetas.Instance.MetaMinima, GestorMetas.Instance.MetaDeIntermedio, GestorMetas.Instance.MetaDeSuperacion })
        {
            if (MetaRecompensa.tiposMetas == tipoMeta) { return MetaRecompensa; }
        }

        return null;
    }

    private bool MetasNoInicializadas()
    {
        return ValorMetaMinima <= 0 || ValorMetaDeIntermedio <= 0 || ValorMetaDeSuperacion <= 0;
    }

    public bool ComprobarEsOtroDia()
    {
        ////hecho para ejectuarse una vez
        //string ultimaActualizacionStr = PlayerPrefs.GetString("UltimaActualizacion", DateTime.Now.ToString());
        //DateTime ultimaActualizacion = DateTime.Parse(ultimaActualizacionStr);

        //if (ultimaActualizacion.Month == DateTime.Now.Month && ultimaActualizacion.Year == DateTime.Now.Year)
        //{
        //    if (DateTime.Now.Day - ultimaActualizacion.Day == 0)
        //    {
        //        return false;
        //    }

        //}

        //return true;

        string ultimaActualizacionStr = PlayerPrefs.GetString("UltimaActualizacion", DateTime.Now.ToString());
        DateTime ultimaActualizacion;

        Debug.Log($"Última actualización guardada: {ultimaActualizacionStr}");
        // Intenta parsear la fecha guardada
        if (!DateTime.TryParse(ultimaActualizacionStr, out ultimaActualizacion))
        {
            // Si falla, fuerza regeneración de metas
            return true;
        }

        // Calcula la diferencia de días completos
        return (DateTime.Now.Date - ultimaActualizacion.Date).Days >= 1;
    }

    public float GetMetaMinimaPor()
    {
        return (ValorMetaMinima / ValorMetaDeSuperacion) * 100f;
    }
    public float GetMetaDeIntermedioPor()
    {
        return (ValorMetaDeIntermedio / ValorMetaDeSuperacion) * 100f;
    }
    public float GetMetaDeSuperacionPor()
    {
        return 100f;
    }

    public float GetMetaSuperaciónValor()
    {
        return ValorMetaDeSuperacion;
    }




    public IEnumerator EsperarParaIniciarGeneraciónDeMetas()
    {
        // Espera hasta que el objeto requerido no sea null y esté activo en la jerarquía.
        while (EstadisticasManager.Instance == null || ContadorProgreso.Instance == null || (EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count <= 0 /*&& PlayerPrefs.GetInt("CantidadMotivadores") == 0*/)) // posible errorr
        {
            Debug.Log("Esperando a que carge los elemntos necesarios para generar metas");
            yield return null; // Espera un frame.
        }

        Debug.Log("Carga terminada");
        RegenerarMetasDiarias();
    }

    public bool ConfirmarMotivadoresCompletamenteCargados()
    {
        foreach (Dinosaurio motivador in EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema())
        {
            foreach (Exigencia exigencia in motivador.exigencias)
            {
                if (exigencia == null)
                {
                    return false;
                }
            }
        }

        return true;
    }


}

public class MarcaBasicaRecompensa
{
    public string Nombre;
    protected bool Completado = false;
    protected bool Reclamado = false;
    public float PuntuacionRequerida { get => (PorcentajeRequeridoMeta / 100f) * GestorMetas.Instance.GetMetaSuperaciónValor(); set { } }
    protected float porcentajeRequeridoMeta;
    public float PorcentajeRequeridoMeta
    {
        get => porcentajeRequeridoMeta; set
        {

            if (value >= 0 && value <= 100f)
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

    public MarcaBasicaRecompensa(string nombre, float porcentajeRequerido)
    {
        this.Nombre = nombre;
        PorcentajeRequeridoMeta = porcentajeRequerido;

    }

    public event ProgresoMarcaUsuario PremioPorMarcaLograda = (marca) =>
    {

        Debug.Log($"¡Marca {marca.Nombre} logarado!:" + marca.PorcentajeRequeridoMeta + "%");
        SistemaRecompensa.Instancia.IntentoRuletaGanado();
    };


    public void SetPorcentajeRequeridoSegunPuntuacion(float puntuacion)
    {

    }

    public void SetCompletado(bool estado)
    {
   

        Completado = estado;
    }
    public void SetPorcentaje(float porcentaje)
    {
        porcentajeRequeridoMeta = porcentaje;
    }
    public void SetNombre(string nombre)
    {
        Nombre = nombre.Trim();
    }
    public void SetReclamado(bool estado)
    {
        Reclamado = estado;
    }

    public void ReclamarPremio()
    {
        if (Completado == true && Reclamado == false)
        {

            PremioPorMarcaLograda?.Invoke(this);
            Reclamado = true;
            ContadorProgreso.Instance.IntentarGuardarProgreso();

        }
        else
        {
            Debug.Log("El premio ya fue reclamado");
        }

    }

    public bool GetCompletado()
    {
        return Completado;
    }

    public bool GetReclamado()
    {
        return Reclamado;
    }


    private bool ComprobarEsOtroDia()
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
}

public enum TipoMeta { MetaMinima, MetaIntermedia, MetaDeSuperación }
public class MetaProgresoRecompensa : MarcaBasicaRecompensa
{
    public readonly TipoMeta tiposMetas;

    public MetaProgresoRecompensa(string nombre, float porcentajeRequerido, TipoMeta tipoMeta) : base(nombre, porcentajeRequerido)
    {
        this.tiposMetas = tipoMeta;
    }




}