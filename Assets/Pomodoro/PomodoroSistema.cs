using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public delegate void ProgresoUsuarioPomodoro(Pomodoro pomodoro);
public delegate void ProgresoUsuarioTempos(Tempos tempo);
public delegate void ProgresoUsuarioCiclo(Ciclo ciclo);


public class PomodoroSistema : MonoBehaviour
{
    [HideInInspector] public static Pomodoro _pomodoroActual;
    [HideInInspector] public static Sesion _sesionActual;
    [HideInInspector] public static Ciclo _cicloActual;
    [HideInInspector] public static Tempos _tempoActual;
    [SerializeField] private static int numeroCicloActual = 0;
    [SerializeField] private static int numeroTempoActual = 0;
    [SerializeField] private TextMeshProUGUI textoPomodorosRestantes;
    #region UnityMethods
    private void Awake()
    {
        Pomodoro pomodoro1 = new Pomodoro(Pomodoro.PomodorosTipos.prueba);
        Pomodoro pomodoro2 = new Pomodoro(Pomodoro.PomodorosTipos.normal);

        _sesionActual = new Sesion(new List<Pomodoro> { pomodoro1, pomodoro2 }, "Sesion de prueba");
    }
    void Start()
    {
        // necesario crear un metodo instanciador
        _pomodoroActual = _sesionActual._pomodorosSesion[0];
        _cicloActual = _sesionActual._pomodorosSesion[0].ciclosPomodoro[0];
        _tempoActual = _sesionActual._pomodorosSesion[0].ciclosPomodoro[0].TemposCiclo[0];
        //solo para probar
 
    }
    void Update()
    {
        EjecutarSision(_sesionActual);
        Debug.Log($"tempo: {numeroTempoActual} y ciclo: {numeroCicloActual}");
        textoPomodorosRestantes.text = "Pomodoros restantes: " + _sesionActual.pomodorosRestantes;

    }
    #endregion



    public static event ProgresoUsuarioPomodoro PomodoroTerminado = Pomodoro => Debug.Log($"se terminó el pomodoro: {Pomodoro.Nombre}");
    public static event ProgresoUsuarioPomodoro PomodoroIniciado = Pomodoro => Debug.Log($"se inició el pomodoro: {Pomodoro.Nombre}");
    //= delegate (Pomodoro pomodoro)
    //{

    //    Debug.Log($"se terminó el pomodoro: {pomodoro.Nombre}");
    //};
    public static event ProgresoUsuarioTempos TemposTerminado = Tempos => Debug.Log($"se terminó tempo: {Tempos.Nombre}");
    public static event ProgresoUsuarioTempos TemposIniciado = Tempos => Debug.Log($"se inició tempo: {Tempos.Nombre}");
    
    public static event ProgresoUsuarioCiclo CicloTerminado = Ciclo => Debug.Log($"se terminó ciclo: {Ciclo.Nombre}");
    public static event ProgresoUsuarioCiclo CicloIniciado = Ciclo => Debug.Log($"se inició ciclo: {Ciclo.Nombre}");



    public static void ReiniciarNumeroTempo()
    {
        numeroTempoActual = 0;
    }

    public void EjecutarSision(Sesion sesion)
    {
        if (sesion.GetEstadoCompletado() == false)
        {
            _sesionActual = sesion;
            EjecutarPomodoro(sesion._pomodorosSesion[sesion.numeroPomodoroActual], sesion);

        }
        else
        {
            Debug.Log("Sesión terminada");
        }
    }

    public void EjecutarPomodoro(Pomodoro pomodoro, Sesion sesionPerteneciente)
    {
        if (pomodoro.GetEstadoCompletado() == false)
        {
            if (pomodoro.ciclosPomodoro[numeroCicloActual].TryGetEstadoCompletado() == false)
            {
                _cicloActual = pomodoro.ciclosPomodoro[numeroCicloActual];

                if (_cicloActual.TemposCiclo[numeroTempoActual].GetEstadoCompletado() == false)
                {

                    _tempoActual = _cicloActual.TemposCiclo[numeroTempoActual];
                    UsarTempo(_tempoActual);
                }
                else
                {
                    TemposTerminado?.Invoke(_tempoActual);
                    numeroTempoActual++;
                    if(_cicloActual.TemposCiclo[numeroTempoActual] == null)
                    {
                        //se ha terminado todos los tempos del ciclo, por ende debe elegir el primero del ciclo si aún se debe repetir
                        _tempoActual = pomodoro.ciclosPomodoro[numeroCicloActual + 1].TemposCiclo[0];
                    }
                    else
                    {
                        _tempoActual = _cicloActual.TemposCiclo[numeroTempoActual];
                    }
                   

                    TemposIniciado.Invoke(_tempoActual);

                }



            }
            else
            {
                CicloTerminado.Invoke(_cicloActual);
                numeroCicloActual++;           
                _cicloActual = pomodoro.ciclosPomodoro[numeroCicloActual];
                CicloIniciado.Invoke(_cicloActual);

            }

        }
        else
        {
            PomodoroTerminado.Invoke(_pomodoroActual);
            sesionPerteneciente.numeroPomodoroActual++;
            _pomodoroActual = sesionPerteneciente._pomodorosSesion[sesionPerteneciente.numeroPomodoroActual];
            numeroCicloActual = 0;
            PomodoroIniciado.Invoke(_pomodoroActual);
            
        }


    }

    public void UsarTempo(Tempos tempo)
    {
        Contador.AsignarContador(tempo.TiempoTotal);

    }
}





public class Sesion : IEstadoCompletado
{
    public string nombre;
    public int numeroPomodoroActual = 0;
    private int numeroPomodoroTotal = 0;
    public List<Pomodoro> _pomodorosSesion;
    private bool _completado = false;
    public int pomodorosRestantes { get => numeroPomodoroTotal - numeroPomodoroActual - 1; }
    public Sesion(List<Pomodoro> pomodoros, string nombre = "")
    {
        _pomodorosSesion = pomodoros;
        numeroPomodoroActual = 0;
        numeroPomodoroTotal = pomodoros.Count;
        this.nombre = nombre;
    }

    public bool Completado { get => _completado; set => _completado = value; }

    public void AsignarEstadoCompletado(bool estado)
    {
        Completado = estado;
    }

    public bool GetEstadoCompletado()
    {
        Completado = true;
        foreach (Pomodoro pomodoro in _pomodorosSesion)
        {
            if (pomodoro.GetEstadoCompletado() == false)
            {
                Completado = false;
            }

        }

        return Completado;
    }
}

public class Pomodoro : IEstadoCompletado
{
    public List<Ciclo> ciclosPomodoro;
    private bool _completado = false;
    public string Nombre { get => _tipo.ToString(); }
    public bool Completado { get => _completado; set => _completado = value; }

    public Vector3 DuracionTotal
    {
        get
        {
            Vector3 tiempoTotal = Vector3.zero;

            foreach (Ciclo ciclo in ciclosPomodoro)
            {
                tiempoTotal += ciclo.DuracionTotal;

            }
            while (tiempoTotal.z > 60)
            {
                tiempoTotal.z -= 60;
                tiempoTotal.y++;
            }
            while (tiempoTotal.y > 60)
            {
                tiempoTotal.y -= 60;
                tiempoTotal.x++;
            }

            return tiempoTotal;
        }
    }

    public enum PomodorosTipos { normal, corto, largo, prueba }

    private PomodorosTipos _tipo;

    public Pomodoro(PomodorosTipos tipoDePomdoro)
    {
        _tipo = tipoDePomdoro;

        switch (tipoDePomdoro)
        {
            case PomodorosTipos.normal:

                CrearPomodoroNormal();

                break;
            case PomodorosTipos.largo:

                CrearPomodoroLargo();

                break;
            case PomodorosTipos.prueba:
                CrearPomodoroPrueba();
                break;
        }
    }


    private void CrearPomodoroPrueba()
    {
        Tempos tempo1 = new Tempos("Tempo 1", new Vector3(0, 0, 3));
        Tempos tempo2 = new Tempos("Tempo 2", new Vector3(0, 0, 2));
        Tempos tempo3 = new Tempos("Tempo 3", new Vector3(0, 0, 1));


        Ciclo ciclo1 = new Ciclo(new List<Tempos> { tempo1, tempo2, tempo3 }, 1, "Ciclo 1");
        Ciclo ciclo2 = new Ciclo(new List<Tempos> { tempo2, tempo3 }, 1 ,"Ciclo 2");
        Ciclo ciclo3 = new Ciclo(new List<Tempos> { tempo2 }, 1, "Ciclo 3");

        ciclosPomodoro = new List<Ciclo> { ciclo1, ciclo2, ciclo3 };
    }

    private void CrearPomodoroNormal()
    {
        Tempos trabajo = new Tempos("Trabajo", new Vector3(0, 30, 0));
        Tempos descanso = new Tempos("Descanso", new Vector3(0, 5, 0));
        Tempos descansoLargo = new Tempos("Descanso Largo", new Vector3(0, 45, 0));


        Ciclo cicloProductivo = new Ciclo(new List<Tempos> { trabajo, descanso }, 4, "Ciclo Productivo");
        Ciclo cicloDeDescanso = new Ciclo(new List<Tempos> { descansoLargo }, 1, "Ciclo de Descanso Merecido");

        ciclosPomodoro = new List<Ciclo> { cicloProductivo, cicloDeDescanso };
    }

    private void CrearPomodoroLargo()
    {
        Tempos trabajo = new Tempos("Trabajo", new Vector3(0, 60, 0));
        Tempos descanso = new Tempos("Descanso", new Vector3(0, 10, 0));
        Tempos descansoLargo = new Tempos("Descanso Largo", new Vector3(0, 45, 0));


        Ciclo cicloProductivo = new Ciclo(new List<Tempos> { trabajo, descanso }, 2);
        Ciclo cicloDeDescanso = new Ciclo(new List<Tempos> { descansoLargo }, 1);

        ciclosPomodoro = new List<Ciclo> { cicloProductivo, cicloDeDescanso };
    }

    public void AsignarEstadoCompletado(bool estado)
    {
        Completado = estado;
    }

    public bool GetEstadoCompletado()
    {
        Completado = true;
        foreach (Ciclo ciclo in ciclosPomodoro)
        {
            if (ciclo.GetEstadoCompletado() == false)
            {
                Completado = false;
            }

        }

        return Completado;
    }
}


public class Ciclo : IEstadoCompletado
{
    private int _repeticionesTotales;
    private int _repeticionesRestantes;
    public List<Tempos> TemposCiclo;
    private bool _completado = false;
    public string Nombre = "";
    public int RepeticionesTotales { get => _repeticionesTotales; set => _repeticionesTotales = (value > 0) ? value : 0; }
    public bool Completado { get => _completado; set => _completado = value; }
    public int RepeticionesRestantes { get => _repeticionesRestantes; set => _repeticionesRestantes = value; }

    public Vector3 DuracionTotal
    {
        get
        {
            Vector3 tiempoTotal = Vector3.zero;

            foreach (Tempos tempo in TemposCiclo)
            {
                tiempoTotal += tempo.TiempoTotal;

            }
            tiempoTotal*= RepeticionesTotales;
            while (tiempoTotal.z > 60)
            {
                tiempoTotal.z -= 60;
                tiempoTotal.y++;
            }
            while (tiempoTotal.y > 60)
            {
                tiempoTotal.y -= 60;
                tiempoTotal.x++;
            }
          
            return  tiempoTotal;
        }
    }


    public Ciclo(List<Tempos> tempos, int repeticiones, string nombre ="")
    {
        TemposCiclo = tempos;
        RepeticionesTotales = repeticiones;
        RepeticionesRestantes = RepeticionesTotales;
        Nombre = nombre;


    }

    public void AsignarEstadoCompletado(bool estado)
    {
        Completado = estado;
    }

    public bool GetEstadoCompletado()
    {

        if (RepeticionesRestantes <= 0)
        {
            Completado = true;
        }
        else
        {
            Completado = false;
        }



        return Completado;
    }

    public bool TryGetEstadoCompletado()
    {
        if (RepeticionesRestantes <= 0)
        {
            Completado = true;
        }
        else
        {
            Completado = false;
            int cantidadDeCompletadosTotales = TemposCiclo.Count;
            int cantidadDeCompletadosActuales = 0;

            foreach (Tempos tempo in TemposCiclo)
            {
                if (tempo.GetEstadoCompletado() == true)
                {
                    cantidadDeCompletadosActuales++;
                }

            }
            if (cantidadDeCompletadosActuales == cantidadDeCompletadosTotales)
            {
                ReiniciarCiclo();
                RepeticionesRestantes--;
            }
        }



        return Completado;
    }



    private void ReiniciarCiclo()
    {
        foreach (Tempos tempo in TemposCiclo)
        {
            tempo.AsignarEstadoCompletado(false);

        }
        PomodoroSistema.ReiniciarNumeroTempo();
    }
}


public class Tempos : IEstadoCompletado
{
    private string _nombre;
    private Vector3 _tiempoTotal;
    private bool _completado = false;

    public Tempos(string nombre, Vector3 tiempoTotal)
    {
        Nombre = nombre;
        this._tiempoTotal = tiempoTotal;
    }
    public string Nombre { get => _nombre; set => _nombre = value; }
    public bool Completado
    {
        get => _completado; set => _completado = value;

    }
    public Vector3 TiempoTotal
    {
        get => _tiempoTotal; set
        {

            if (value.x < 0)
            {
                _tiempoTotal.x = 0;
            }
            if (value.y < 0)
            {
                _tiempoTotal.z = 0;
            }
            if (value.z < 0)
            {
                _tiempoTotal.z = 0;
            }
        }
    }
    public void AsignarEstadoCompletado(bool estado)
    {
        Completado = estado;
    }

    public bool GetEstadoCompletado()
    {
        return Completado;
    }
}


public interface IEstadoCompletado
{
    void AsignarEstadoCompletado(bool estado);
    bool GetEstadoCompletado();
}