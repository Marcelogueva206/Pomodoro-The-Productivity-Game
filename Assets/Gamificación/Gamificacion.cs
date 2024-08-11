using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Gamificacion : MonoBehaviour
{
    [Header("Productivity")]
    [SerializeField] public static float ProductivityPointsTotal = 500; // 0 pp
    [SerializeField] public static float ProductivityPointsDaily = 0;
    [SerializeField] public static float ProductivityPointsHolded = 0; //acumulas hasta que termines el último pomodoro que tengas o ciclo de descanso
    [SerializeField] public static float ProductivityPointsLoginGoal = 100;//  100 pp
    [SerializeField] public static float ProductivityPointsMinGoal = 1000;// 1000 pp
    [SerializeField] public static float ProductivityPointsMaxGoal = 2000; // 2000 pp

    // Porcentajes para ganar estrellas, configurables desde el Inspector
    [Range(0, 100)] public static float firstStarPercentage = 10f; //recompensa por iniciar sesión
    [Range(0, 100)] public static float secondStarPercentage = 60f; //mínimo
    [Range(0, 100)] public static float thirdStarPercentage = 90f; //superar siguiente CE


    public  static float ProgresoTotalMeta { get 
        {
            float value = ProductivityPointsDaily / ProductivityPointsMaxGoal * 100;
            if(value < 0)
            {
                return 0;
            } 
            if(value > 100) { 
            
                return 100;
            }
            return value; 
        }
        set { } }

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
    [SerializeField] private TextMeshProUGUI textoProductivityPointsTotal;
    [SerializeField] private TextMeshProUGUI textoProductivityPointsDaily;
    public static float MinY { get => Mathf.Min(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxY { get => Mathf.Max(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxX { get => Mathf.Max(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }
    public static float MinX { get => Mathf.Min(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }
    #endregion

    private void Awake()
    {
        PomodoroSistema.TemposTerminado += AcumularPPs;
        PomodoroSistema.TemposTerminado += AumentarProgresoDiarioPPs;
        PomodoroSistema.PomodoroTerminado += RecibirAcumuladoPPS;
    }

    
    private void AcumularPPs(Tempos tempo)
    {
        ProductivityPointsHolded += (tempo.TiempoTotal.x * 3600 + tempo.TiempoTotal.y * 60 + tempo.TiempoTotal.z)*(360/100);
    }

    private void AumentarProgresoDiarioPPs(Tempos tempo)
    {
        ProductivityPointsDaily += (tempo.TiempoTotal.x * 3600 + tempo.TiempoTotal.y * 60 + tempo.TiempoTotal.z) * (360 / 100);
    }
    private void RecibirAcumuladoPPS(Pomodoro pomodoro)
    {
        ProductivityPointsTotal += ProductivityPointsHolded;
        ProductivityPointsHolded = 0;
    }
    void Start()
    {
        gamificacionManager = this;
    }

 
  

    //private void MostrarProductivityPoints()
    //{
    //    textoProductivityPointsDaily.text = ProductivityPointsTotal.ToString();
    //}

}
