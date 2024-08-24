using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

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

            if(estadoAnimo == EstadoAnimo.Triste)
            {
                CambiarEstadoAnimo(EstadoAnimo.Deprimido);
            }
        }
    }

    private void CambiarEstadoAnimo(EstadoAnimo estado)
    {
        estadoAnimo = estado;
        if(estadoAnimo == EstadoAnimo.Deprimido)
        {
            CambiarComportamiento(Comportamiento.Deprimirse);
        }
    }

    private void setEstadoAnimo()
    {
        ac.SetInteger("Estado de Animo", (int)estadoAnimo);
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
    }
    #endregion
    #region Sistema de dialogo 
    [Header("Sistema de dialogos")]
    [SerializeField] private float letterPerSeconds;
    [SerializeField] private float TiempoExtraMostrarDialogo = 5f;
    private Dialogo dialogoPorDecir;

    private void Felicitar(Tempos tempos)
    {
        dialogoPorDecir = new Dialogo($"!Pequeñas felicidades por terminar el tiempo¡ las cosas son poco a poco"); //proceso mental en analisar la situación y pensar en qué quieres decir
        CambiarComportamiento(Comportamiento.Hablar);
      
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

    

    public void MostrarDialogo(float tiempo, string dialogo)
    {
        PanelDialogo.gameObject.SetActive(true);

        StartCoroutine(TypeDialog("\n\n" + dialogo));
        //textoDialogo.text += "\n\n" + dialogo;
        StartCoroutine(StopDialogo(dialogo.ToCharArray().Length/letterPerSeconds+tiempo, dialogo));
    }


    IEnumerator StopDialogo(float tiempo, string dialogo)
    {
        yield return new WaitForSeconds(tiempo);
        textoDialogo.text = SubtractStrings(textoDialogo.text, "\n\n" + dialogo);

        if (textoDialogo.text.Trim() == "")
        {
            PanelDialogo.gameObject.SetActive(false);
            CambiarComportamiento(Comportamiento.Merodear);
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
    public IEnumerator TypeDialog(string dialog)
    {
        foreach (var letter in dialog.ToCharArray())
        {
            textoDialogo.text += letter;
            yield return new WaitForSeconds(1f/letterPerSeconds);
        }
       
    }


   

    #endregion
    #region Sistema de coportamiento
    [Header("Sistema de comportamiento")]
    [SerializeField] private Comportamiento comportamiento;
    [HideInInspector] private bool moverse = true;
    [HideInInspector] private bool EsperandoParaHablar = false;
    //[HideInInspector] private bool YaDijeLoQueQueriaDecir = true;
    [HideInInspector] private Vector2 Destino = new Vector2(0, 0);
    [HideInInspector] private Vector2 Position2D;
    private void Comportarse(Comportamiento comportamiento)
    {
        switch (comportamiento)
        {
            case Comportamiento.Merodear:
                if(EsperandoParaHablar == true)
                {
                    CambiarComportamiento(Comportamiento.Hablar);    
                }

                MovimientoMerodear();
                break;
            case Comportamiento.Hablar:
                DetenerseInstantaneamente();
                MirarAlJugador();
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
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float rangoDetectarHablando;
    private bool OtroEstaHablando()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangoDetectarHablando, detectionLayer);

        foreach(Collider2D collider in hitColliders)
        {
            if(collider.GetComponent<Dinosaurio>().comportamiento == Comportamiento.Hablar)
            {
                //Debug.Log("Están hablando:" + collider.GetComponent<Dinosaurio>().name);
                return true;
                
            }
        }

        return false;
    }

    private void MirarAlJugador()
    {
        
        if (Camara.transform.position.x>Position2D.x)
        {
            //está hacia la izquierda de la camara
            FondoDeDialogo.localScale = new Vector3(-1, FondoDeDialogo.localScale.y, FondoDeDialogo.localScale.z);
            spriteRenderer.flipX = true;
            SetRectTransformValues(PanelDialogo,-PosicionOriginalPanelDeDialogo.x, PanelDialogo.anchoredPosition.y);

        } else
        {
            FondoDeDialogo.localScale = new Vector3(1, FondoDeDialogo.localScale.y, FondoDeDialogo.localScale.z);
            spriteRenderer.flipX = false;
            SetRectTransformValues(PanelDialogo, PosicionOriginalPanelDeDialogo.x, PanelDialogo.anchoredPosition.y);
            //está hacia la derecha de la camara
        }

        if(Camara.transform.position.y > Position2D.y)
        {
            //está hacia arriba la camara
            FondoDeDialogo.localScale = new Vector3(FondoDeDialogo.localScale.x, 1, FondoDeDialogo.localScale.z);
            SetRectTransformValues(PanelDialogo, PanelDialogo.anchoredPosition.x, PosicionOriginalPanelDeDialogo.y);
        }
        else
        {
            FondoDeDialogo.localScale = new Vector3(FondoDeDialogo.localScale.x, -1, FondoDeDialogo.localScale.z);
            SetRectTransformValues(PanelDialogo, PanelDialogo.anchoredPosition.x, -PosicionOriginalPanelDeDialogo.y);
            //está hacia abajo la camara
        }

    }


    public void CambiarComportamiento(Comportamiento comportamiento)
    {
        
        if (comportamiento == Comportamiento.Hablar)
        {
            if(OtroEstaHablando() == true)
            {
                EsperandoParaHablar = true;
                return;
            }else
            {
                MostrarDialogo(TiempoExtraMostrarDialogo, dialogoPorDecir.mensaje);
                EsperandoParaHablar = false;
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
    [SerializeField] private RectTransform PanelDialogo;
    [HideInInspector] private RectTransform FondoDeDialogo;
    [HideInInspector] private GameObject Camara;
    #endregion
}




public class Dialogo
{
    public string mensaje;

    public Dialogo(string mensaje)
    {
        this.mensaje = mensaje;
    }


}