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
    [SerializeField] public float TiempoTotal = 0; //Tiempo acumulado de varios días
    [SerializeField] public float TiempoTotalDiario = 0; //Tiempo total del día
    [SerializeField] public float TiempoPorAdquirir = 0; //Tiempo total de


    public float ProgresoTotalMetaPor
    {
        get
        {
            float value = TiempoTotalDiario / GestorMetas.Instance.GetMetaSuperaciónValor() * 100f;
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
        PomodoroSistema.PomodoroTerminado += RecibirAcumuladoTiempo;

         PlayerPrefs.GetFloat("TiempoTotal", 0);
         PlayerPrefs.GetFloat("TiempoTotalDiario", 0);
         PlayerPrefs.GetFloat("TiempoPorAdquirir", 0);

    }

    private void Start()
    {
        //PrimeraMeta = new MetaProgresoRecompensa("Primera meta de la productividad", gamificacionManager.cuerpoPrimeraEstrella, GestorMetas.Instance.getMetaMinimaPor(), TipoEstrella.MetaMinima);
        //SegundaMeta = new MetaProgresoRecompensa("Segunda meta de la productividad", gamificacionManager.cuerpoSegundaEstrella, GestorMetas.Instance.getMetaDeIntermedioPor(), TipoEstrella.MetaIntermedia);
        //TerceraMeta = new MetaProgresoRecompensa("Tercera máxima meta de la productividad", gamificacionManager.cuerpoTerceraEstrella, GestorMetas.Instance.getMetaDeSuperacionPor(), TipoEstrella.MetaDeSuperación);
        TiempoTotal = PlayerPrefs.GetFloat("TiempoTotal");
        TiempoTotalDiario = PlayerPrefs.GetFloat("TiempoTotalDiario");
        TiempoPorAdquirir = PlayerPrefs.GetFloat("TiempoPorAdquirir");
    }





    private void AcumularTiempo(Tempos tempo)
    {
        TiempoPorAdquirir += (float)(tempo.TiempoTotal.TotalSeconds) * (1f / 60f);
        GuardarTiempo();
    }
    private void AumentarProgresoDiarioTiempo(Tempos tempo)
    {
        TiempoTotalDiario += (float)(tempo.TiempoTotal.TotalSeconds) * (1f / 60f);

        GuardarTiempo();
    }
    private void RecibirAcumuladoTiempo(Pomodoro pomodoro)
    {
        TiempoTotal += TiempoPorAdquirir;
        TiempoPorAdquirir = 0;
        GuardarTiempo();
    }
    public bool TryConsumirTiempo(float cantidad)
    {
        if (TiempoPorAdquirir - cantidad >= 0)
        {
            TiempoPorAdquirir -= cantidad;
            return true;
        }
        GuardarTiempo();
        return false;     
    }

    public void GuardarTiempo()
    {
        PlayerPrefs.SetFloat("TiempoTotal", TiempoTotal);
        PlayerPrefs.SetFloat("TiempoTotalDiario", TiempoTotalDiario);
        PlayerPrefs.SetFloat("TiempoPorAdquirir", TiempoPorAdquirir);
    }

    void OnApplicationQuit()
    {
        GuardarTiempo();
    }
    void OnApplicationPause()
    {
        GuardarTiempo();
    }
}


