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
    [SerializeField] public static float ProductivityPointsTotal = 0; // 0 pp
    [SerializeField] public static float ProductivityPointsDaily = 0;
    [SerializeField] public static float ProductivityPointsHolded = 60; //acumulas hasta que termines el último pomodoro que tengas o ciclo de descanso

    [SerializeField] public static float ProductivityPointsLoginGoal = 100;//  100 pp
    [SerializeField] public static float ProductivityPointsMinGoal = 1000;// 1000 pp
    [SerializeField] public static float ProductivityPointsMaxGoal = 2000; // 2000 pp
 
    //public static MetaProgresoRecompensa PrimeraMeta;
    //public static MetaProgresoRecompensa SegundaMeta;
    //public static MetaProgresoRecompensa TerceraMeta;
    //[SerializeField] private RectTransform cuerpoPrimeraEstrella;
    //[SerializeField] private RectTransform cuerpoSegundaEstrella;
    //[SerializeField] private RectTransform cuerpoTerceraEstrella;

  


    public static float ProgresoTotalMeta
    {
        get
        {
            float value = ProductivityPointsDaily / ProductivityPointsMaxGoal * 100;
            if (value < 0)
            {
                return 0;
            }
            if (value > 100)
            {

                return 100;
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
    public static Gamificacion gamificacionManager;
    public static float MinY { get => Mathf.Min(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxY { get => Mathf.Max(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxX { get => Mathf.Max(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }
    public static float MinX { get => Mathf.Min(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }
    #endregion

    private void Awake()
    {
        gamificacionManager = this;
        PomodoroSistema.TemposTerminado += AcumularTiempo;
        PomodoroSistema.TemposTerminado += AumentarProgresoDiarioTiempo;
        PomodoroSistema.PomodoroTerminado += RecibirAcumuladoTiempo;

       
        
    }

    private void Start()
    {
        //PrimeraMeta = new MetaProgresoRecompensa("Primera meta de la productividad", gamificacionManager.cuerpoPrimeraEstrella, GestorMetas.Instance.getMetaMinimaPor(), TipoEstrella.MetaMinima);
        //SegundaMeta = new MetaProgresoRecompensa("Segunda meta de la productividad", gamificacionManager.cuerpoSegundaEstrella, GestorMetas.Instance.getMetaDeIntermedioPor(), TipoEstrella.MetaIntermedia);
        //TerceraMeta = new MetaProgresoRecompensa("Tercera máxima meta de la productividad", gamificacionManager.cuerpoTerceraEstrella, GestorMetas.Instance.getMetaDeSuperacionPor(), TipoEstrella.MetaDeSuperación);

    }





    private void AcumularTiempo(Tempos tempo)
    {
        ProductivityPointsHolded +=(float) (tempo.TiempoTotal.TotalSeconds) * (1 / 60);
    }
    private void AumentarProgresoDiarioTiempo(Tempos tempo)
    {
        ProductivityPointsDaily += (float)(tempo.TiempoTotal.TotalSeconds) * (1 / 60);
    }
    private void RecibirAcumuladoTiempo(Pomodoro pomodoro)
    {
        ProductivityPointsTotal += ProductivityPointsHolded;
        ProductivityPointsHolded = 0;
    }
    public static bool TryConsumirTiempo(float cantidad)
    {
        if (ProductivityPointsHolded - cantidad >= 0)
        {
            ProductivityPointsHolded -= cantidad;
            return true;
        }
        return false;
    }

}
