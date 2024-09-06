using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderData;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.EventSystems.EventTrigger;

public class Dinosaurio : MonoBehaviour
{
    #region Caracteristicas generales
    [Header("Caracteristicas generales")]
    [SerializeField] private string _nombre;
    [SerializeField] private float _tamaño;
    [SerializeField] private float _velocidad;
    public string Nombre { get => _nombre; set => _nombre = value; }
    [SerializeField] private Especie especie;
    [SerializeField] private Rareza rareza;

    [HideInInspector] private Rigidbody2D rb;
    [HideInInspector] private Animator ac;
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
        ac.SetInteger("Estado de Animo", (int)estadoAnimo);
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
        ac = GetComponent<Animator>();
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
    }


    void Update()
    {
        Position2D = gameObject.transform.position;
        Comportarse(comportamiento);
        setEstadoAnimo();
        if (IsMoving())
        {
            ac.SetBool("Moviendose", true);
        }
        else
        {
            ac.SetBool("Moviendose", false);
        }

        ac.SetInteger("Comportamiento", (int)comportamiento);

        if(dialogosPorDecir.Count > 0) //¿tengo algoq ue decir?
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
    private List<Dialogo> dialogosPorDecir = new List<Dialogo>();
    private Dialogo dialogoActualPorDecir;

    private void Felicitar(Tempos tempos)
    {
        Dialogo dialogoNuevo = new Felicitacion(this, tempos); //proceso mental en analisar la situación y pensar en qué quieres decir
        IncluirDialogosPorDecir(dialogoNuevo);
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


    public void IncluirDialogosPorDecir(Dialogo dialogo)
    {
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
            felicitacion =  dialogoActualPorDecir as Felicitacion;

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

        PanelDialogo.gameObject.SetActive(true);
        StartCoroutine(TypeDialog(dialogoActualPorDecir.mensajeFinal,esperarParaMostrar));
        StartCoroutine(StopDialogo(dialogoActualPorDecir.mensajeFinal.ToCharArray().Length / letterPerSeconds + tiempo));
    }


    IEnumerator StopDialogo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        textoDialogo.text = SubtractStrings(textoDialogo.text,dialogoActualPorDecir.mensajeFinal);

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
    public IEnumerator TypeDialog(string dialog,float firstWait)
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
                //EsperandoParaHablar = true;
                return;
            }
            else
            {
                MostrarDialogoActual(TiempoExtraMostrarDialogo);
                //EsperandoParaHablar = false;
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
    #endregion
}




public class Dialogo
{
    public string mensajeFinal;
    protected List<string> mensajesVariantes;
    public Dinosaurio.EstadoAnimo animoDelDialogo;

    public Dialogo(Dinosaurio emisor)
    {
        this.animoDelDialogo = emisor.getEstadoDeAnimo();

    }


    protected string SelectRandomElement(List<string> items)
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogError("La lista no puede estar vacía o ser nula.");
            return null;
        }

        int index = UnityEngine.Random.Range(0, items.Count); // Genera un índice aleatorio
        return items[index];
    }

}

public class Felicitacion : Dialogo
{
    public enum Rareza { nula, sencillo, desafiante, superior, Top }
    public Rareza rareza;
    public Felicitacion(Dinosaurio emisor, Tempos tempoTerminado) : base(emisor)
    {
        if (tempoTerminado.tiposTempos == TiposTempos.productivo)
        {
            //felicitar simplemente por terminar un tempo productivo

            rareza = Rareza.sencillo;
            #region Mensaje
            switch (animoDelDialogo)
            {
                case Dinosaurio.EstadoAnimo.Euforia:
                    mensajesVariantes = new List<String> {
                  $"¡Estoy extasiado! ¡Has roto todos los récords de productividad! ¡Increíble!",
            $"¡Dino-destacado! ¡Superaste todas las expectativas! ¡Estoy muy emocionado!",
            $"¡Esto es histórico! ¡Tu productividad está por las nubes! ¡Sigue así!",
            $"¡Wow! ¡Terminaste ese tiempo productivo como un verdadero campeón!",
            $"¡Impresionante! ¡No puedo creer lo productivo que has sido!",
            $"¡Estás en racha! ¡Cada vez lo haces mejor!",
            $"¡Increíble energía! ¡Terminaste fuerte y con estilo!",
            $"¡Dino-épico! ¡Tu productividad hoy es algo digno de celebrar!",
            $"¡Bravo! ¡Lo lograste con un nivel de productividad excepcional!",
            $"¡Eres un titán de la productividad! ¡No hay quien te pare!"
                    };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;
                case Dinosaurio.EstadoAnimo.Feliz:
                    mensajesVariantes = new List<String> {
                        $"¡Rugidos de alegría! Has superado tu tiempo productivo en un Pomodoro. ¡Bien hecho!",
            $"¡Rawr! ¡Eres imparable! Has superado tu marca de productividad en un Pomodoro.",
            $"¡Dino-genial! Has superado tu promedio de tiempo productivo. ¡Vamos por más!",
            $"¡Fantástico! ¡Terminaste tu tiempo productivo con gran éxito!",
            $"¡Qué bien lo has hecho! ¡Ese tiempo productivo fue excelente!",
            $"¡Rugido de felicidad! ¡Completaste tu sesión con gran dedicación!",
            $"¡Bravo! ¡Terminaste tu tiempo productivo sin perder el ritmo!",
            $"¡Maravilloso! ¡Tu productividad hoy ha sido de primera!",
            $"¡Qué logro! ¡Finalizaste tu tiempo productivo de forma brillante!",
            $"¡Excelente trabajo! ¡Cada minuto de productividad cuenta!"
                    };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;
                case Dinosaurio.EstadoAnimo.Triste:
                    mensajesVariantes = new List<String> {
                    $"Hoy me siento un poco triste, pero ¡terminaste tu tiempo productivo! ¡Eso es lo importante!",
            $"Mi ánimo está bajo, pero estoy orgulloso de que hayas completado tu período de productividad. ¡Bien hecho!",
            $"Aunque me siento triste, no puedo evitar felicitarte por haber terminado tu tiempo productivo. ¡Lo lograste!",
            $"No estoy en mi mejor momento, pero ver tu esfuerzo me alegra un poco.",
            $"Estoy un poco triste, pero terminar tu tiempo productivo es algo digno de aplauso.",
            $"Hoy no es mi mejor día, pero ¡tú has completado el tuyo con éxito!",
            $"A pesar de mi tristeza, veo que has terminado tu tiempo productivo. ¡Eso me anima!",
            $"Aunque mi ánimo es bajo, felicitarte por tu productividad me da un poco de alegría.",
            $"Me siento algo decaído, pero ver tu logro productivo me da un poco de consuelo.",
            $"Hoy estoy triste, pero no puedo dejar de reconocer tu esfuerzo. ¡Buen trabajo!"
                    };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;

            }
            #endregion


            //Felicitar por superar de ser más productivo de lo normal
            if (EstadisticasManager.TiempoTempoProductivoPromedio < tempoTerminado.TiempoTotal)
            {
                rareza = Rareza.desafiante;
                #region Mensaje
                switch (animoDelDialogo)
                {
                    case Dinosaurio.EstadoAnimo.Euforia:
                        mensajesVariantes = new List<String> {
                        $"¡Estoy extasiado! ¡Has roto todos los récords de productividad! ¡Increíble!",
                        $"¡Dino-destacado! ¡Superaste todas las expectativas! ¡Estoy muy emocionado!",
                        $"¡Esto es histórico! ¡Tu productividad está por las nubes! ¡Sigue así!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Feliz:
                        mensajesVariantes = new List<String> {
                        $"¡Rugidos de alegría! Has superado tu tiempo productivo en un Pomodoro. ¡Bien hecho!",
                        $"¡Rawr! ¡Eres imparable! Has superado tu marca de productividad en un Pomodoro.",
                        $"¡Dino-genial! Has superado tu promedio de tiempo productivo. ¡Vamos por más!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Triste:
                        mensajesVariantes = new List<String> {
                        $"Oh no, parece que hoy no fue tu mejor día. ¡Pero mañana será mejor!",
                        $"No siempre se puede ganar, pero puedes intentarlo otra vez. ¡No te rindas!",
                        "Sé que puedes hacerlo mejor. ¡Vamos a intentarlo de nuevo!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                }
                #endregion

            }

            //Felicitar por superar tu tiempo de productividad muy larga
            if (tempoTerminado.TiempoTotal >= new TimeSpan(1, 30, 0))
            {
                rareza = Rareza.superior;
                switch (animoDelDialogo)
                {
                    case Dinosaurio.EstadoAnimo.Euforia:
                        mensajesVariantes = new List<String> {
            $"¡No me puedo creer que pasaste {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} concentrado! ¡Eso es demasiado!",
            $"¡Increíble! Has estado enfocado por {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Estás a otro nivel!",
            $"¡Eres imparable! Mantener la concentración durante {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} es algo fuera de serie."
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Feliz:
                        mensajesVariantes = new List<String> {
            $"¡Wow! Te concentraste durante {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} ¡Eso es muchísimo! ¡Felicidades!",
            $"¡Gran trabajo! Lograste mantenerte concentrado por {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} ¡Sigue así!",
            $"¡Impresionante! {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de pura productividad. ¡Estás logrando grandes cosas!"
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Triste:
                        mensajesVariantes = new List<String> {
            $"¿Acabaste de estar concentrado durante {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}? ¡Eso es fascinante!",
            $"Aunque estés cansado, mantuviste la concentración por {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡No te desanimes!",
            $"Sé que fue difícil, pero lograste concentrarte durante {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Eso es admirable!"
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                }
            }
            else if (tempoTerminado.TiempoTotal >= new TimeSpan(0, 45, 0))    //Felicitar por terminar un tempo productivo de duración larga
            {
                rareza = Rareza.desafiante;
                #region Mensaje
                switch (animoDelDialogo)
                {
                    case Dinosaurio.EstadoAnimo.Euforia:
                        mensajesVariantes = new List<String> {
                        $"¡ROOOAR! ¡Has alcanzado {tempoTerminado.TiempoTotal.TotalMinutes} minutos de pura concentración! ¡Eso es asombroso! Estoy tan emocionado que casi no puedo contenerme. ¡Eres el mejor cuidador!",
                        $"¡WOW! ¡{tempoTerminado.TiempoTotal.TotalMinutes} minutos de productividad sin parar! ¡Estoy tan eufórico que no puedo dejar de saltar! ¡Eres un campeón, cuidador!",
                        $"¡ROOOAR! ¡Increíble, lograste más de 45 minutos de concentración! ¡Estoy tan emocionado que casi puedo volar! ¡Gracias por darme tanta energía, juntos somos invencibles!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Feliz:
                        mensajesVariantes = new List<String> {
                        $"¡Roar! ¡Qué felicidad ver que has estado concentrado por {tempoTerminado.TiempoTotal.TotalMinutes} minutos! Gracias por ser tan productivo, cuidador. ¡Juntos estamos creciendo más fuertes!",
                        $"¡Rooaar! ¡Has trabajado más de 45 minutos! Eso es increíble. ¡Estoy tan feliz de que me alimentes con tu dedicación y concentración!",
                        $"¡Qué alegría! ¡Has superado el promedio con {tempoTerminado.TiempoTotal.TotalMinutes} minutos de enfoque! Estoy tan feliz de que sigas siendo tan productivo. ¡Gracias por cuidarme tan bien!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Triste:
                        mensajesVariantes = new List<String> {
                        $"Sniff... aunque me pone un poquito triste que hayas pasado tanto tiempo sin descansar, estoy muy orgulloso de que te hayas mantenido concentrado por {tempoTerminado.TiempoTotal.TotalMinutes} minutos. ¡Eso es más que el promedio! Cuida también de ti, cuidador.",
                        $"Grrr... me da un poco de pena verte trabajar tan duro sin una pausa, pero {tempoTerminado.TiempoTotal.TotalMinutes} minutos es un gran logro. ¡Sigue así, pero no olvides recargar energías!",
                        "Oh... aunque me pone triste que estés tan ocupado, sé que {tempoTerminado.TiempoTotal.TotalMinutes} minutos de enfoque es algo impresionante. No olvides que también es importante descansar."
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                }
                #endregion
            }


            //Fecilitar por conseguir mucho tiempo siendo productivo hoy (más que el promedio)

            if (EstadisticasManager.TiempoTotalProductivoHoy > new TimeSpan(5, 0, 0))
            {
                rareza = Rareza.Top;
                #region Mensaje
                switch (animoDelDialogo)
                {
                    case Dinosaurio.EstadoAnimo.Euforia:
                        mensajesVariantes = new List<String> {
                       $"¡Increíble! ¡Superaste todas las expectativas con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad!",
        $"¡Estás imparable! ¡Has trabajado {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} y no hay quien te detenga!",
        $"¡Woohoo! ¡Estás en racha con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de pura productividad!",

            };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Feliz:
                        mensajesVariantes = new List<String> {
                        $"¡Felicidades totales!, estás estudiando hoy más que el promedio ({EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}).",
                        $"¡Rawr!, !Te speraste!,{EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}",
                        $"¡Impresionante! Has el promedio de productividad con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Vamos por más!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                    case Dinosaurio.EstadoAnimo.Triste:
                        mensajesVariantes = new List<String> {
                        $"Oh no, parece que hoy no fue tu mejor día. ¡Pero mañana será mejor!",
                        $"No siempre se puede ganar, pero puedes intentarlo otra vez. ¡No te rindas!",
                        "Sé que puedes hacerlo mejor. ¡Vamos a intentarlo de nuevo!"
                    };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                }
                #endregion
            }

            //felicitar por estar más tiempo productivo hoy que tu promedio 

            if(EstadisticasManager.TiempoTotalProductivoDiarioPromedio < tempoTerminado.TiempoTotal)
            {
                TimeSpan time = EstadisticasManager.TiempoTotalProductivoDiarioPromedio;
                if (time < new TimeSpan(4,30,0))
                {
                    rareza = Rareza.Top;
                }else if(time < new TimeSpan(3, 0, 0))
                {
                    rareza=Rareza.superior;
                }else if(time < new TimeSpan(2, 0, 0))
                {
                    rareza = Rareza.desafiante;
                }else
                {
                    rareza = Rareza.sencillo;
                }

                switch (animoDelDialogo)
                {
                    case Dinosaurio.EstadoAnimo.Euforia:
                        mensajesVariantes = new List<String> {
            $"¡Súper increíble! Rompiste tu récord de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} promedio diario siendo productivo.",
            $"¡Wow! ¡Tu productividad de hoy superó todo lo que habías logrado antes! ¡{EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} es impresionante!",
            $"¡Estás en la cima! No solo alcanzaste, sino que destrozaste tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas. ¡Increíble!"
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;

                    case Dinosaurio.EstadoAnimo.Feliz:
                        mensajesVariantes = new List<String> {
            $"Superaste tu promedio diario siendo productivo por {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}, ¡increíble!",
            $"¡Genial! Hoy tu productividad ha sido excelente, ¡has trabajado más que tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas!",
            $"¡Felicidades! Hoy estuviste más productivo que de costumbre, superando tu promedio de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}. ¡Sigue así!"
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;

                    case Dinosaurio.EstadoAnimo.Triste:
                        mensajesVariantes = new List<String> {
            $"A pesar de las circunstancias, no puedo negar que te mereces una felicitación por mantenerte concentrado más que tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}.",
            $"Hoy no ha sido fácil, pero superaste tu promedio diario de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")}. Eso merece reconocimiento.",
            $"Aunque fue un día difícil, lograste concentrarte más de lo que sueles hacerlo. ¡Tu promedio de {EstadisticasManager.TiempoTotalProductivoDiarioPromedio.ToString(@"h\:mm\:ss")} horas lo dice todo!"
        };
                        mensajeFinal = SelectRandomElement(mensajesVariantes);
                        break;
                }

            }

      
            //Comparar y felicitar por progreso de tempo dedicado a tempos

        }
        else if (tempoTerminado.tiposTempos == TiposTempos.descanso)
        {
            rareza = Rareza.nula;
            #region Mensaje
            switch (animoDelDialogo)
            {
                case Dinosaurio.EstadoAnimo.Euforia:
                    mensajesVariantes = new List<String> {
            $"¡Increíble! ¡Superaste todas las expectativas con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad! Te mereces este descanso.",
            $"¡Estás imparable! ¡Has trabajado {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} y no hay quien te detenga! Tómate un respiro, te lo ganaste.",
            $"¡Woohoo! ¡Estás en racha con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de pura productividad! Este descanso es más que merecido.",
        };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;

                case Dinosaurio.EstadoAnimo.Feliz:
                    mensajesVariantes = new List<String> {
            $"¡Felicidades totales! Estás estudiando hoy más que el promedio ({EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}). Ahora, relájate un poco.",
            $"¡Rawr! ¡Te superaste con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")} de productividad! ¡Disfruta de este descanso!",
            $"¡Impresionante! Has superado el promedio de productividad con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. ¡Tómate un merecido descanso!",
        };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;

                case Dinosaurio.EstadoAnimo.Triste:
                    mensajesVariantes = new List<String> {
            $"Oh no, parece que hoy no fue tu mejor día con {EstadisticasManager.TiempoTotalProductivoHoy.ToString(@"h\:mm\:ss")}. Pero este descanso te ayudará a recargar energías para mañana.",
            $"No siempre se puede ganar, pero puedes intentarlo otra vez. ¡No te rindas! Tómate este descanso para volver más fuerte.",
            $"Sé que puedes hacerlo mejor. ¡Vamos a intentarlo de nuevo después de un buen descanso!",
        };
                    mensajeFinal = SelectRandomElement(mensajesVariantes);
                    break;
            }
            #endregion

        }
    }




}



