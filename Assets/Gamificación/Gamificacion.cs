using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;



//RECUERDA QUE DEBES AHCER QUE EL LOS LIMITES SE ACTUALICEN SEGUN LA RESOLUCIOn de lA PANTALLA
public class Gamificacion : MonoBehaviour
{
    [Header("Productivity")]
    [SerializeField] public float TiempoTotalAcumulado = 0; //Tiempo acumulado de varios días
    [SerializeField] public float TiempoTotalProducidoHoy = 0; //Tiempo total de exclusivamente lo mostrado en el pomodoro
    [SerializeField] public float TiempoAcumuladoHoy = 0; //Tiempo total del día


    public float ProgresoTotalMetaDiarioPor
    {
        get
        {
            float value = TiempoTotalProducidoHoy / GestorMetas.Instance.GetMetaSuperaciónValor() * 100f;
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 100f)
            {

                return 100f;
            }
            return value;
        }
        set { }
    }

    /// <summary>
    /// 6 minutos => 100 pp
    /// 1 hora productiva ==> 1000 pp
    /// </summary>

    ///Tabla de CE <summary>
    //    Horas de Estudio Diarias    Porcentaje de Estudiantes(%)
    //1 hora	15%
    //2 horas	25%
    //3 horas	20%
    //4 horas	15%
    //5 horas	10%
    //6 horas	8%
    //7 horas	4%
    //8 horas	2%
    //9 horas	1%
    //10 horas o más	0.5%


    //    Horas de Estudio Diarias	% de Estudiantes Acumulado Percentil(%)   TOP Percentil(%)
    //0 horas	5%	5%	TOP 100% - 96%
    //1 hora	15%	10%	TOP 95% - 86%
    //2 horas	30%	15%	TOP 85% - 71%
    //3 horas	50%	20%	TOP 70% - 51% : estudiante promedio 
    //4 horas	65%	15%	TOP 50% - 36%
    //5 horas	80%	15%	TOP 35% - 21%
    //6 horas	90%	10%	TOP 20% - 11%
    //7 horas	95%	5%	TOP 10% - 6%
    //8 horas	97%	2%	TOP 5% - 3%
    //9 horas o más	100%	3%	TOP 2% - 1%
    /// </summary>
    #region Extra
    [Header("Extra")]
    [SerializeField] private GameObject Limite1;
    [SerializeField] private GameObject Limite2;
    public static Gamificacion Instance;
    public static float MinY { get => Mathf.Min(Instance.Limite1.transform.position.y, Instance.Limite2.transform.position.y); }
    public static float MaxY { get => Mathf.Max(Instance.Limite1.transform.position.y, Instance.Limite2.transform.position.y); }
    public static float MaxX { get => Mathf.Max(Instance.Limite1.transform.position.x, Instance.Limite2.transform.position.x); }
    public static float MinX { get => Mathf.Min(Instance.Limite1.transform.position.x, Instance.Limite2.transform.position.x); }
    #endregion

    private void Awake()
    {
        Instance = this;
        PomodoroSistema.TemposTerminado += AcumularTiempo;
        PomodoroSistema.TemposTerminado += AumentarProgresoDiarioTiempo;
        //PomodoroSistema.PomodoroTerminado += RecibirAcumuladoTiempo;

        AsignarValoresPredeterminadosPuntuacion();

        if (ComprobarEsOtroDia())
        {
            RecibirAcumuladoTiempoHoy();
            ReiniciarContadorTiempoProducidoHoy();

        }
    }

    private void ReiniciarContadorTiempoProducidoHoy()
    {
        TiempoTotalProducidoHoy = 0;
    }

    private void Start()
    {
       


    }

    void AsignarValoresPredeterminadosPuntuacion()
    {
        // Verificar y asignar valores predeterminados para TiempoTotal
        if (!PlayerPrefs.HasKey("TiempoTotalAcumulado"))
        {
            PlayerPrefs.SetFloat("TiempoTotalAcumulado", 0f); // Valor predeterminado
        }
        TiempoTotalAcumulado = PlayerPrefs.GetFloat("TiempoTotalAcumulado");

        // Verificar y asignar valores predeterminados para TiempoTotalDiario
        if (!PlayerPrefs.HasKey("TiempoTotalProducidoHoy"))
        {
            PlayerPrefs.SetFloat("TiempoTotalProducidoHoy", 0f); // Valor predeterminado
        }
        TiempoTotalProducidoHoy = PlayerPrefs.GetFloat("TiempoTotalProducidoHoy");

        // Verificar y asignar valores predeterminados para TiempoPorAdquirir
        if (!PlayerPrefs.HasKey("TiempoAcumuladoHoy"))
        {
            PlayerPrefs.SetFloat("TiempoAcumuladoHoy", 0f); // Valor predeterminado
        }
        TiempoAcumuladoHoy = PlayerPrefs.GetFloat("TiempoAcumuladoHoy");
    }



    private void AcumularTiempo(Tempos tempo)
    {
        TiempoAcumuladoHoy += (float)(tempo.TiempoTotal.TotalSeconds) * (1f / 60f);
        GuardarTiempo();
    }
    private void AumentarProgresoDiarioTiempo(Tempos tempo)
    {
        if(tempo.tiposTempos == TiposTempos.productivo)
        {
            TiempoTotalProducidoHoy += (float)(tempo.TiempoTotal.TotalSeconds) * (1f / 60f);
            Debug.Log($"[DEBUG] Aumentando progreso diario: {TiempoTotalProducidoHoy} minutos");

            if (tempo == null)
            {
                Debug.LogWarning("Tempo es nulo. No se puede aumentar el progreso.");
                return;
            }

            if (tempo.TiempoTotal.TotalSeconds <= 0)
            {
                Debug.LogWarning("Tempo terminado tiene 0 segundos. No se suma nada.");
                return;
            }

            GuardarTiempo();
        }
     
    }
    private void RecibirAcumuladoTiempoHoy()
    {
        TiempoTotalAcumulado += TiempoAcumuladoHoy;
        TiempoAcumuladoHoy = 0;
        GuardarTiempo();
    }
    public bool TryConsumirTiempo(float cantidad)
    {
        if (TiempoAcumuladoHoy - cantidad >= 0)
        {
            TiempoAcumuladoHoy -= cantidad;
            return true;
        }
        else if (TiempoAcumuladoHoy + TiempoTotalAcumulado - cantidad >=0)
        {
            float exceso = cantidad - TiempoAcumuladoHoy;

                TiempoAcumuladoHoy = 0;
            TiempoTotalAcumulado -= exceso;
            return true;
        }
        GuardarTiempo();
        return false;     
    }

    public void GuardarTiempo()
    {
        PlayerPrefs.SetFloat("TiempoTotalAcumulado", TiempoTotalAcumulado);
        PlayerPrefs.SetFloat("TiempoTotalProducidoHoy", TiempoTotalProducidoHoy);
        PlayerPrefs.SetFloat("TiempoAcumuladoHoy", TiempoAcumuladoHoy);
    }

    void OnApplicationQuit()
    {
        GuardarTiempo();
    }
    void OnApplicationPause()
    {
        GuardarTiempo();
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

}


