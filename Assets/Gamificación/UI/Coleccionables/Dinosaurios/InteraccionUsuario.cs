using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InteraccionUsuario : MonoBehaviour
{
    [HideInInspector] private Dinosaurio interactuado;

    [Header("Menú")]
    [SerializeField] private GameObject customMenu;
    // Variable para rastrear si el menú está abierto o cerrado
    private bool isMenuOpen = false;

    [Header("Resaltado")]
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 30f); // Color de resaltado (puedes ajustarlo)


    [Header("Estadisticas")]
    [SerializeField] private GameObject ExigenciaUI1;
    [SerializeField] private GameObject ExigenciaUI2;
    [SerializeField] private GameObject ExigenciaUI3;
    [SerializeField] private GameObject EmocionalidadUI;
    [SerializeField] private GameObject MostrarDescripcionUI1;
    [SerializeField] private GameObject MostrarDescripcionUI2;
    [SerializeField] private GameObject MostrarDescripcionUI3;
    [SerializeField] private GameObject MostrarNombre;

    [SerializeField] private GameObject MostrarDescripcionEmocionalidad;
    [SerializeField] private GameObject MostrarRareza;

    public void ActualizarNombre()
    {
        MostrarNombre.GetComponent<TMP_Text>().text = interactuado.Nombre;
    }
    public void ActualizarRareza()
    {
        MostrarRareza.GetComponent<TMP_Text>().text = (GetProbabilidadTotal(interactuado._Especie, interactuado.Rareza) * 100f).ToString("0.#") + "%";
    }

    public float GetProbabilidadTotal(Especie especie, Rareza rareza)
    {
        // Probabilidades de especie
        var especieChances = new Dictionary<Especie, int>
    {
        { Especie.Stegosaurus, 40 },
        { Especie.Espinosaurio, 20 },
        { Especie.Triceratops, 30 },
        { Especie.TiranosaurioRex, 10 }
    };

        // Probabilidades de rareza
        var rarezaChances = new Dictionary<Rareza, int>
    {
        { Rareza.Comun, 50 },
        { Rareza.Rara, 30 },
        { Rareza.SuperRara, 15 },
        { Rareza.Legendaria, 5 }
    };

        if (!especieChances.ContainsKey(especie) || !rarezaChances.ContainsKey(rareza))
            return 0f;

        float probEspecie = especieChances[especie] / 100f;
        float probRareza = rarezaChances[rareza] / 100f;

        // Probabilidad total (independientes)
        return probEspecie * probRareza;
    }
    public void ActualizarMostrarDescripcionExigencias()
    {
        foreach (Exigencia exigencia in interactuado.exigencias)
        {


            if (exigencia.dificultad == Exigencia.Dificultad.facil)
            {
                MostrarDescripcionUI1.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarDescripcionExigencia();
            }
            else

            if (exigencia.dificultad == Exigencia.Dificultad.moderado)
            {
                MostrarDescripcionUI2.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarDescripcionExigencia();
            }
            else
            if (exigencia.dificultad == Exigencia.Dificultad.dificil)
            {
                MostrarDescripcionUI3.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarDescripcionExigencia();
            }
        }
    }

    public void ActualizarMostrarDescripcionEmocionalidad()
    {
        MostrarDescripcionEmocionalidad.GetComponentInChildren<TMP_Text>().text = Mathf.FloorToInt(interactuado.Emocionalidad*100).ToString()+" %";
    }
    public void AbrirDescripcionUI1()
    {
        ActualizarMostrarDescripcionExigencias();
        IntercambiarEstadoActivo(MostrarDescripcionUI1);
    }

    private void IntercambiarEstadoActivo(GameObject objetoAIntercambiarEstado)
    {
        if (objetoAIntercambiarEstado != null)
        {
            // Cambia el estado activo del objeto
            objetoAIntercambiarEstado.SetActive(!objetoAIntercambiarEstado.activeSelf);
        }
    }

    public void AbrirDescripcionUI2()
    {
        ActualizarMostrarDescripcionExigencias();
        IntercambiarEstadoActivo(MostrarDescripcionUI2);
    }
    public void AbrirDescripcionUI3()
    {
        ActualizarMostrarDescripcionExigencias();
        IntercambiarEstadoActivo(MostrarDescripcionUI3);
    }

    public void ActualizarMostrarEmocionalidad()
    {

        EmocionalidadUI.GetComponent<Slider>().value = interactuado.Emocionalidad * (1f / 1.5f);
    }


    public void ActualizarMostrarIndicadorCompletadoExigencias()
    {
        foreach (Exigencia exigencia in interactuado.exigencias)
        {
            
           
            if (exigencia.dificultad == Exigencia.Dificultad.facil)
            {
                ExigenciaUI1.GetComponent<Slider>().value = exigencia.GetValueIndicadorCompletado();
                ExigenciaUI1.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarIndicadorCompletado();
            }
            else

           if (exigencia.dificultad == Exigencia.Dificultad.moderado)
            {
                ExigenciaUI2.GetComponent<Slider>().value = exigencia.GetValueIndicadorCompletado();
                ExigenciaUI2.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarIndicadorCompletado();
            }
            else
           if (exigencia.dificultad == Exigencia.Dificultad.dificil)
            {
                ExigenciaUI3.GetComponent<Slider>().value = exigencia.GetValueIndicadorCompletado();
                ExigenciaUI3.GetComponentInChildren<TMP_Text>().text = exigencia.GetMostrarIndicadorCompletado();
            }
        }


        //ExigenciaTiempoProductivo exigenciaBasica = interactuado.exigencias[0] as ExigenciaTiempoProductivo;

        //ExigenciaBasicaUI.GetComponent<Slider>().value = exigenciaBasica.GetMostrarIndicadorCompletado();

        //ExigenciaBasicaUI.GetComponentInChildren<TMP_Text>().text = (Mathf.FloorToInt(exigenciaBasica.GetMostrarIndicadorCompletado()*100)).ToString()+"%";

    }

    private void Awake()
    {
        interactuado = GetComponent<Dinosaurio>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
       
        ActualizarNombre();
        ActualizarRareza();


        // Asegúrate de que el menú está desactivado al inicio
        if (customMenu != null)
        {
            customMenu.SetActive(false);
        }

        foreach (GameObject MostrarUI in new List<GameObject> { MostrarDescripcionUI1, MostrarDescripcionUI2, MostrarDescripcionUI3 })
        {
            if (MostrarUI != null)
            {
                MostrarUI.SetActive(false);
            }


        }

        
        originalColor = spriteRenderer.color; // Guardar el color original
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
        // Detecta clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Verifica si el GameObject es clickeado


            if (hitCollider != null && hitCollider.transform == transform)
            {
                // Alternar visibilidad del menú
                ToggleMenu();
            }
        }

        if (hitCollider != null && hitCollider.transform == transform)
        {
            // Cambiar al color de resaltado
            spriteRenderer.color = highlightColor;

            // Puedes agregar más lógica aquí si es necesario (como el clic)
            if (Input.GetMouseButtonDown(0))
            {
                // Llama a tu función para abrir el menú aquí
                // ToggleMenu();
            }
        }
        else
        {
            // Restablecer al color original
            spriteRenderer.color = originalColor;
        }

        // Solo actualiza la posición si el menú está activo
        if (!customMenu.activeSelf) return;

        Vector3 worldPos = interactuado.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = customMenu.transform.parent.GetComponent<RectTransform>();
        RectTransform rect = customMenu.GetComponent<RectTransform>();
        float panelWidth = rect.rect.width;
        float panelHeight = rect.rect.height;
        float margin = 100f; // Ajusta según tu preferencia

        // Calcula el centro de la pantalla
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        // Decide la dirección en X
        float targetX = screenPos.x;
        if (screenPos.x < centerX)
            targetX = screenPos.x + panelWidth / 2 + margin; // Mueve a la derecha
        else
            targetX = screenPos.x - panelWidth / 2 - margin; // Mueve a la izquierda

        // Limita en X para que no se salga
        targetX = Mathf.Clamp(targetX, panelWidth / 2, Screen.width - panelWidth / 2);

        // Decide la dirección en Y
        float targetY = screenPos.y;
        if (screenPos.y < centerY)
            targetY = screenPos.y + panelHeight / 2 + margin; // Mueve hacia arriba
        else
            targetY = screenPos.y - panelHeight / 2 - margin; // Mueve hacia abajo

        // Limita en Y para que no se salga
        targetY = Mathf.Clamp(targetY, panelHeight / 2, Screen.height - panelHeight / 2);

        // Convierte a coordenadas locales del canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, new Vector2(targetX, targetY), Camera.main, out localPoint);

        rect.anchoredPosition = localPoint;
    }

    void ToggleMenu()
    {
        // Cambia el estado del menú
        isMenuOpen = !isMenuOpen;
        if (customMenu != null)
        {
            customMenu.SetActive(isMenuOpen);
        }
    }


    public void SetRectTransformValues(RectTransform rectTransform, float posX, float posY)
    {
        // Asignar Pos X y Pos Y
        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }




    //public GameObject menu;  // El menú desplegable
    //public Button openMenuButton;  // El botón que aparece al poner el puntero sobre el personaje

    //private bool isMenuOpen = false;  // Para verificar si el menú está abierto o cerrado
    //private bool isPointerOverCharacter = false;  // Para saber si el puntero está sobre el personaje

    //void Start()
    //{
    //    // Asegúrate de que el menú y el botón no estén visibles al inicio
    //    menu.SetActive(false);
    //    openMenuButton.gameObject.SetActive(false);

    //    // Añadir listener al botón para abrir/cerrar el menú
    //    openMenuButton.onClick.AddListener(ToggleMenu);
    //}

    //void Update()
    //{
    //    // Mostrar el botón si el puntero está sobre el personaje
    //    if (isPointerOverCharacter && !isMenuOpen)
    //    {
    //        openMenuButton.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        openMenuButton.gameObject.SetActive(false);
    //    }
    //}

    //void OnMouseEnter()
    //{
    //    isPointerOverCharacter = true;
    //}

    //// Detectar cuando el puntero sale del área del personaje
    //void OnMouseExit()
    //{
    //    isPointerOverCharacter = false;
    //}

    //// Método para abrir o cerrar el menú
    //void ToggleMenu()
    //{
    //    isMenuOpen = !isMenuOpen;
    //    menu.SetActive(isMenuOpen);
    //}





}
