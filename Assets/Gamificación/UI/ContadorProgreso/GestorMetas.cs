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
    [HideInInspector] private Vector3  IncrementoDeMetaIntermedio = new Vector3 (5, 15);
    public MetaProgresoRecompensa MetaDeSuperacion;
    [HideInInspector] private float ValorMetaDeSuperacion;
    [HideInInspector] private Vector3  IncrementoDeMetaSuperacion = new Vector3 (10 , 30);

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

        contadorProgreso = GameObject.Find("Dominio Usuario Panel").GetComponentInChildren<ContadorProgreso>();

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



        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye instancias adicionales
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Opcional: persiste entre escenas
    }

    private void Start() //es muy probableque haya un error, ya que los motivadores se implementan en el sistema ambién en el start
    {
        

        StartCoroutine(EsperarParaIniciarGeneraciónDeMetas());
    }

    public void RegenerarMetasDiarias()
    {


        if(EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count != 0)
        {
            TotalMinutosDeExigencias = 0;
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
            ValorMetaMinima = 5;
            ValorMetaDeIntermedio = 10;
            ValorMetaDeSuperacion = 15;



        }
        MetaMinima = new MetaProgresoRecompensa("Primera meta de la productividad", Mathf.FloorToInt(GetMetaMinimaPor()), TipoMeta.MetaMinima);
        MetaDeIntermedio = new MetaProgresoRecompensa("Segunda meta de la productividad", Mathf.FloorToInt(GetMetaDeIntermedioPor()), TipoMeta.MetaIntermedia);
        MetaDeSuperacion = new MetaProgresoRecompensa("Tercera máxima meta de la productividad", Mathf.FloorToInt(GetMetaDeSuperacionPor()), TipoMeta.MetaDeSuperación);


    }

    public float GetMetaMinimaPor()
    {
        return (ValorMetaMinima/ValorMetaDeSuperacion)*100;
    }
    public float GetMetaDeIntermedioPor()
    {
        return (ValorMetaDeIntermedio/ValorMetaDeSuperacion)*100;
    }
    public float GetMetaDeSuperacionPor()
    {
        return 100;
    }

    public float GetMetaSuperaciónValor()
    {
        return ValorMetaDeSuperacion;
    }

    public ContadorProgreso contadorProgreso;
   

    public IEnumerator EsperarParaIniciarGeneraciónDeMetas()
    {
        // Espera hasta que el objeto requerido no sea null y esté activo en la jerarquía.
        while (EstadisticasManager.Instance == null&& contadorProgreso == null&&ConfirmarMotivadoresCompletamenteCargados())
        {
            Debug.Log("Esperando a que carge los elemntos necesarios para generar metas");
            yield return null; // Espera un frame.
        }

        Debug.Log("Carga terminada");
        RegenerarMetasDiarias();
    }

    public bool ConfirmarMotivadoresCompletamenteCargados()
    {
        foreach(Dinosaurio motivador in EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema())
        {
            foreach(Exigencia exigencia in motivador.exigencias)
            {
                if(exigencia == null)
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
    public float PuntuacionRequerida { get => (PorcentajeRequeridoMeta / 100) * GestorMetas.Instance.GetMetaSuperaciónValor(); set { } }
    protected float porcentajeRequeridoMeta;
    public float PorcentajeRequeridoMeta
    {
        get => porcentajeRequeridoMeta; set
        {

            if (value >= 0 && value <= 100)
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
    };


    public void SetPorcentajeRequeridoSegunPuntuacion(float puntuacion)
    {

    }


    public void MarcaCompletada(bool estado)
    {
        Completado = estado;
        if (Completado == true)
        {
            PremioPorMarcaLograda?.Invoke(this);
        }

    }

}

public enum TipoMeta { MetaMinima, MetaIntermedia, MetaDeSuperación }
public class MetaProgresoRecompensa : MarcaBasicaRecompensa
{
    readonly TipoMeta tiposMetas;

    public MetaProgresoRecompensa(string nombre, float porcentajeRequerido, TipoMeta tipoMeta) : base(nombre, porcentajeRequerido)
    {
        this.tiposMetas = tipoMeta;
    }




}