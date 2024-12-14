using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importar el namespace de TextMeshPro
using UnityEngine.Events;

public class LogicaVentanaConfirmacion : MonoBehaviour
{

    public static LogicaVentanaConfirmacion Instance { get; private set; }
    // Referencias a los elementos de la ventana
    public GameObject popupPanel; // Panel de la ventana emergente
    public TMP_Text titleText; // Texto del título (TextMeshPro)
    public TMP_Text descriptionText; // Texto de la descripción (TextMeshPro)
    public Button confirmButton; // Botón de confirmar
    public Button cancelButton; // Botón de cancelar

    // Eventos para confirmar o cancelar
    private UnityAction onConfirmAction;
    private UnityAction onCancelAction;

    // Método para mostrar la ventana
    public void ShowPopup(string title, string description, UnityAction confirmAction, UnityAction cancelAction)
    {
        // Configurar textos
        titleText.text = title;
        descriptionText.text = description;

        // Asignar acciones a los botones
        onConfirmAction = confirmAction;
        onCancelAction = cancelAction;

        // Configurar los botones
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Cancel);

        // Mostrar la ventana
        popupPanel.SetActive(true);
    }

    // Método para confirmar la acción
    public void Confirm()
    {
        onConfirmAction?.Invoke(); // Ejecutar la acción de confirmación
        HidePopup(); // Ocultar la ventana
    }

    // Método para cancelar la acción
    public void Cancel()
    {
        onCancelAction?.Invoke(); // Ejecutar la acción de cancelación
        HidePopup(); // Ocultar la ventana
    }

    // Método para ocultar la ventana
    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    private void Awake()
    {
        // Configurar el Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garantiza que solo haya una instancia
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Opcional: Mantener entre escenas

        HidePopup();
    }

}