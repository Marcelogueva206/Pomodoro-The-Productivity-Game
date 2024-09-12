using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;

public class Dinosaurio : MonoBehaviour
{
    #region Caracteristicas generales
    [Header("Caracteristicas generales")]
    [SerializeField] private string _nombre;
    [SerializeField] private float _tamaño;
    [SerializeField] private float _velocidad;
    public string Nombre { get => _nombre; set => _nombre = value; }
    [HideInInspector] public Especie _Especie { get => especie; }

    [SerializeField] private Especie especie;
    [SerializeField] private Rareza rareza;

    [HideInInspector] private Rigidbody2D rb;
    [HideInInspector] private Animator AnimadorCaracter;
  
    [HideInInspector] private bool LookAtRight;
    [HideInInspector] private SpriteRenderer spriteRenderer;
    #endregion
    #region Sistema de emociones
    [Header("Sistema de Emociones")]
    [SerializeField] private EstadoAnimo estadoAnimo;

    public void MejorarEstadoAnimo(bool QuieresMejorar)
    {
        if (QuieresMejorar == true)
        {
            if (estadoAnimo == EstadoAnimo.Feliz)
            {
                CambiarEstadoAnimo(EstadoAnimo.Euforia);
            }
            if (estadoAnimo == EstadoAnimo.Triste)
            {
                CambiarEstadoAnimo(EstadoAnimo.Feliz);
            }
        }
        else
        {
            if (estadoAnimo == EstadoAnimo.Euforia || estadoAnimo == EstadoAnimo.Feliz)
            {
                CambiarEstadoAnimo(EstadoAnimo.Triste);
            }

            if (estadoAnimo == EstadoAnimo.Triste)
            {
                CambiarEstadoAnimo(EstadoAnimo.Deprimido);
            }
        }
    }

    private void CambiarEstadoAnimo(EstadoAnimo estado)
    {
        estadoAnimo = estado;
        if (estadoAnimo == EstadoAnimo.Deprimido)
        {
            CambiarComportamiento(Comportamiento.Deprimirse);
        }
    }

    private void setEstadoAnimo()
    {
        AnimadorCaracter.SetInteger("Estado de Animo", (int)estadoAnimo);
    }

    public EstadoAnimo getEstadoDeAnimo()
    {
        return estadoAnimo;
    }

    #endregion

    #region Unity métodos
    private void Awake()
    {
        //PomodoroSistema.PomodoroTerminado += FelicitarPomodoroTerminado;
        //PomodoroSistema.CicloTerminado += FelicitarCicloTerminado;
        //PomodoroSistema.TemposTerminado += FelicitarTempoTerminado;
        //Contador.TempoIniciadoPorUsuario += MotivarTempoIniciado;
        //Contador.CicloIniciadoPorUsuario += MotivarCicloIniciado;
        //Contador.PomodoroIniciadoPorUsuario += MotivarPomodoroIniciado;

        PomodoroSistema.TemposTerminado += Felicitar;

        rb = GetComponent<Rigidbody2D>();
        AnimadorCaracter = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camara = Camera.main.gameObject;
        FondoDeDialogo = PanelDialogo.transform.Find("Fondo dialogo").GetComponent<RectTransform>();
        PosicionOriginalPanelDeDialogo = PanelDialogo.anchoredPosition;
        RectTransformTextoDialogo = textoDialogo.GetComponent<RectTransform>();
        PosicionOriginalTextoDeDialogo = RectTransformTextoDialogo.anchoredPosition;
        PanelDialogo.gameObject.SetActive(false);
    }


    void Update()
    {
        Position2D = gameObject.transform.position;
        Comportarse(comportamiento);
        setEstadoAnimo();
        if (IsMoving())
        {
            AnimadorCaracter.SetBool("Moviendose", true);
        }
        else
        {
            AnimadorCaracter.SetBool("Moviendose", false);
        }


        if(AnimadorGloboDeTexto.gameObject.activeSelf)
        {
            if (PensandoDialogo == true)
            {
                AnimadorGloboDeTexto.SetBool("Pensando", true);
            }
            else
            {
                AnimadorGloboDeTexto.SetBool("Pensando", false);
            }
        }
 
    

        AnimadorCaracter.SetInteger("Comportamiento", (int)comportamiento);

        if (dialogosPorDecir.Count > 0) //¿tengo algoq ue decir?
        {
            CambiarComportamiento(Comportamiento.Hablar);
        }
    }
    #endregion
    #region Sistema de dialogo 
    [Header("Sistema de dialogos")]
    [SerializeField] private float esperarParaMostrar = 0.01f;
    [SerializeField] private float letterPerSeconds;
    [SerializeField] private float TiempoExtraMostrarDialogo = 5f;
    public List<Dialogo> dialogosPorDecir = new List<Dialogo>();
    private Dialogo dialogoActualPorDecir;
    [HideInInspector] private bool PensandoDialogo = false;

    private void Felicitar(Tempos tempos)
    {
        Dialogo dialogoNuevo = new Felicitacion(this, tempos); //proceso mental en analisar la situación y pensar en qué quieres decir

        StartCoroutine(IncluirDialogosPorDecir(dialogoNuevo));
        PanelDialogo.gameObject.SetActive(true);
    }
    #region Experimentación dialogo
    //public void FelicitarTempoTerminado(Tempos tempo)
    //{
    //    MostrarDialogo(10f, $"pequeñas felicidades por terminar el tempo, las cosas son poco a poco");
    //}
    //public void FelicitarCicloTerminado(Ciclo ciclo)
    //{
    //    MostrarDialogo(10f, $"Felicidades por acabar tremendo ciclo, cada vez estás más cerca");
    //}
    //public void FelicitarPomodoroTerminado(Pomodoro pomodoro)
    //{
    //    MostrarDialogo(10f, $"FELICIDADES por terminar el pomodoro llamado {pomodoro.Nombre} el cual duró {pomodoro.DuracionTotal}");
    //}
    //public void MotivarTempoIniciado(Tempos tempo)
    //{
    //    MostrarDialogo(10f, $"WOW, enserio vas a dedicar tanto tiempo? buena suete!!!");
    //}
    //public void MotivarCicloIniciado(Ciclo ciclo)
    //{
    //    MostrarDialogo(10f, $"!!!!!!");
    //}
    //public void MotivarPomodoroIniciado(Pomodoro pomodoro)
    //{
    //    MostrarDialogo(10f, $"*******");
    //} 
    #endregion


    public IEnumerator IncluirDialogosPorDecir(Dialogo dialogo)
    {
        PensandoDialogo = true;
        yield return StartCoroutine(TestAI.Gemini.UseGeminiAI(dialogo.Prompt));
       
        dialogo.mensajeFinal = TestAI.Gemini.response;
        dialogosPorDecir.Add(dialogo);
    }




    public void MostrarDialogoActual(float tiempo)
    {

        if (dialogosPorDecir.Count > 0)
        {
            dialogoActualPorDecir = dialogosPorDecir[0];
        }
        else
        {
            Debug.Log("Desbordamiento de dialogos");
            return;
        }

        Felicitacion felicitacion;
        if (dialogoActualPorDecir is Felicitacion)
        {
            felicitacion = dialogoActualPorDecir as Felicitacion;

            switch (felicitacion.rareza)
            {
                case Felicitacion.Rareza.nula:
                    FondoDeDialogo.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    break;
                case Felicitacion.Rareza.sencillo:
                    FondoDeDialogo.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    textoDialogo.color = Color.gray;
                    break;
                case Felicitacion.Rareza.desafiante:
                    FondoDeDialogo.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
                    break;
                case Felicitacion.Rareza.superior:
                    FondoDeDialogo.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
                    textoDialogo.color = Color.black + Color.blue;
                    break;
                case Felicitacion.Rareza.Top:
                    FondoDeDialogo.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                    textoDialogo.color = Color.yellow;
                    break;
            }
        }

        StartCoroutine(TypeDialog(dialogoActualPorDecir.mensajeFinal, esperarParaMostrar));
        StartCoroutine(StopDialogo(dialogoActualPorDecir.mensajeFinal.ToCharArray().Length / letterPerSeconds + tiempo));
    }


    IEnumerator StopDialogo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        textoDialogo.text = SubtractStrings(textoDialogo.text, dialogoActualPorDecir.mensajeFinal);

        dialogosPorDecir.Remove(dialogoActualPorDecir);
        dialogoActualPorDecir = null;

        if (textoDialogo.text.Trim() == "")
        {
            PanelDialogo.gameObject.SetActive(false);
            CambiarComportamiento(Comportamiento.Merodear);
        }
    }
    #region Métodos de apoyo

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
    public IEnumerator TypeDialog(string dialog, float firstWait)
    {
        yield return new WaitForSeconds(firstWait);
        foreach (var letter in dialog.ToCharArray())
        {
            textoDialogo.text += letter;
            yield return new WaitForSeconds(1f / letterPerSeconds);
        }

    }

    #endregion
    #endregion
    #region Sistema de comportamiento
    [Header("Sistema de comportamiento")]
    [SerializeField] private Comportamiento comportamiento;
    [HideInInspector] private bool moverse = true;
    //[HideInInspector] private bool EsperandoParaHablar = false;
    //[HideInInspector] private bool YaDijeLoQueQueriaDecir = true;
    [HideInInspector] private Vector2 Destino = new Vector2(0, 0);
    [HideInInspector] private Vector2 Position2D;
    private void Comportarse(Comportamiento comportamiento)
    {
        switch (comportamiento)
        {
            case Comportamiento.Merodear:
                //if (EsperandoParaHablar == true)
                //{
                //    CambiarComportamiento(Comportamiento.Hablar);
                //}

                MovimientoMerodear();
                break;
            case Comportamiento.Hablar:
                DetenerseInstantaneamente();
                MirarAlJugadorAlHablar();
                break;
            case Comportamiento.Celebrar:
                DetenerseInstantaneamente();
                break;
            case Comportamiento.Deprimirse:
                DetenerseInstantaneamente();
                break;
        }

    }

    [HideInInspector] private Vector2 PosicionOriginalPanelDeDialogo;
    [HideInInspector] private Vector2 PosicionOriginalTextoDeDialogo;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float rangoDetectarHablando;
    private bool OtroEstaHablando()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangoDetectarHablando, detectionLayer);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.GetComponent<Dinosaurio>().comportamiento == Comportamiento.Hablar)
            {
                return true;
            }
        }

        return false;
    }

    private void MirarAlJugadorAlHablar()
    {

        if (Camara.transform.position.x > Position2D.x)
        {
            //está hacia la izquierda de la camara
            FondoDeDialogo.localScale = new Vector3(-1, FondoDeDialogo.localScale.y, FondoDeDialogo.localScale.z);
            spriteRenderer.flipX = true;
            SetRectTransformValues(PanelDialogo, -PosicionOriginalPanelDeDialogo.x, PanelDialogo.anchoredPosition.y);
            SetRectTransformValues(RectTransformTextoDialogo, -PosicionOriginalTextoDeDialogo.x, RectTransformTextoDialogo.anchoredPosition.y);

        }
        else
        {
            FondoDeDialogo.localScale = new Vector3(1, FondoDeDialogo.localScale.y, FondoDeDialogo.localScale.z);
            spriteRenderer.flipX = false;
            SetRectTransformValues(PanelDialogo, PosicionOriginalPanelDeDialogo.x, PanelDialogo.anchoredPosition.y);
            SetRectTransformValues(RectTransformTextoDialogo, PosicionOriginalTextoDeDialogo.x, RectTransformTextoDialogo.anchoredPosition.y);
            //está hacia la derecha de la camara
        }

        if (Camara.transform.position.y > Position2D.y)
        {
            //está hacia arriba la camara
            FondoDeDialogo.localScale = new Vector3(FondoDeDialogo.localScale.x, 1, FondoDeDialogo.localScale.z);
            SetRectTransformValues(PanelDialogo, PanelDialogo.anchoredPosition.x, PosicionOriginalPanelDeDialogo.y);
            SetRectTransformValues(RectTransformTextoDialogo, RectTransformTextoDialogo.anchoredPosition.x, PosicionOriginalTextoDeDialogo.y);
        }
        else
        {
            FondoDeDialogo.localScale = new Vector3(FondoDeDialogo.localScale.x, -1, FondoDeDialogo.localScale.z);
            SetRectTransformValues(PanelDialogo, PanelDialogo.anchoredPosition.x, -PosicionOriginalPanelDeDialogo.y);
            SetRectTransformValues(RectTransformTextoDialogo, RectTransformTextoDialogo.anchoredPosition.x, -PosicionOriginalTextoDeDialogo.y);
            //está hacia abajo la camara
        }

    }


    public void CambiarComportamiento(Comportamiento comportamiento)
    {

        if (comportamiento == Comportamiento.Hablar)
        {
            if (OtroEstaHablando() == true)
            {
                return;
            }
            else
            {
                PensandoDialogo = false;
                MostrarDialogoActual(TiempoExtraMostrarDialogo);
            }

        }
        DetenerseInstantaneamente();
        this.comportamiento = comportamiento;

    }

    public void MovimientoMerodear()
    {
        if (Destino == Vector2.zero)
        {
            Vector2 PosiciónAleatoria = new(UnityEngine.Random.Range(Gamificacion.MinX, Gamificacion.MaxX), UnityEngine.Random.Range(Gamificacion.MinY, Gamificacion.MaxY));
            Destino = PosiciónAleatoria; //necesario para empezar a caminar
        }

        if (Vector2.Distance(Destino, Position2D) < 1)
        {
            Vector2 PosiciónAleatoria = new(UnityEngine.Random.Range(Gamificacion.MinX, Gamificacion.MaxX), UnityEngine.Random.Range(Gamificacion.MinY, Gamificacion.MaxY));
            Destino = PosiciónAleatoria;
            Detenerse(UnityEngine.Random.Range(0f, 10f));

        }
        Perseguir();

    }
    private void Perseguir()
    {
        if (moverse)
        {
            Vector2 dirrecion = Destino - Position2D;
            if (dirrecion.x < 0f)
            {
                LookAtRight = false;
            }
            else
            {
                LookAtRight = true;
            }

            if (LookAtRight)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }


            rb.AddForce((Destino - Position2D).normalized * _velocidad * Time.deltaTime);
        }
        else
        {
            DetenerseInstantaneamente();
        }
    }

    private void DetenerseInstantaneamente()
    {
        rb.velocity = Vector2.zero; rb.angularVelocity = 0;
    }

    private bool IsMoving()
    {
        // Verifica la velocidad del Rigidbody2D en los ejes x e y
        if (rb.velocity.x != 0 || rb.velocity.y != 0)
        {
            return true; // Se está moviendo
        }
        else
        {
            return false; // No se está moviendo
        }
    }
    public void Detenerse(float tiempo)
    {

        moverse = false;
        DetenerseInstantaneamente();
        StartCoroutine(EjecutarEn(tiempo, StopDetener));
    }
    private void StopDetener()
    {
        moverse = true;
    }

    public void SetRectTransformValues(RectTransform rectTransform, float posX, float posY)
    {
        // Asignar Pos X y Pos Y
        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }

    #endregion
    #region Declaración de variables enum

    public enum Rareza { Comun, Especial, Epica, Legendaria }
    public enum Especie { TiranosaurioRex, Estegosaurio, Triceratops, Velociraptor, Espinosaurus, Allosaurus, Megalosaurio, Diplodocus, Anquilosaurio }

    public enum Comportamiento { Merodear, Hablar, Celebrar, Deprimirse }

    public enum EstadoAnimo { Triste, Feliz, Euforia, Deprimido }


    #endregion
    #region Métodos extras
    public IEnumerator EjecutarEn(float tiempo, Action metodo)
    {
        yield return new WaitForSeconds(tiempo);
        metodo();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(Destino, new Vector2(0.1f, 0.1f));
        Gizmos.DrawRay(transform.position, Destino - Position2D);

        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.1f);
        Gizmos.DrawSphere(transform.position, rangoDetectarHablando);

    }
    #endregion
    #region Variables extras
    [Header("Extras")]
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private RectTransform RectTransformTextoDialogo;
    [SerializeField] private RectTransform PanelDialogo;
    [HideInInspector] private RectTransform FondoDeDialogo;
    [HideInInspector] private GameObject Camara;
    [SerializeField] public Animator AnimadorGloboDeTexto;
    #endregion
}




public class Dialogo
{
    public string mensajeFinal;
    protected Dinosaurio.EstadoAnimo animoDelDialogo;
    protected string id;

    //Sistema de uso de AI Gemini
    protected string contextoGeneral = "";
    protected string contextoEspecifico = "";
    protected string tarea = "";
    public string Prompt { get => contextoGeneral + contextoEspecifico + tarea; }
    public string Id { get => id; }

    protected List<String> idValidadas;
    public Dialogo(Dinosaurio emisor, string id = "")
    {
        this.id = id;
        contextoGeneral = "Interpretas a una mascota que acompaña al usario en su trabajo. Te comportas feliz o triste según la productividad del usuario. Tú output no debe superar los 200 caracteres";
        contextoGeneral += ".Eres un pequeño " + emisor._Especie.ToString();
        contextoGeneral += ".Estás " + emisor.getEstadoDeAnimo().ToString();
        this.animoDelDialogo = emisor.getEstadoDeAnimo();

    }

}

public class Felicitacion : Dialogo
{
    public enum Rareza { nula, sencillo, desafiante, superior, Top }
    public Rareza rareza;
    public Felicitacion(Dinosaurio emisor, Tempos tempoTerminado) : base(emisor)
    {
        tarea = "Felicitalo por lo que logró el usuario, incluso si tu estadode ánimo es triste";

        if (tempoTerminado.tiposTempos == TiposTempos.productivo)
        {

            //el usuario termino un pomodoro normal
            idValidadas.Add("001");
            if ("001" == id||"" == id)
            {        
                rareza = Rareza.sencillo;
                contextoEspecifico += $".El usuario logró superar estar concentrado un total de {tempoTerminado.TiempoTotal.ToString(@"h\:mm\:ss")}";
            }
          
         
            //Felicitar por superar de ser más productivo de lo normal
            if (EstadisticasManager.TiempoTempoProductivoPromedio < tempoTerminado.TiempoTotal)
            {
                idValidadas.Add("002");
                if ("002" == id || "" == id)
                {                 
                    rareza = Rareza.desafiante;
                    contextoEspecifico += $".El usuario logró superar su promedio productivo diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}";
                }

               

            }


            //Felicitar por superar tu tiempo de productividad muy larga
            if (tempoTerminado.TiempoTotal >= new TimeSpan(1, 30, 0))
            {
                idValidadas.Add("003");
                if ("003" == id || "" == id)
                {    
                    rareza = Rareza.superior;
                    contextoEspecifico += $".El usuario logró estar concentrado durante el largo periodo consecutivo de más 1 hora y 30 minutos. El usuario estuvo {tempoTerminado.TiempoTotal.ToString(@"h\:mm\:ss")} en total";
                }
              


            }
            //else if (tempoTerminado.TiempoTotal >= new TimeSpan(0, 45, 0))    //Felicitar por terminar un tempo productivo de duración larga
            //{
            //    id = "004";
            //    rareza = Rareza.desafiante;
            //    contextoEspecifico += $".El usuario logró estar concentrado durante el largo periodo consecutivo de más 45 minutos. El usuario estuvo {tempoTerminado.TiempoTotal.ToString(@"h\:mm\:ss")} en total";

            //}
            #region Anterior trabajo

            //    //Fecilitar por conseguir mucho tiempo siendo productivo hoy (más que el promedio)

            //    if (EstadisticasManager.TiempoTotalProductivoHoy > new TimeSpan(5, 0, 0))
            //    {
            //        rareza = Rareza.Top;
            //        #region Mensaje
            //        switch (animoDelDialogo)
            //        {
            //            case Dinosaurio.EstadoAnimo.Euforia:
            //                mensajesVariantes = new List<String> {
            //               $"¡Increíble! ¡Superaste todas las expectativas con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad!",
            //$"¡Estás imparable! ¡Has trabajado {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} y no hay quien te detenga!",
            //$"¡Woohoo! ¡Estás en racha con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de pura productividad!",

            //    };
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;
            //            case Dinosaurio.EstadoAnimo.Feliz:
            //                mensajesVariantes = new List<String> {
            //                $"¡Felicidades totales!, estás estudiando hoy más que el promedio ({EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}).",
            //                $"¡Rawr!, !Te speraste!,{EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}",
            //                $"¡Impresionante! Has el promedio de productividad con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Vamos por más!"
            //            };
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;
            //            case Dinosaurio.EstadoAnimo.Triste:
            //                mensajesVariantes = new List<String> {
            //                $"Oh no, parece que hoy no fue tu mejor día. ¡Pero mañana será mejor!",
            //                $"No siempre se puede ganar, pero puedes intentarlo otra vez. ¡No te rindas!",
            //                "Sé que puedes hacerlo mejor. ¡Vamos a intentarlo de nuevo!"
            //            };
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;
            //        }
            //        #endregion
            //    }

            //    //felicitar por estar más tiempo productivo hoy que tu promedio 

            //    if(EstadisticasManager.TiempoTotalProductivoDiarioPromedio < tempoTerminado.TiempoTotal)
            //    {
            //        TimeSpan time = EstadisticasManager.TiempoTotalProductivoDiarioPromedio;
            //        if (time < new TimeSpan(4,30,0))
            //        {
            //            rareza = Rareza.Top;
            //        }else if(time < new TimeSpan(3, 0, 0))
            //        {
            //            rareza=Rareza.superior;
            //        }else if(time < new TimeSpan(2, 0, 0))
            //        {
            //            rareza = Rareza.desafiante;
            //        }else
            //        {
            //            rareza = Rareza.sencillo;
            //        }

            //        switch (animoDelDialogo)
            //        {
            //            case Dinosaurio.EstadoAnimo.Euforia:
            //                mensajesVariantes = new List<String> {
            //    $"¡Súper increíble! Rompiste tu récord de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} promedio diario siendo productivo.",
            //    $"¡Wow! ¡Tu productividad de hoy superó todo lo que habías logrado antes! ¡{EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} es impresionante!",
            //    $"¡Estás en la cima! No solo alcanzaste, sino que destrozaste tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas. ¡Increíble!"
            //};
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;

            //            case Dinosaurio.EstadoAnimo.Feliz:
            //                mensajesVariantes = new List<String> {
            //    $"Superaste tu promedio diario siendo productivo por {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}, ¡increíble!",
            //    $"¡Genial! Hoy tu productividad ha sido excelente, ¡has trabajado más que tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas!",
            //    $"¡Felicidades! Hoy estuviste más productivo que de costumbre, superando tu promedio de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}. ¡Sigue así!"
            //};
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;

            //            case Dinosaurio.EstadoAnimo.Triste:
            //                mensajesVariantes = new List<String> {
            //    $"A pesar de las circunstancias, no puedo negar que te mereces una felicitación por mantenerte concentrado más que tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}.",
            //    $"Hoy no ha sido fácil, pero superaste tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}. Eso merece reconocimiento.",
            //    $"Aunque fue un día difícil, lograste concentrarte más de lo que sueles hacerlo. ¡Tu promedio de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas lo dice todo!"
            //};
            //                mensajeFinal = SelectRandomElement(mensajesVariantes);
            //                break;
            //        }

            //    }


            //Comparar y felicitar por progreso de tempo dedicado a tempos 
            #endregion
        }
        #region Anterior version
        //else if (tempoTerminado.tiposTempos == TiposTempos.descanso)
        //{
        //    rareza = Rareza.nula;
        //    #region Mensaje
        //    switch (animoDelDialogo)
        //    {
        //        case Dinosaurio.EstadoAnimo.Euforia:
        //            mensajesVariantes = new List<String> {
        //    $"¡Increíble! ¡Superaste todas las expectativas con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad! Te mereces este descanso.",
        //    $"¡Estás imparable! ¡Has trabajado {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} y no hay quien te detenga! Tómate un respiro, te lo ganaste.",
        //    $"¡Woohoo! ¡Estás en racha con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de pura productividad! Este descanso es más que merecido.",
        //};
        //            mensajeFinal = SelectRandomElement(mensajesVariantes);
        //            break;

        //        case Dinosaurio.EstadoAnimo.Feliz:
        //            mensajesVariantes = new List<String> {
        //    $"¡Felicidades totales! Estás estudiando hoy más que el promedio ({EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}). Ahora, relájate un poco.",
        //    $"¡Rawr! ¡Te superaste con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad! ¡Disfruta de este descanso!",
        //    $"¡Impresionante! Has superado el promedio de productividad con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Tómate un merecido descanso!",
        //};
        //            mensajeFinal = SelectRandomElement(mensajesVariantes);
        //            break;

        //        case Dinosaurio.EstadoAnimo.Triste:
        //            mensajesVariantes = new List<String> {
        //    $"Oh no, parece que hoy no fue tu mejor día con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. Pero este descanso te ayudará a recargar energías para mañana.",
        //    $"No siempre se puede ganar, pero puedes intentarlo otra vez. ¡No te rindas! Tómate este descanso para volver más fuerte.",
        //    $"Sé que puedes hacerlo mejor. ¡Vamos a intentarlo de nuevo después de un buen descanso!",
        //};
        //            mensajeFinal = SelectRandomElement(mensajesVariantes);
        //            break;
        //    }
        //    #endregion

        //} 
        #endregion
    }




}



