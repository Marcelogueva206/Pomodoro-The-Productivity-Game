using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;

public class Dinosaurio : MonoBehaviour
{
    /// <summary>
    /// Su función principal es servir como Proposito y como Dominio:
    /// Proposito: debe apoyar al usuario mejorando su proposito
    /// 1.- Recordando al usuario por qué está invirtiendo su tiempo y vale la pena (en lograr un sueño o una meta a largo plazo) y de desarrollar alguno
    /// 2.- Incentivar al usuario de el impacto de utilidad actual y personal de lo que está trabajando le va a ser útil para sus sueños, además de asignarse obejtivos
    /// 3.- Inspirar al usuario con casos de éxito o situaciones similares que viva el usuario, además de como ellos se motivaron y lograron
  
    /// 
    /// Dominio: debe apoyar al usuario motivandolo con su progreso de ser más productivo
    /// 1.- Felicitar a los usuario después de lograr una  sesion, un pomodoro, un objetivo o un sueño
    /// 2.- Dividir los sueños del usuario para hacerlos más manejables y decirle que está cerca de lograr uno cerca
    /// 3.- Demostrarle su progreso del usuario y que su esfuerzo valió la pena 
    /// </summary>

    public string Nombre { get => _nombre; set => _nombre = value; }
    [SerializeField] private Especie _specie;
    [SerializeField] private Rareza _rareza;
    [SerializeField] private Genero _genero;
    [SerializeField] private Personalidad _personalidad;
    [SerializeField] private Comportamiento _comportamiento;
    [SerializeField] private TextMeshProUGUI textoDialogo;

    private void Awake()
    {
        PomodoroSistema.PomodoroTerminado += FelicitarPomodoroTerminado;
        PomodoroSistema.CicloTerminado += FelicitarCicloTerminado;
        PomodoroSistema.TemposTerminado += FelicitarTempoTerminado;
        Contador.TempoIniciadoPorUsuario += MotivarTempoIniciado;
        Contador.CicloIniciadoPorUsuario += MotivarCicloIniciado;
        Contador.PomodoroIniciadoPorUsuario += MotivarPomodoroIniciado;
        _rb = GetComponent<Rigidbody2D>();
        
    }
    void Start()
    {


        //TempoTerminado  = () =>  Debug.Log("hola"); 
        //TempoTerminado  = delegate () { Debug.Log("hola"); }; 

        //TempoTerminado = new EventosContador(FelicitarUsuario);


    }
  
    void Update()
    {
        Position2D = gameObject.transform.position;
        Comportarse(_comportamiento);
    }

    [SerializeField] private string _nombre;
    [SerializeField] private float _peso;
    [SerializeField] private float _consumo;
    [SerializeField] private float _tamaño;
    [SerializeField] private float _velocidad;
    [HideInInspector] private bool moverse = true;


    public enum Genero {Femenino, Masculino}
    public enum Personalidad { Optimista, Tonta, Orgullosa, Molestosa, Deprimida, Timida, Peresosa, Cariñosa }

    /// <summary>
    ///  Teanquila = Dandere
    ///  Optimista = Deredere
    ///  TontaAmorosa = Bakadere
    ///  Orgullosa = Himedere/Oujidere
    ///  Molestosa = Sadodere
    ///  Deprimida = Shundere
    ///  Aburrida = Darudere
    ///  Timida = Bodere
    /// </summary>

    public enum Rareza {Comun, Especial, Epica, Legendaria }
    public enum Especie { TiranosaurioRex, Estegosaurio, Triceratops, Velociraptor, Espinosaurus, Allosaurus, Megalosaurio, Diplodocus, Anquilosaurio }

    public enum Comportamiento {Merodear, Hablar, Dormir, Jugar, Quieto }


    private void Comportarse(Comportamiento comportamiento)
    {
        switch (comportamiento)
        {
            case Comportamiento.Merodear:

                MovimientoMetodear();


                break;
            case Comportamiento.Hablar:
                break;
            case Comportamiento.Dormir:
                break;
            case Comportamiento.Jugar:
                break;
            case Comportamiento.Quieto:
                break;
        }

    }



    public void FelicitarTempoTerminado(Tempos tempo)
    {
        MostrarDialogo(10f, $"pequeñas felicidades por terminar el tempo, las cosas son poco a poco");
    }


    public void FelicitarCicloTerminado(Ciclo ciclo)
    {
        MostrarDialogo(10f, $"Felicidades por acabar tremendo ciclo, cada vez estás más cerca");
    }

    public void FelicitarPomodoroTerminado(Pomodoro pomodoro)
    {
       MostrarDialogo(10f, $"FELICIDADES por terminar el pomodoro llamado {pomodoro.Nombre} el cual duró {pomodoro.DuracionTotal}") ;
    }

    public void MotivarTempoIniciado(Tempos tempo)
    {
        MostrarDialogo(10f, $"WOW, enserio vas a dedicar tanto tiempo? buena suete!!!");
    }
    public void MotivarCicloIniciado(Ciclo ciclo)
    {
        MostrarDialogo(10f, $"!!!!!!");
    }

    public void MotivarPomodoroIniciado(Pomodoro pomodoro)
    {
        MostrarDialogo(10f, $"*******");
    }

    public void MostrarDialogo(float tiempo,string dialogo)
    {
        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text += "\n\n"+dialogo;
        StartCoroutine(StopDialogo(tiempo,dialogo));
    }


    IEnumerator StopDialogo(float tiempo,  string dialogo)
    {
        yield return new WaitForSeconds(tiempo);
        textoDialogo.text = SubtractStrings(textoDialogo.text, "\n\n"+dialogo);

        if(textoDialogo.text.Trim() == "")
        {
            textoDialogo.gameObject.SetActive(false);
        }
    }
 
    string SubtractStrings(string str1, string str2)
    {
        // Buscar la subsecuencia str2 en str1
        int index = str1.IndexOf(str2);
        if (index != -1)
        {
            // Eliminar la subsecuencia si se encuentra
            str1 = str1.Remove(index, str2.Length);
        }

        // Retornar el string resultante
        return str1;
    }


    [HideInInspector]private Vector2 Destino; 
    [HideInInspector]private Vector2 Position2D; 
    [HideInInspector]private Rigidbody2D _rb; 
    public void MovimientoMetodear()
    {    
        if(Destino == Vector2.zero)
        {
            Vector2 PosiciónAleatoria = new(UnityEngine.Random.Range(Gamificacion.MinX, Gamificacion.MaxX), UnityEngine.Random.Range(Gamificacion.MinY, Gamificacion.MaxY));
            Destino = PosiciónAleatoria; //necesario para empezar a caminar
        }

        if (Vector2.Distance(Destino, Position2D) < 1)
        {
            Vector2 PosiciónAleatoria = new(UnityEngine.Random.Range(Gamificacion.MinX, Gamificacion.MaxX), UnityEngine.Random.Range(Gamificacion.MinY, Gamificacion.MaxY));
            Destino = PosiciónAleatoria;
            Detenerse(UnityEngine.Random.Range(0f,10f));

        }
        Perseguir();

    }

    public void Detenerse(float tiempo)
    {

        moverse = false;
        StartCoroutine(EjecutarEn(tiempo,StopDetener));
    }

    private void StopDetener()
    {
        moverse=true;
    }

    public IEnumerator EjecutarEn(float tiempo,Action metodo)
    {
        yield return new WaitForSeconds(tiempo);
        metodo();
    }

    private void Perseguir()
    {
        if (moverse)
        {
            _rb.AddForce((Destino - Position2D).normalized * _velocidad * Time.deltaTime);
        }else
        {
            _rb.velocity = Vector2.zero; _rb.angularVelocity = 0;
        }
    }




}
