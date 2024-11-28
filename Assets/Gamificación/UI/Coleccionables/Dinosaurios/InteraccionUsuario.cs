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


    void Start()
    {
        interactuado = GetComponent<Dinosaurio>();




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

        spriteRenderer = GetComponent<SpriteRenderer>();
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
