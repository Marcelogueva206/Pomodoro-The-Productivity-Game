using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Contador : MonoBehaviour
{
    public enum FasesContador { Inicio, Detenido, Progeso, Terminado }
    [HideInInspector] private static bool ContandoActivo = false;
    [SerializeField] public static FasesContador FaseActual = FasesContador.Inicio;
    public static Contador contador;
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private TextMeshProUGUI textoTempoNombre;
    [SerializeField] private TextMeshProUGUI textoCicloNombre;
    [SerializeField] private TextMeshProUGUI textoPomodoroNombre;
    [SerializeField] private TextMeshProUGUI textoPuntuacionTempo;
    [SerializeField] public static float PuntuacionTempo = 0;
    [HideInInspector] private float _tiempoRestante;
    [HideInInspector] private float _tiempoTotal;
    [SerializeField] private Vector3 TiempoQueFalta;
    [HideInInspector] private int _horasRestante;
    [HideInInspector] private int _minutosRestante;
    [HideInInspector] private int _segundosRestante;
    [SerializeField] private GameObject barraProgreso;
    [HideInInspector] private Slider slider;
    private float TiempoRestante { get => _tiempoRestante; set => _tiempoRestante = (value > 0) ? value : 0; }
    private int HorasRestante { get => _horasRestante; set => _horasRestante = (value > 0) ? value : 0; }
    private int MinutosRestante { get => _minutosRestante; set => _minutosRestante = (value > 0) ? value : 0; }
    private int SegundosRestante { get => _segundosRestante; set => _segundosRestante = (value > 0) ? value : 0; }

    public static float TiempoRestanteStatic;
    public static int HorasRestanteStatic;
    public static int MinutosRestanteStatic;
    public static int SegundosRestanteStatic;
    public static float TiempoTotalStatic;

    public static event ProgresoUsuarioTempos TempoIniciadoPorUsuario = Tempo => Debug.Log($"se inició tempo: {Tempo.Nombre}");
    public static event ProgresoUsuarioCiclo CicloIniciadoPorUsuario = Ciclo => Debug.Log($"se inició tempo: {Ciclo.Nombre}");
    public static event ProgresoUsuarioPomodoro PomodoroIniciadoPorUsuario = Pomodoro => Debug.Log($"se inició tempo: {Pomodoro.Nombre}");

    private void Awake()
    {
        contador = this; // FLATA SINGLETON


        AsignarContador(new TimeSpan(0, 10, 0));
        slider = barraProgreso.GetComponent<Slider>();
        //PomodoroSistema.TemposIniciado += MostrarNombreTempoUI;
        //PomodoroSistema.CicloIniciado += MostrarNombreCicloUI;
        //PomodoroSistema.PomodoroIniciado += MostrarNombrePomodoroUI;
        OnTerminadoContador = ReiniciarPuntuaciónTempo;
       
    }
    public void MostrarNombreTempoUI(Tempos tempo)
    {
        if (tempo != null)
        {
            textoTempoNombre.text = tempo.Nombre;
        }

    }

    public void MostrarNombreCicloUI(Ciclo ciclo)
    {
        if (ciclo != null)
        {
            textoCicloNombre.text = ciclo.Nombre;
        }

    }

    public void MostrarNombrePomodoroUI(Pomodoro pomodoro)
    {
        if (pomodoro != null)
        {
            textoPomodoroNombre.text = pomodoro.Nombre;
        }

    }




    private void MostrarTiempoUI()
    {
        HorasRestanteStatic = Mathf.FloorToInt(TiempoRestanteStatic / 3600);
        MinutosRestanteStatic = Mathf.FloorToInt((Mathf.FloorToInt(TiempoRestanteStatic / 60)) % 60);

        SegundosRestanteStatic = Mathf.FloorToInt(TiempoRestanteStatic % 60);

        textoContador.text = string.Format("{0:00}:{1:00}:{2:00}", HorasRestanteStatic, MinutosRestanteStatic, SegundosRestanteStatic);
        slider.value = TiempoRestanteStatic / TiempoTotalStatic;
    }
    private void ContandoTiempo()
    {
        TiempoRestanteStatic -= Time.deltaTime;
        PuntuacionTempo += Time.deltaTime * (360 / 100);
        float value = Mathf.Round(PuntuacionTempo);
        textoPuntuacionTempo.text = value.ToString();
    }

    private void Start()
    {
        CambiarFaseContador(FasesContador.Inicio);

    }

    public void AsignarContadorGUI()
    {
        TiempoTotalStatic = TiempoQueFalta.x * 3600 + TiempoQueFalta.y * 60 + TiempoQueFalta.z;
        ContandoTiempo();
    }

    public static void AsignarContador(int horas, int minutos, int segundos)
    {
        TiempoTotalStatic = horas * 3600 + minutos * 60 + segundos;
    }
    public static void AsignarContador(TimeSpan tiempo)
    {
        TiempoTotalStatic = (float)tiempo.TotalSeconds;
    }


    void Update()
    {
        SetStatic();
        MostrarTiempoUI();
        AsignarFase();
        if (ContandoActivo)
        {
            ContandoTiempo();
        }
    }

    private static void AsignarFase()
    {
        if (FaseActual == FasesContador.Inicio)
        {
            contador.MostrarNombreTempoUI(PomodoroSistema._tempoActual);
            contador.MostrarNombreCicloUI(PomodoroSistema._cicloActual);
            contador.MostrarNombrePomodoroUI(PomodoroSistema._pomodoroActual);
            ReiniciarTiempoContador();
        }


        if (TiempoRestanteStatic <= 0)
        {
            TiempoRestanteStatic = 0;
            CambiarFaseContador(FasesContador.Terminado);
        }
    }

    private static void ReiniciarTiempoContador()
    {
        TiempoRestanteStatic = TiempoTotalStatic;
    }

    public static void CambiarFaseContador(FasesContador fasesPomodoro)
    {
        FaseActual = fasesPomodoro;

        switch (FaseActual)
        {
            case FasesContador.Detenido:
                OnDetenidoContador.Invoke();
                ContandoActivo = false;
                break;
            case FasesContador.Inicio:
                OnInicioContador.Invoke();

                ContandoActivo = false;
                ReiniciarTiempoContador();
                break;
            case FasesContador.Progeso:
                OnProgresoContador.Invoke();
                ContandoActivo = true;

                break;
            case FasesContador.Terminado:
                OnTerminadoContador.Invoke();
                ContandoActivo = false;
                PomodoroSistema._tempoActual.AsignarEstadoCompletado(true);
                CambiarFaseContador(FasesContador.Inicio);
                break;
        }
    }

    private void ReiniciarPuntuaciónTempo()
    {
        PuntuacionTempo = 0;
    }


    private void SetStatic()
    {
        TiempoRestante = TiempoRestanteStatic;
        HorasRestante = HorasRestanteStatic;
        MinutosRestante = MinutosRestanteStatic;
        SegundosRestante = SegundosRestanteStatic;
        _tiempoTotal = TiempoTotalStatic;
    }


    public static void IniciarPausarContador()
    {
        if (FasesContador.Inicio == FaseActual)
        {
            TempoIniciadoPorUsuario.Invoke(PomodoroSistema._tempoActual);

            SeInicioNuevoCiclo();
            SeinicioNuevoPomodoro();

            CambiarFaseContador(FasesContador.Progeso);
        }
        else if (FasesContador.Detenido == FaseActual)
        {
            CambiarFaseContador(FasesContador.Progeso);
        }
        else if (FasesContador.Progeso == FaseActual)
        {
            CambiarFaseContador(FasesContador.Detenido);
        }

    }


    private static Ciclo cicloReciente;
    private static void SeInicioNuevoCiclo()
    {

        if (cicloReciente == null)
        {
            cicloReciente = PomodoroSistema._cicloActual;
            CicloIniciadoPorUsuario.Invoke(cicloReciente);

        }

        if (cicloReciente != PomodoroSistema._cicloActual)
        {
            CicloIniciadoPorUsuario.Invoke(PomodoroSistema._cicloActual);
            cicloReciente = PomodoroSistema._cicloActual;
        }
    }
    private static Pomodoro pomodoroReciente;
    private static void SeinicioNuevoPomodoro()
    {
        if(pomodoroReciente == null)
        {
            pomodoroReciente = PomodoroSistema._pomodoroActual;
            PomodoroIniciadoPorUsuario.Invoke(pomodoroReciente);
        }

        if (pomodoroReciente != PomodoroSistema._pomodoroActual)
        {
            PomodoroIniciadoPorUsuario.Invoke(PomodoroSistema._pomodoroActual);
            pomodoroReciente = PomodoroSistema._pomodoroActual;
        }
    }

    public static void ReiniciarContador()
    {
        if (FasesContador.Inicio != FaseActual)
        {
            CambiarFaseContador(FasesContador.Inicio);
        }

        ReiniciarContador();
    }


    public static event FaseContador OnInicioContador = () => Debug.Log("Contador: Inicio");
    public static event FaseContador OnProgresoContador = () => Debug.Log("Contador: Progreso");
    public static event FaseContador OnDetenidoContador = () => Debug.Log("Contador: Detenido");
    public static event FaseContador OnTerminadoContador = () => Debug.Log("Contador: Terminado");

}
public delegate void FaseContador();


