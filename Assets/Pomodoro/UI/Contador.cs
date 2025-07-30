using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using Unity.Notifications.Android;

public class Contador : MonoBehaviour
{
    public enum FasesContador { Inicio, Detenido, Progeso, Terminado }
    [HideInInspector] private static bool ContandoActivo = false;
    [SerializeField] public static FasesContador FaseActual = FasesContador.Inicio;
    public static Contador contador;
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private TextMeshProUGUI textoTempoNombre;
    [SerializeField] private TextMeshProUGUI textoPuntuacionTempo;
    [SerializeField] public static float PuntuacionTempo = 0;
    [HideInInspector] private float _tiempoRestante;
    [HideInInspector] private float _tiempoTotal;
    [SerializeField] private Vector3 TiempoQueFalta;
    [HideInInspector] private int _horasRestante;
    [HideInInspector] private int _minutosRestante;
    [HideInInspector] private int _segundosRestante;
    [SerializeField] private GameObject barraProgreso;
    [SerializeField] private GameObject BotonIniciar;
    [SerializeField] private GameObject BotonPausar;
    [HideInInspector] private Slider sliderTiempoRestante;
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
        sliderTiempoRestante = barraProgreso.GetComponent<Slider>();
        //PomodoroSistema.TemposIniciado += MostrarNombreTempoUI;
        //PomodoroSistema.CicloIniciado += MostrarNombreCicloUI;
        //PomodoroSistema.PomodoroIniciado += MostrarNombrePomodoroUI;
        OnTerminadoContador = ReiniciarPuntuaciónTempo;
        CargarTiempoRestante();

    }
    public void MostrarNombreTempoUI(Tempos tempo)
    {
        if (tempo != null)
        {
            textoTempoNombre.text = tempo.Nombre;
        }

    }

  




    private void MostrarTiempoUI()
    {
        HorasRestanteStatic = Mathf.FloorToInt(TiempoRestanteStatic / 3600);
        MinutosRestanteStatic = Mathf.FloorToInt((Mathf.FloorToInt(TiempoRestanteStatic / 60)) % 60);

        SegundosRestanteStatic = Mathf.FloorToInt(TiempoRestanteStatic % 60);

        textoContador.text = string.Format("{0:00}:{1:00}:{2:00}", HorasRestanteStatic, MinutosRestanteStatic, SegundosRestanteStatic);
        sliderTiempoRestante.value = TiempoRestanteStatic / TiempoTotalStatic;
    }

    private void CargarTiempoRestante()
    {
        if (PlayerPrefs.HasKey("TiempoRestante"))
        {
            TiempoRestanteStatic = PlayerPrefs.GetFloat("TiempoRestante");
        }
        else
        {
            // Si no hay valor guardado, usar el valor predeterminado
            TiempoRestanteStatic = TiempoTotalStatic;
        }
    }

    private void GuardarTiempoRestante()
    {
        PlayerPrefs.SetFloat("TiempoRestante", TiempoRestanteStatic);
        PlayerPrefs.Save(); // Aseguramos que se guarde inmediatamente
    }

    private void ContandoTiempo()
    {
        TiempoRestanteStatic -= Time.deltaTime;
        GuardarTiempoRestante();

        if(PomodoroSistema._tempoActual.tiposTempos == TiposTempos.productivo)
        {
            PuntuacionTempo += Time.deltaTime * (1f / 60f);
        }

        if(Contador.FaseActual == FasesContador.Inicio)
        {
            PuntuacionTempo = 0;
        }
     
        float value = Mathf.Floor(PuntuacionTempo);
        textoPuntuacionTempo.text = value.ToString();
    }

    private void Start()
    {
        CambiarFaseContador(FasesContador.Inicio);
        IniciarEnfoqueCorto();
        BotonPausar.SetActive(false);

        if (PlayerPrefs.HasKey("pomodoro_fase"))
        {
            string faseGuardada = PlayerPrefs.GetString("pomodoro_fase");

            if (faseGuardada == "Progreso" && PlayerPrefs.HasKey("pomodoro_fin"))
            {
                long binary = Convert.ToInt64(PlayerPrefs.GetString("pomodoro_fin"));
                DateTime horaFin = DateTime.FromBinary(binary);

                double segundosRestantes = (horaFin - DateTime.Now).TotalSeconds;

                if (segundosRestantes <= 0)
                {
                    // ⏰ Ya terminó
                    CambiarFaseContador(FasesContador.Terminado);
                }
                else
                {
                    // 🔁 Continuar Pomodoro
                    contador.IniciarConTiempoRestante((float)segundosRestantes);
                    CambiarFaseContador(FasesContador.Progeso);
                }
            }
            else if (faseGuardada == "Detenido")
            {
                // Si estaba en pausa, solo restaura el tiempo restante
                float segundosGuardados = PlayerPrefs.GetFloat("pomodoro_tiempo_restante");
                contador.IniciarConTiempoRestante(segundosGuardados);
                CambiarFaseContador(FasesContador.Detenido);
            }
        }


    }

    public void IniciarConTiempoRestante(float segundos)
    {
        TiempoRestanteStatic = segundos;
        MostrarTiempoUI();
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
                contador.BotonPausar.SetActive(false); // <- Esto debe estar en false
                contador.BotonIniciar.SetActive(true);  // <- Este en true

                if (NotificacionManager.Instance != null)
                {
                    NotificacionManager.Instance.CancelarTodasNotificaciones();
                }
                break;
            case FasesContador.Inicio:
                OnInicioContador.Invoke();

                ContandoActivo = false;
                ReiniciarTiempoContador();
                contador.BotonPausar.SetActive(false);
                contador.BotonIniciar.SetActive(true);

                if (NotificacionManager.Instance != null)
                {
                    NotificacionManager.Instance.CancelarTodasNotificaciones();
                }

                PlayerPrefs.SetString("pomodoro_fin", DateTime.Now.AddSeconds((int)TiempoRestanteStatic).ToBinary().ToString());
                PlayerPrefs.SetString("pomodoro_fase", FaseActual.ToString());
                PlayerPrefs.Save();

                break;
            case FasesContador.Progeso:
                OnProgresoContador.Invoke();
                ContandoActivo = true;
                contador.BotonPausar.SetActive(true);
                contador.BotonIniciar.SetActive(false);
                // ✅ Cancelar notificaciones anteriores
                if (NotificacionManager.Instance != null)
                {
                    NotificacionManager.Instance.CancelarTodasNotificaciones();
                    NotificacionManager.Instance?.ProgramarNotificacion((int)TiempoRestanteStatic);  //NO TE DICE CUANTO FALTA, SINO CUANDO ESTABA PROGRESANDO ANTES DE CERRAR
                }
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

        }else if (FasesContador.Terminado == FaseActual)
        {

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

    public static void IniciarEnfoqueCorto()
    {
        PomodoroSistema.ActualizarNuevoPomodoro(Pomodoro.PomodorosTipos.corto);
        CambiarFaseContador(FasesContador.Inicio);
    }

    public static void IniciarEnfoqueMediano()
    {
        PomodoroSistema.ActualizarNuevoPomodoro(Pomodoro.PomodorosTipos.mediano);
             CambiarFaseContador(FasesContador.Inicio);
    }
    public static void IniciarEnfoqueLargo()
    {
        PomodoroSistema.ActualizarNuevoPomodoro(Pomodoro.PomodorosTipos.largo);
        CambiarFaseContador(FasesContador.Inicio);
    }


    public static event FaseContador OnInicioContador = () => Debug.Log("Contador: Inicio");
    public static event FaseContador OnProgresoContador = () => Debug.Log("Contador: Progreso");
    public static event FaseContador OnDetenidoContador = () => Debug.Log("Contador: Detenido");
    public static event FaseContador OnTerminadoContador = () => Debug.Log("Contador: Terminado");

}
public delegate void FaseContador();


