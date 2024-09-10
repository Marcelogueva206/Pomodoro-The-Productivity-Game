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
    [SerializeField] public static float ProductivityPointsTotal = 500; // 0 pp
    [SerializeField] public static float ProductivityPointsDaily = 0;
    [SerializeField] public static float ProductivityPointsHolded = 0; //acumulas hasta que termines el último pomodoro que tengas o ciclo de descanso
    [SerializeField] public static float ProductivityPointsLoginGoal = 100;//  100 pp
    [SerializeField] public static float ProductivityPointsMinGoal = 1000;// 1000 pp
    [SerializeField] public static float ProductivityPointsMaxGoal = 2000; // 2000 pp
 
    public static EstrellaRecompensa PrimeraEstrella;
    public static EstrellaRecompensa SegundaEstrella;
    public static EstrellaRecompensa TerceraEstrella;
    [SerializeField] private RectTransform cuerpoPrimeraEstrella;
    [SerializeField] private RectTransform cuerpoSegundaEstrella;
    [SerializeField] private RectTransform cuerpoTerceraEstrella;

    public static List<MarcaBasicaRecompensa> marcasSimples;

    public static MarcaBasicaRecompensa Marca0Porciento;
    public static MarcaBasicaRecompensa Marca10Porciento;
    public static MarcaBasicaRecompensa Marca20Porciento;
    public static MarcaBasicaRecompensa Marca30Porciento;
    public static MarcaBasicaRecompensa Marca40Porciento;
    public static MarcaBasicaRecompensa Marca50Porciento;
    public static MarcaBasicaRecompensa Marca60Porciento;
    public static MarcaBasicaRecompensa Marca70Porciento;
    public static MarcaBasicaRecompensa Marca80Porciento;
    public static MarcaBasicaRecompensa Marca90Porciento;
    public static MarcaBasicaRecompensa Marca100Porciento;
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
        PomodoroSistema.TemposTerminado += AcumularPPs;
        PomodoroSistema.TemposTerminado += AumentarProgresoDiarioPPs;
        PomodoroSistema.PomodoroTerminado += RecibirAcumuladoPPS;

        PrimeraEstrella = new EstrellaRecompensa("Primera estrella de la productividad", gamificacionManager.cuerpoPrimeraEstrella, 10f, TipoEstrella.EstrellaPorInicio);
        SegundaEstrella = new EstrellaRecompensa("Segunda estrella de la productividad", gamificacionManager.cuerpoSegundaEstrella, 60f, TipoEstrella.EstrellaMinima);
        TerceraEstrella = new EstrellaRecompensa("Tercera máxima estrella de la productividad", gamificacionManager.cuerpoTerceraEstrella, 100f, TipoEstrella.EstrellaMaxima);

        Marca0Porciento = new MarcaBasicaRecompensa("Marca del mínimo esfuerzo", gamificacionManager.cuerpoMarca0, 1f);
        Marca10Porciento = new MarcaBasicaRecompensa("Marca del 10%", gamificacionManager.cuerpoMarca10, 10f);
        Marca20Porciento = new MarcaBasicaRecompensa("Marca del 20%", gamificacionManager.cuerpoMarca20, 20f);
        Marca30Porciento = new MarcaBasicaRecompensa("Marca del 30%", gamificacionManager.cuerpoMarca30, 30f);
        Marca40Porciento = new MarcaBasicaRecompensa("Marca del 40%", gamificacionManager.cuerpoMarca40, 40f);
        Marca50Porciento = new MarcaBasicaRecompensa("Marca de la mitad de progreso", gamificacionManager.cuerpoMarca50, 50f);
        Marca60Porciento = new MarcaBasicaRecompensa("Marca del 60%", gamificacionManager.cuerpoMarca60, 60f);
        Marca70Porciento = new MarcaBasicaRecompensa("Marca del 70%", gamificacionManager.cuerpoMarca70, 70f);
        Marca80Porciento = new MarcaBasicaRecompensa("Marca del 80%", gamificacionManager.cuerpoMarca80, 80f);
        Marca90Porciento = new MarcaBasicaRecompensa("Marca del 90%", gamificacionManager.cuerpoMarca90, 90f);
        Marca100Porciento = new MarcaBasicaRecompensa("Marca del éxito%", gamificacionManager.cuerpoMarca100, 100f);
        marcasSimples = new List<MarcaBasicaRecompensa> { Marca0Porciento, Marca10Porciento, Marca20Porciento, Marca30Porciento, Marca40Porciento, Marca50Porciento, Marca60Porciento, Marca70Porciento, Marca80Porciento, Marca90Porciento, Marca100Porciento };
    }


    private void AcumularPPs(Tempos tempo)
    {
        ProductivityPointsHolded +=(float) (tempo.TiempoTotal.TotalSeconds) * (360 / 100);
    }

    private void AumentarProgresoDiarioPPs(Tempos tempo)
    {
        ProductivityPointsDaily += (float)(tempo.TiempoTotal.TotalSeconds) * (360 / 100);
    }
    private void RecibirAcumuladoPPS(Pomodoro pomodoro)
    {
        ProductivityPointsTotal += ProductivityPointsHolded;
        ProductivityPointsHolded = 0;
    }

}
