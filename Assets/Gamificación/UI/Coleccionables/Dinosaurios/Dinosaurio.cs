using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;
[System.Serializable]
public class Dinosaurio : MonoBehaviour
{
    #region Caracteristicas generales
    [Header("Caracteristicas generales")]
    [SerializeField] private string _nombre;
    [SerializeField] private float _velocidad;
    [SerializeField] public string Nombre { get { return _nombre; } set { _nombre = value; interactuador.ActualizarNombre(); } }
    [HideInInspector] public Especie _Especie { get => especie; set => especie = value; }


    [SerializeField] private Especie especie;
    [SerializeField] private Rareza rareza;

    [HideInInspector] private Rigidbody2D rb;
    [HideInInspector] private Animator AnimadorCaracter;

    [HideInInspector] private bool LookAtRight;
    [HideInInspector] private SpriteRenderer spriteRenderer;

    #endregion

    #region Sistema de emociones

    [Header("Sistema de Emociones")]
    [Range(-0.5f, 1.5f)][SerializeField] private float emocionalidad;
    [HideInInspector] private EstadoAnimo estadoAnimo;
    /// <summary>
    /// 1 - 3 euforico
    /// 0.5 - 1 feliz
    /// 0 - 0.5 triste
    /// -3 - 0 deprimido
    /// </summary>
    public float Emocionalidad
    {
        get
        {
            return emocionalidad;

        }
        set
        {
            emocionalidad = value;
            ActualizarEstadoDeAnimo();
            if (interactuador != null)
            {
                interactuador.ActualizarMostrarEmocionalidad();
                interactuador.ActualizarMostrarDescripcionEmocionalidad();
            }

        }
    }

    public Rareza Rareza { get => rareza; set => rareza = value; }

    [SerializeField] private float sensibilidad = 1f;
    [SerializeField] private float consumoBaseDiarioEmocionalidad = 25f;
    public void AplicarDepresionPorTiempo()
    {
        // se ejecuta cada 5 minutos

        // 24 horas = 25% tristeza
        // 1 horas = 1.04 % tristeza
        // 5 minuto = 0.086% tristeza 

        AlterarEmocionalidadEnPorcentaje(-consumoBaseDiarioEmocionalidad * (1f / 24f) * (1f / 60f) * (5f) * sensibilidad*100);
        Debug.Log("porcentaje de depresión generadoa por el tiempo transcurrido durante la aplicación: " + -consumoBaseDiarioEmocionalidad * (1f / 24f) * (1f / 60f) * (5f) * sensibilidad*100 + "%");
    }


    public void AplicarDepresionPorTiempo(float TiempoTranscurrido)
    {
        switch (rareza)
        {
            case Rareza.Comun:
                consumoBaseDiarioEmocionalidad = 0.15f;
                break;
            case Rareza.Rara:
                consumoBaseDiarioEmocionalidad = 0.25f;
                break;
            case Rareza.SuperRara:
                consumoBaseDiarioEmocionalidad = 0.50f;
                break;
            case Rareza.Legendaria:
                consumoBaseDiarioEmocionalidad = 0.75f;
                break;
        }


        // Cálculo exacto de cuánto debería perder según el tiempo transcurrido en segundos
        float porcentajePorSegundo = (consumoBaseDiarioEmocionalidad / (24f * 60f * 60f)) * sensibilidad;
        float variacion = -TiempoTranscurrido * porcentajePorSegundo;

        variacion = Mathf.Max(variacion, -2f); // límite de -200% para evitar locuras

        AlterarEmocionalidadEnPorcentaje(variacion*100);


        //float variacion = -TiempoTranscurrido * sensibilidad * (1f / 60f) * (1f / 60f) * (25f / 24f);

        //AlterarEmocionalidadEnPorcentaje(Mathf.Max(variacion, -200f));

        //float porcentajePorSegundo = (consumoBaseDiarioEmocionalidad / (24f * 60f * 60f)) * sensibilidad;
        Debug.Log(
     "🧠 DEPRESIÓN FUERA DE LA APP\n" +
     "- Tiempo transcurrido (s): " + TiempoTranscurrido + "\n" +
     "- Consumo base diario: " + consumoBaseDiarioEmocionalidad + "\n" +
     "- Sensibilidad: " + sensibilidad + "\n" +
     "- Porcentaje por segundo: " + (porcentajePorSegundo * 100f) + "%\n" +
     "- Porcentaje total restado: " + (variacion * 100f) + "%\n" +
     "- Nombre: " + Nombre
 );



    }


    public float ObtenerTiempoTranscurrido()
    {
        // Recupera la última vez que se guardó el tiempo, si no existe, devuelve 0 segundos
        string ultimaActualizacionStr = PlayerPrefs.GetString("UltimaActualizacion", DateTime.Now.ToString());
        DateTime ultimaActualizacion = DateTime.Parse(ultimaActualizacionStr);

        // Calcula el tiempo transcurrido
        TimeSpan tiempoTranscurrido = DateTime.Now - ultimaActualizacion;

        return (float)tiempoTranscurrido.TotalSeconds;
    }



    public float GetMinimoSostenible()
    {
        //proporción de 1% => 1 minuto 
        return consumoBaseDiarioEmocionalidad * sensibilidad * (1 / 1);
    }

    public void ActualizarEstadoDeAnimo()
    {
        if (Emocionalidad >= 1) //100% hasta 200%
        {
            CambiarEstadoAnimo(EstadoAnimo.Euforia);
        }
        else if (Emocionalidad >= 0.5) // 50% - 100% 
        {
            CambiarEstadoAnimo(EstadoAnimo.Feliz);
        }
        else if (emocionalidad > 0) // 0% - 50%
        {
            CambiarEstadoAnimo(EstadoAnimo.Triste);
        }
        else if (Emocionalidad <= 0) // -100% a 0% 
        {
            CambiarEstadoAnimo(EstadoAnimo.Deprimido);
        }
    }

    private void CambiarEstadoAnimo(EstadoAnimo estado)
    {
        if (this.estadoAnimo == EstadoAnimo.Deprimido && estado != EstadoAnimo.Deprimido)
        {
            CambiarComportamiento(Comportamiento.Merodear);
        }
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

    public void AlterarEmocionalidadEnPorcentaje(float variacionEnPorcentaje)
    {

        ////50% = 0.5f
        //Emocionalidad += (variacionEnPorcentaje / 100f) * sensibilidad;
        Debug.Log("Nueva emocionalidad antes del cambio: " + emocionalidad + "para: " + Nombre);

        emocionalidad += (variacionEnPorcentaje / 100f) * sensibilidad;
        if (interactuador != null)
        {
            interactuador.ActualizarMostrarEmocionalidad();
            interactuador.ActualizarMostrarDescripcionEmocionalidad();
        }

        ActualizarEstadoDeAnimo();

        Debug.Log("Nueva emocionalidad luego del cambio: " + emocionalidad + "para: " + Nombre);
    }

    #endregion
    #region Sistema de exigencias

    [SerializeField] public List<Exigencia> exigencias;
    public GameObject vfxBoletoPrefab;
    public GameObject vfxAdopcionPrefab;
    public float GetTiempoTotalExigido()
    {
        float tiempoTotal = 0;

        foreach (Exigencia exigencia in exigencias)
        {
            tiempoTotal += exigencia.GetMostrarTiempoAproximadoExigido();

        }
        return tiempoTotal;
    }

    public void MostrarVFXBoleto()
    {
        MostrarVFX(vfxBoletoPrefab);
        //SONIDO XD
    }
    public void MostrarVFXAdopcion()
    {
        MostrarVFX(vfxAdopcionPrefab);
        //SONIDO XD
    }

    private void MostrarVFX(GameObject prefab)
    {
        // Instanciamos como hijo del motivador (este script debe estar en el motivador)
        GameObject efecto = Instantiate(prefab, transform);

        // Posicionamos localmente 1.5 unidades sobre su cabeza
        efecto.transform.localPosition = Vector3.up * 1.5f;

        // Limpiamos el efecto después de 2 segundos
        Destroy(efecto, 2f);
    }

    //public float TiempoMinimoExigido()
    //{

    //}


    public Rareza GetRereza()
    {
        return rareza;
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
        interactuador = GetComponent<InteraccionUsuario>();
        rb = GetComponent<Rigidbody2D>();
        AnimadorCaracter = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // experimentación





        IniciarExigencias();

    }

    private void Start()
    {
        moverse = true;
        EstadisticasManager.Instance.AñadirCaracterMotivadorAlSistema(this);
        interactuador.ActualizarMostrarIndicadorCompletadoExigencias();
        interactuador.ActualizarMostrarEmocionalidad();

        rb = GetComponent<Rigidbody2D>();
        Camara = Camera.main.gameObject;
        FondoDeDialogo = PanelDialogo.transform.Find("Fondo dialogo").GetComponent<RectTransform>();
        PosicionOriginalPanelDeDialogo = PanelDialogo.anchoredPosition;
        RectTransformTextoDialogo = textoDialogo.GetComponent<RectTransform>();
        PosicionOriginalTextoDeDialogo = RectTransformTextoDialogo.anchoredPosition;
        PanelDialogo.gameObject.SetActive(false);

        //ComprobarEliminarPorDepresion();
        switch (rareza)
        {
            case Rareza.Comun:
                consumoBaseDiarioEmocionalidad = 0.15f;
                break;
            case Rareza.Rara:
                consumoBaseDiarioEmocionalidad = 0.25f;
                break;
            case Rareza.SuperRara:
                consumoBaseDiarioEmocionalidad = 0.50f;
                break;
            case Rareza.Legendaria:
                consumoBaseDiarioEmocionalidad = 0.75f;
                break;
        }
        InvokeRepeating("AplicarDepresionPorTiempo", 0f, 300f);

        //AplicarDepresionPorTiempo(ObtenerTiempoTranscurrido());


        SetEspecieYRareza();

        EstadisticasManager.Instance.GuardarInformarciónMotivadores();
    }

    public void SetEspecieYRareza()
    {
        AnimadorCaracter.SetInteger("Especie", (int)especie);
        AnimadorCaracter.SetInteger("Rareza", (int)rareza);
    }

    void Update()
    {
        Position2D = gameObject.transform.position;
        Comportarse(comportamiento);
        setEstadoAnimo();


        if (IsMoving())
        {
            AnimadorCaracter.SetFloat("Moviendose", 1);
        }
        else
        {
            AnimadorCaracter.SetFloat("Moviendose", 0);
        }


        if (AnimadorGloboDeTexto.gameObject.activeSelf)
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



        AnimadorCaracter.SetFloat("Comportamiento", (float)comportamiento);

        SetEspecieYRareza(); //Debes quitarlo


        //if(dialogoActualPorDecir != null)
        //{
        if (dialogosPorDecir.Count > 0) //¿tengo algoq ue decir?
        {
            Debug.Log("cantidad de dialogos " + dialogosPorDecir.Count);
            CambiarComportamiento(Comportamiento.Hablar);
        }
        //}
        //else
        //{
        //    Debug.Log("el dialogo actual no existe");
        //}

        UpdateSortingOrder();

    }

    void UpdateSortingOrder()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
    private void OnValidate()
    {
        // Aquí forzamos la llamada a la propiedad cada vez que cambie desde el Inspector.
        Emocionalidad = emocionalidad;
    }

    //private void OnDestroy()
    //{
    //    EstadisticasManager.Instance.EliminarCaracterMotivadorDelSistema(this);
    //}

    public void SerAdoptado(Especie especie, Rareza rareza, string nombre)
    {
        this.especie = especie;
        this.rareza = rareza;
        Nombre = nombre;
        ActualizarAspectoMotivador();
        emocionalidad = UnityEngine.Random.Range(0.1f, 0.4f);
    }

    public void ActualizarAspectoMotivador()
    {
        //// NECESITAS AÑADIR DESPUES
        ///
    }

    public void IniciarExigencias()
    {
        if (exigencias == null)
        {
            exigencias = new List<Exigencia> { new ExigenciaTiempoProductivo(this, Exigencia.Dificultad.facil), new ExigenciaTiempoProductivo(this, ExigenciaTiempoProductivo.Dificultad.moderado), new ExigenciaTiempoProductivo(this, ExigenciaTiempoProductivo.Dificultad.dificil) };
        }


    }

    public void ReiniciarExigencias()
    {
        //las exigencias se han reiniciado
        exigencias.Clear();
        exigencias = new List<Exigencia> { new ExigenciaTiempoProductivo(this, Exigencia.Dificultad.facil), new ExigenciaTiempoProductivo(this, ExigenciaTiempoProductivo.Dificultad.moderado), new ExigenciaTiempoProductivo(this, ExigenciaTiempoProductivo.Dificultad.dificil) };

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

    #endregion
    #region Sistema de dialogo 
    [Header("Sistema de dialogos")]
    [SerializeField] private float esperarParaMostrar = 0.01f;
    [SerializeField] private float letterPerSeconds;
    [SerializeField] private float TiempoExtraMostrarDialogo = 5f;
    public List<Dialogo> dialogosPorDecir = new List<Dialogo>();
    private Dialogo dialogoActualPorDecir;
    [HideInInspector] private bool PensandoDialogo = false;
    private bool dialogoActivo = false;
    private Dialogo dialogoPendienteAUsuario = null;



    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && dialogoPendienteAUsuario != null)
        {
            StartCoroutine(ProcesoDeFecilitación(dialogoPendienteAUsuario));
            dialogoPendienteAUsuario = null; // Limpiamos para que no se repita
        }
    }

    private void Felicitar(Tempos tempos)
    {
        if (tempos.tiposTempos == TiposTempos.productivo)
        {
            if (estadoAnimo != EstadoAnimo.Triste || estadoAnimo != EstadoAnimo.Deprimido)
            {
                dialogoPendienteAUsuario = new Felicitacion(this, tempos); // Guardamos pero NO ejecutamos aún


            }

        }
    }

    public void Celebrar()
    {
        StartCoroutine(ProcesoDeFecilitación(null));
    }

    private IEnumerator ProcesoDeFecilitación(Dialogo dialogoNuevo)
    {
        CambiarComportamiento(Comportamiento.Celebrar); // Iniciar celebración

        float TiempoDeCelebracion = 0f;
        float ProbabilidadExtraDeHablar = 0f;
        switch (estadoAnimo)
        {
            case EstadoAnimo.Triste:
                TiempoDeCelebracion = 0f;
                break;
            case EstadoAnimo.Feliz:
                TiempoDeCelebracion = UnityEngine.Random.Range(1f, 6f);
                break;
            case EstadoAnimo.Euforia:
                TiempoDeCelebracion = UnityEngine.Random.Range(3f, 9f);
                ProbabilidadExtraDeHablar = 0.01f;
                break;
            case EstadoAnimo.Deprimido:
                TiempoDeCelebracion = 0f;
                break;
        }

        yield return new WaitForSeconds(TiempoDeCelebracion);
        CambiarComportamiento(Comportamiento.Merodear);
        //RECORDATORIO MENTAL QUE ESTO DE AQUI ELIMINA EL COMPORTAMIENTO DE CELEBRAR Y SOLO POR AHORA LO DEJARË COMO UN COMENTARI
        float probabilidadHablar = 0f;
        if (ConexionInternetManager.HayInternet() == true && dialogoNuevo != null)
        {
            //probabilidadHablar = Mathf.Max(0.05f, 0.5f - 0.1f * EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count + ProbabilidadExtraDeHablar);
            probabilidadHablar = 0.01f + ProbabilidadExtraDeHablar;
        }
        else
        {
            probabilidadHablar = 0;
        }

        //float probabilidadHablar = 0f; //ELIMINAR ESTO Y REEMPALZA CON LO DE ARRIBA
        if (UnityEngine.Random.value <= probabilidadHablar)
        {
            IniciarComportamientoHablar(dialogoNuevo); // Iniciar el diálogo después de celebrar
        }


    }


    private void IniciarComportamientoHablar(Dialogo dialogoNuevo)
    {
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
        yield return StartCoroutine(TestAI.Instance.UseGeminiAI(dialogo.Prompt));


        dialogo.mensajeFinal = TestAI.Instance.response;


        //dialogo.mensajeFinal = TestAI.Instance.response.candidates[0].content.parts[0].text;




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

        if (dialogoActivo == false)
        {
            dialogoActivo = true;
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

    }


    IEnumerator StopDialogo(float tiempo)
    {
        yield return new WaitForSecondsRealtime(tiempo);
        textoDialogo.text = SubtractStrings(textoDialogo.text, dialogoActualPorDecir.mensajeFinal);

        dialogosPorDecir.Remove(dialogoActualPorDecir);
        dialogoActualPorDecir = null;

        if (textoDialogo.text.Trim() == "")
        {
            PanelDialogo.gameObject.SetActive(false);
            CambiarComportamiento(Comportamiento.Merodear);
        }

        dialogoActivo = false;
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
        yield return new WaitForSecondsRealtime(firstWait);

        textoDialogo.text = string.Empty;


        foreach (var letter in dialog.ToCharArray())
        {
            textoDialogo.text += letter;
            yield return new WaitForSecondsRealtime(1f / letterPerSeconds);
        }

    }

    #endregion
    #endregion
    #region Sistema de comportamiento
    [Header("Sistema de comportamiento")]
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float rangoDetectarHablando;
    [HideInInspector] private Comportamiento comportamiento;
    [HideInInspector] private bool moverse = true;
    //[HideInInspector] private bool EsperandoParaHablar = false;
    //[HideInInspector] private bool YaDijeLoQueQueriaDecir = true;
    [HideInInspector] private Vector2 Destino = new Vector2(0, 0);
    [HideInInspector] private Vector2 Position2D;
    private void Comportarse(Comportamiento comportamiento)
    {
        if(emocionalidad >= 0f) //O SEA ESTA DEPRIMIDO Y POR ENDE NO DEBERIA AHCER NINGUNA DE LAS ANIMACIONES
        {
            switch (comportamiento)
            {
                case Comportamiento.Merodear:
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
                case Comportamiento.Comer:
                    DetenerseInstantaneamente();
                    break;
            }
        }
        else
        {
            CambiarComportamiento(Comportamiento.Deprimirse);
            DetenerseInstantaneamente();
        }

      

    }

    public bool PuedoComerlo()
    {
        if (comportamiento == Comportamiento.Comer && estadoAnimo != EstadoAnimo.Deprimido)
        {
            return false;
        }
        else return true;
    }

    private IEnumerator ProcesoDeComer()
    {
        float TiempoDeCelebracionPorComer = 0f;

        string NombreAnimacion = "Comer";

        switch (rareza)
        {
            case Rareza.Comun:
                NombreAnimacion += "C";
                break;
            case Rareza.Rara:
                NombreAnimacion += "R";
                break;
            case Rareza.SuperRara:
                NombreAnimacion += "S";
                break;
            case Rareza.Legendaria:
                NombreAnimacion += "L";
                break;
        }

        switch (especie)
        {
            case Especie.TiranosaurioRex:
                NombreAnimacion += "4";
                break;
            case Especie.Triceratops:
                NombreAnimacion += "1";
                break;
            case Especie.Espinosaurio:
                NombreAnimacion += "3";
                break;
            case Especie.Stegosaurus:
                NombreAnimacion += "2";
                break;
        }

        float TiempoDeDuraciónDeComer = ObtenerDuracionAnimacion(NombreAnimacion);

        Debug.Log("Tiempo de duración de comer: " + TiempoDeDuraciónDeComer);
        switch (estadoAnimo)
        {
            case EstadoAnimo.Triste:
                TiempoDeCelebracionPorComer = 0f;
                break;
            case EstadoAnimo.Feliz:
                TiempoDeCelebracionPorComer = UnityEngine.Random.Range(3f, 6f);
                break;
            case EstadoAnimo.Euforia:
                TiempoDeCelebracionPorComer = UnityEngine.Random.Range(3f, 8f);

                break;
            case EstadoAnimo.Deprimido:
                TiempoDeCelebracionPorComer = 0f;
                break;
        }
        Debug.Log("Tiempo total de espera: " + (TiempoDeDuraciónDeComer + TiempoDeCelebracionPorComer));
        yield return new WaitForSeconds(TiempoDeDuraciónDeComer + TiempoDeCelebracionPorComer);
        CambiarComportamiento(Comportamiento.Merodear);
    }

    private float ObtenerDuracionAnimacion(string nombreAnimacion)
    {
        // Buscar el clip correcto en el AnimatorController
        AnimationClip clipEncontrado = AnimadorCaracter.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == nombreAnimacion);

        if (clipEncontrado == null)
        {
            Debug.LogWarning($"⚠️ No se encontró la animación '{nombreAnimacion}'.");
            return 0f;
        }

        Debug.Log($"🎥 Animación encontrada: {clipEncontrado.name}, Duración base: {clipEncontrado.length}s");

        // Obtener el estado actual o el próximo en reproducción
        AnimatorStateInfo estado = AnimadorCaracter.GetCurrentAnimatorStateInfo(0);
        if (!estado.IsName(nombreAnimacion))
        {
            estado = AnimadorCaracter.GetNextAnimatorStateInfo(0);
        }

        // Obtener velocidad de la animación desde el Animator
        //float speed = estado.speed;

        // Si la velocidad es 0 (posiblemente aún no se ha actualizado), devolvemos la duración normal
        //if (speed == 0)
        //{
        //    Debug.LogWarning("⚠️ La velocidad de la animación es 0. Devolviendo duración base.");
        //    return clipEncontrado.length;
        //}

        float duracionFinal = clipEncontrado.length / 0.10f;
        Debug.Log($"✅ Duración ajustada: {duracionFinal}s (Speed: {0.10f})");

        return duracionFinal;
    }


    [HideInInspector] private Vector2 PosicionOriginalPanelDeDialogo;
    [HideInInspector] private Vector2 PosicionOriginalTextoDeDialogo;

    private bool OtroEstaHablando()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangoDetectarHablando, detectionLayer);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.GetComponent<Dinosaurio>().comportamiento == Comportamiento.Hablar)
            {
                if (collider.GetComponent<Dinosaurio>().gameObject != gameObject)
                {
                    return true;
                }

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
        //if (emocionalidad >= 0f)
        //{


        //}
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
        else if (comportamiento == Comportamiento.Deprimirse)
        {
            if (AnimadorCaracter != null)
            {
                AnimadorCaracter.SetTrigger("DeprimirseTrigger");
            }

            if(this.comportamiento != Comportamiento.Deprimirse)
            {
                LogicaVentanaConfirmacion.Instance.ShowPopup("Modo depresivo", "Alguna de tus mascotas ha entrado en modo depresivo. Hoy es tu oportunidad final para salvarlo subiendo su vida. Si abandonas la aplicación en este modo, lo puedes perder", () => { Debug.Log("Cancelado"); }, () => { Debug.Log("Cancelado"); });
            }

        }
        else
        if (comportamiento == Comportamiento.Comer && estadoAnimo != EstadoAnimo.Deprimido)
        {
            StartCoroutine(ReproducirSonidoComerConRetraso());
            StartCoroutine(ProcesoDeComer());
        }


        DetenerseInstantaneamente();
        this.comportamiento = comportamiento;

    }
    private IEnumerator ReproducirSonidoComerConRetraso()
    {
        yield return new WaitForSeconds(0.1f);
        ControladorSonidos.Instancia.ReproducirSonidoComer();
    }
    public void MovimientoMerodear()
    {
        if (Destino == Vector2.zero)
        {
            Vector2 PosiciónAleatoria = new(UnityEngine.Random.Range(Gamificacion.MinX, Gamificacion.MaxX), UnityEngine.Random.Range(Gamificacion.MinY, Gamificacion.MaxY));
            Destino = PosiciónAleatoria; //necesario para empezar a caminar
        }

        if (Vector2.Distance(Destino, Position2D) < 2)
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
        if (rb != null)
        {
            rb.velocity = Vector2.zero; rb.angularVelocity = 0;
        }
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

    #region Sistema de ganar o perder
    public void EliminarMotivador()
    {
        EstadisticasManager.Instance.EliminarCaracterMotivadorDelSistema(this);
        Destroy(gameObject);
        EstadisticasManager.Instance.GuardarInformarciónMotivadores();
    }

    public void ComprobarEliminarPorDepresion()
    {
        if (estadoAnimo == EstadoAnimo.Deprimido)
        {
            LogicaVentanaConfirmacion.Instance.ShowPopup("¡Escapó!", $"Alguno de tus fieles compañeros acaba de escapar debido al hambre. Recuerda siempre alimentarlos", () => Debug.Log("Cancelado"), () => Debug.Log("Cancelado"));
            EliminarMotivador();
        }


    }

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
    [HideInInspector] public InteraccionUsuario interactuador;
    #endregion
}
#region variables enum
public enum Rareza { Comun, Rara, SuperRara, Legendaria }
public enum Especie { TiranosaurioRex, Triceratops, Espinosaurio, Stegosaurus }


public enum Comportamiento { Merodear, Hablar, Celebrar, Deprimirse, Comer }

public enum EstadoAnimo { Triste, Feliz, Euforia, Deprimido }
#endregion


#region Clases de Dialogos

public class Dialogo
{
    public string mensajeFinal;
    protected EstadoAnimo animoDelDialogo;
    //Sistema de uso de AI Gemini
    protected string contextoGeneral = "";
    protected string contextoEspecifico = "";
    protected string tarea = "";
    public string Prompt { get => contextoGeneral + contextoEspecifico + tarea; }
    public Dialogo(Dinosaurio emisor)
    {
        contextoGeneral = "Interpretas a una mascota que acompaña al usario en su trabajo. Te comportas feliz o triste según la productividad del usuario. Tú output no debe superar los 150 caracteres";
        contextoGeneral += ".Eres un pequeño " + emisor._Especie.ToString();
        contextoGeneral += ".Estás " + emisor.getEstadoDeAnimo().ToString();
        this.animoDelDialogo = emisor.getEstadoDeAnimo();

    }
}

public class Felicitacion : Dialogo
{
    public enum Rareza { nula, sencillo, desafiante, superior, Top }
    public Rareza rareza;

    List<string> promptsMotivadores = new List<string>()
{
    "Imagina que eres un motivador digital muy leal y creativo. Tu usuario acaba de completar una sesión de trabajo productiva y tú, como su compañero de camino, quieres agradecerle profundamente. Exprésale tu orgullo como si fueras su mascota virtual favorita y comparte una reflexión motivadora sobre cómo las pequeñas acciones construyen grandes logros.",

    "Tu usuario acaba de avanzar en su jornada de productividad. Eres su guía emocional en este viaje. Inspíralo con un mensaje emocional, con un toque filosófico, que lo haga sentir que su esfuerzo es parte de un propósito mayor. Evita lo genérico y conéctalo con la idea de transformación personal.",

    "Escribe un mensaje motivacional como si fueras un mentor sabio y divertido, con un toque de humor inteligente. Tu misión es recordarle al usuario que incluso cuando no lo nota, está avanzando. Sé creativo, como un personaje de película animada que le habla directamente al corazón.",

    "Finge ser un motivador raro y legendario, y dile al usuario que ha sido digno de tu atención. Exprésale admiración por su constancia y dile una frase digna de recordar, como si le estuvieras entregando un artefacto mágico con poder emocional.",

    "Tu usuario ha vuelto a trabajar pese al cansancio. Exprésale tu gratitud como si fueras un compañero que ha visto todo su esfuerzo en silencio. Hazlo con palabras profundas y visuales, como si escribieras un poema breve que se convierte en mantra personal.",

    "Sé un motivador con alma de cuentacuentos. Dile al usuario que cada sesión productiva es como un capítulo de su historia, y que hoy ha escrito una página valiente. Invítalo a seguir escribiendo su libro personal de logros.",

    "Escribe un mensaje como si fueras el espíritu interior del propio usuario, que despierta solo cuando trabaja con propósito. Felicítalo y hazle ver lo lejos que ha llegado desde que empezó. Usa una metáfora poderosa (montaña, fuego, viaje, etc.).",

    "Tu usuario acaba de dar un paso más en su meta. Sé un motivador con voz de ciencia ficción: dile que en una línea temporal alternativa, él o ella es una leyenda por su disciplina. Hazle sentir que está hackeando su destino.",

    "Habla como un viejo sabio que ha acompañado a muchos grandes humanos. Felicita al usuario, pero hazlo con un mensaje que pueda quedarse en su mente todo el día. Evita clichés, busca profundidad y originalidad.",

    "Crea un mensaje corto pero poderoso que inicie con: 'Hoy demostraste que...' y termina con una frase que haga sentir al usuario que es parte de un cambio grande en su vida."
};
    public Felicitacion(Dinosaurio emisor, Tempos tempoTerminado) : base(emisor)
    {
        tarea = "Felicitalo por lo que logró el usuario, incluso si tu estadode ánimo es triste";

        if (tempoTerminado.tiposTempos == TiposTempos.productivo)
        {
            rareza = Rareza.sencillo;
            contextoEspecifico += $".El usuario logró superar estar concentrado un total de {tempoTerminado.TiempoTotal.ToString(@"h\:mm\:ss")}" + promptsMotivadores[UnityEngine.Random.Range(0, promptsMotivadores.Count)];
            //Felicitar por superar de ser más productivo de lo normal
            //if (EstadisticasManager.TiempoTempoProductivoPromedio < tempoTerminado.TiempoTotal)
            //{
            //    rareza = Rareza.desafiante;
            //    contextoEspecifico += $".El usuario logró superar su promedio productivo diario de {EstadisticasManager.TiempoTotalProductivo.ToString(@"h\:mm\:ss")}";
            //}


            //Felicitar por superar tu tiempo de productividad muy larga
            //////////if (tempoTerminado.TiempoTotal >= new TimeSpan(1, 30, 0))
            //////////{
            //////////    rareza = Rareza.superior;
            //////////    contextoEspecifico += $".El usuario logró estar concentrado durante el largo periodo consecutivo de más 1 hora y 30 minutos. El usuario estuvo {tempoTerminado.TiempoTotal.ToString(@"h\:mm\:ss")} en total";

            //////////}
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

#endregion
#region Clases de Exigencias

//
// retos generales
// lleva cierta cantidad de pomorodos
// establece la tarea más importante
// planifica las tareas de tu siguiente pomodoro

public class Exigencia : IMostrarIndicadorCompletado
{
    protected string descripcion = "";
    protected float recompensaEmocional;

    public Dificultad dificultad;
    public readonly Dinosaurio exigidor;
    public bool Completado;
    protected float tiempoAproximadoExigido;

    public virtual float GetMostrarTiempoAproximadoExigido()
    {
        Debug.LogError("estás mostrando el tiempo exigisdo de una clase abstracta");
        return 0;
    }



    public Exigencia(Dinosaurio exigidor, Dificultad dificultad, string descripcion = "-", float recompensaEmocional = 0) //se deberá quitar estos parametros opcionales
    {
        Completado = false;

        this.exigidor = exigidor;

        if (descripcion != "-")
        {
            this.descripcion = descripcion;
        }
        if (recompensaEmocional > 0)
        {
            this.recompensaEmocional = recompensaEmocional;
        }

        this.dificultad = dificultad;
    }

    public enum Dificultad { facil, moderado, dificil } // yambien hace refrencia a la posición


    public virtual string GetMostrarIndicadorCompletado()
    {
        return "estás mostrando el progreso de una clase abstracta";
    }

    public virtual string GetMostrarDescripcionExigencia()
    {
        return "estás mostrando la descripción de una clase abstracta";
    }

    public virtual float GetValueIndicadorCompletado()
    {
        Debug.Log("Estás mostrando el valor de progreso de una clase abstracta");
        return Completado ? 1f : 0f;
    }
}
public enum TiposComidas
{
    prueba1, cualquierTipo
}

public class ExigenciaTiempoProductivo : Exigencia//Exigencia por comida / exigencia básica
{
    public float metaTiempoProductivo;
    private static Dictionary<Rareza, double> valoresRareza = new Dictionary<Rareza, double>()
    {
        { Rareza.Comun, 0.1 },
        { Rareza.Rara, 0.5 },
        { Rareza.SuperRara, 1.0 },
        { Rareza.Legendaria, 2.0 }
    };
    //creador de exigencias diarias
    public float progresoMeta = 0;
    public readonly TiposComidas tiposComidaRequerida;
    public float ProgresoMeta
    {
        get
        {
            exigidor.interactuador.ActualizarMostrarIndicadorCompletadoExigencias();
            return progresoMeta;
        }

        set
        {

            if (value >= metaTiempoProductivo)
            {
                progresoMeta = metaTiempoProductivo;
                if (Completado == false)
                {
                    ExigenciaCompletadaEfectos();
                    Completado = true;
                }

            }
            else
            {
                progresoMeta = value;

            }

            exigidor.interactuador.ActualizarMostrarIndicadorCompletadoExigencias();

        }



    }
    // debo crear un randomizadorde tipos de comida
    public ExigenciaTiempoProductivo(Dinosaurio exigidor, Dificultad dificultad, TiposComidas tiposComidaRequerida = TiposComidas.cualquierTipo, float metaProductiva = 0) : base(exigidor, dificultad)
    {

        // max 5 CM -  5/10 horas
        // promedio 3 CM - 3/6 horas
        // min 1 CM -  1/2 horas

        // facil ==> 1 - 30  minutos promedio (para mantener estado de ánimo) 
        // mediano ==> 30 - 60 (para aumentar un poco el estado de ánimo)
        // dificil ==> 60 - 120 (para aumentar mucho el estado de ánimo) 

        this.tiposComidaRequerida = tiposComidaRequerida;

        progresoMeta = 0;

        if (metaProductiva <= 0)
        {
            switch (exigidor.GetRereza())
            {
                case Rareza.Comun:
                    switch (dificultad)
                    {
                        case Dificultad.facil:
                            metaTiempoProductivo = SelecionarMeta(1, 5);
                            break;
                        case Dificultad.moderado:
                            metaTiempoProductivo = SelecionarMeta(5, 10);
                            break;
                        case Dificultad.dificil:
                            metaTiempoProductivo = SelecionarMeta(10, 15);
                            break;
                    }
                    break;

                case Rareza.Rara:
                    switch (dificultad)
                    {
                        case Dificultad.facil:
                            metaTiempoProductivo = SelecionarMeta(5, 10);
                            break;
                        case Dificultad.moderado:
                            metaTiempoProductivo = SelecionarMeta(10, 20);
                            break;
                        case Dificultad.dificil:
                            metaTiempoProductivo = SelecionarMeta(20, 35);
                            break;
                    }
                    break;

                case Rareza.SuperRara:
                    switch (dificultad)
                    {
                        case Dificultad.facil:
                            metaTiempoProductivo = SelecionarMeta(10, 15);
                            break;
                        case Dificultad.moderado:
                            metaTiempoProductivo = SelecionarMeta(15, 30);
                            break;
                        case Dificultad.dificil:
                            metaTiempoProductivo = SelecionarMeta(30, 90);
                            break;
                    }
                    break;

                case Rareza.Legendaria:
                    switch (dificultad)
                    {
                        case Dificultad.facil:
                            metaTiempoProductivo = SelecionarMeta(15, 20);
                            break;
                        case Dificultad.moderado:
                            metaTiempoProductivo = SelecionarMeta(20, 60);
                            break;
                        case Dificultad.dificil:
                            metaTiempoProductivo = SelecionarMeta(60, 240);
                            break;
                    }
                    break;
            }

        }
        else
        {
            this.metaTiempoProductivo = metaProductiva;
        }

        this.descripcion = $"Los caracteres motivadores necesitan alimentarse. Alimentalo de el tipo de comida que exiga para completar su exigencia.";

    }



    private float SelecionarMeta(float min, float max)
    {
        float rareza = (float)valoresRareza[exigidor.GetRereza()];
        float aleatorioNormalizado = UnityEngine.Random.Range(0f, 1f);
        float numeroSesgado = Mathf.Pow(aleatorioNormalizado, 1 / rareza);
        return min + (numeroSesgado * (max - min));
    }
    public void ExigenciaCompletadaEfectos() // Se invoca cada vez que se logra una exigencia
    {
        float proporciónDificultad = 0f;
        switch (dificultad)
        {
            case Dificultad.facil:
                proporciónDificultad = 1f;
                break;
            case Dificultad.moderado:
                proporciónDificultad = 1.5f;
                break;
            case Dificultad.dificil:
                proporciónDificultad = 2f;
                break;
        }

        //consumo de emocionalidad diario 0.3 - 241.5 pp lo calculé viendo el promedio entre los pp de una exigencia facil, que se supone que es quien regula
        //exigidor.AlterarEmocionalidad(10f * proporciónDificultad);
        exigidor.Celebrar();
        Debug.Log("la exigencia a sido completada");










        //SistemaRecompensa.Instancia.IntentoRuletaGanado();
        ////Crear una animación en su cabeza indicando que sa adquirido un boleto extra

        //VentanaAdopcion.Instance.GanarUnaAdopción();
        ////Crear animación de adopción adquirida






        // 🎲 Recompensa opcional: adopción gratuita
        float rareza = (float)valoresRareza[exigidor.GetRereza()]; // Asegúrate que va de 0.5 a 2.0, por ejemplo


        // Fórmula con mezcla de rareza y dificultad
        float probabilidad = ObtenerProbabilidadAdopcionGratuita(exigidor.Rareza, dificultad);

        float dado = UnityEngine.Random.Range(0f, 1f);

        if (dado <= probabilidad)
        {
            VentanaAdopcion.Instance.GanarUnaAdopción();
            exigidor.MostrarVFXAdopcion();
            ControladorSonidos.Instancia.ReproducirSonidoAdopcion();
            //TODO: Animación de adopción brillante y mística
            Debug.Log("¡Has ganado una adopción gratuita!");
        }
        else
        {

            // 🎯 Recompensa fija: intento de ruleta
            SistemaRecompensa.Instancia.IntentoRuletaGanado();
            exigidor.MostrarVFXBoleto();
            ControladorSonidos.Instancia.ReproducirSonidoBoleto();
            Debug.Log("¡Has ganado una ruleta extra!");
            //TODO: Animación de ruleta sobre la cabeza

        }



    }

    public float ObtenerProbabilidadAdopcionGratuita(Rareza rareza, Dificultad dificultad)
    {
        switch (rareza)
        {
            case Rareza.Comun:
                return dificultad == Dificultad.dificil ? 0.05f : 0f;

            case Rareza.Rara:
                switch (dificultad)
                {
                    case Dificultad.facil: return 0.01f;
                    case Dificultad.moderado: return 0.05f;
                    case Dificultad.dificil: return 0.10f;
                }
                break;

            case Rareza.SuperRara:
                switch (dificultad)
                {
                    case Dificultad.facil: return 0.05f;
                    case Dificultad.moderado: return 0.15f;
                    case Dificultad.dificil: return 0.30f;
                }
                break;

            case Rareza.Legendaria:
                switch (dificultad)
                {
                    case Dificultad.facil: return 0.10f;
                    case Dificultad.moderado: return 0.30f;
                    case Dificultad.dificil: return 0.50f;
                }
                break;
        }

        return 0f; // Por si acaso explota el universo
    }


    public static Dificultad DificultadAleatoria() // se rompe si añades más dificultades de lo normal (3)
    {
        int NumeroAleatorio = UnityEngine.Random.Range(0, 3);

        switch (NumeroAleatorio)
        {
            case 0:
                return Dificultad.facil;

            case 1:
                return Dificultad.moderado;
            case 2:
                return Dificultad.dificil;
            default:
                Debug.LogError("GRAVEEEEEEE ERROR");
                return Dificultad.facil;

        }


    }

    //protected  void ActualizarDescripcion()
    //{
    //    base.ActualizarDescripcion();
    //    this.descripcion = $"debes lograr {Mathf.FloorToInt(GetCuantoFaltaMeta()).ToString()}";
    //}

    public override float GetMostrarTiempoAproximadoExigido()
    {
        return metaTiempoProductivo;
    }
    public override string GetMostrarIndicadorCompletado()
    {
        if (Completado == false)
        {
            return Mathf.FloorToInt(progresoMeta).ToString() + "/" + Mathf.Ceil(metaTiempoProductivo) + " minutos";
        }
        else
        {
            return "Exigencia completada";
        }



    }

    public override string GetMostrarDescripcionExigencia()
    {
        return descripcion;
    }

    public override float GetValueIndicadorCompletado()
    {
        return progresoMeta / metaTiempoProductivo;
    }

    //public void AumentarProgreso(float valor)
    //{

    //}
}


public class ExigenciaSuperarRecord : Exigencia
{
    public enum TiposRecord { TiempoDiarioPromedio, DuracionSesionPromedio, PomodoroDuracionPromedio, }

    private TiposRecord tiposRecord;


    public ExigenciaSuperarRecord(Dinosaurio exigidor, Dificultad dificultad, TiposRecord tiposRecord) : base(exigidor, dificultad)
    {




        this.descripcion = $"Los caracteres motivadores necesitan ser alegrado por tus logros. Deberás superar un reto personala productivo para emocionarlos.";

    }



}

public interface IMostrarIndicadorCompletado
{
    string GetMostrarIndicadorCompletado();
    float GetValueIndicadorCompletado();
    string GetMostrarDescripcionExigencia();
}
#endregion


