using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Presentacion : MonoBehaviour
{

    [Header("Área de aparición de comida")]
    [SerializeField] private Vector2 centroArea = Vector2.zero;
    [SerializeField] private Vector2 tamañoArea = new Vector2(5f, 5f); // en unidades del mundo

    [SerializeField] private float precio; //en pps
    [SerializeField] private string nombreProducto;
    [SerializeField] private GameObject item;



    public float radioDeDispensa = 5f; // Radio alrededor del botón donde se dispersará la comida
    public float cooldown = 0.5f; // Intervalo de tiempo entre cada comida
    private float tiempoUltimaDispensa = 0f;
    public Button boton; // El botón de UI

    public void ComprarItem()
    {
        if (Gamificacion.Instance.TryConsumirTiempo(precio))
        {
            Debug.Log($"Compraste un {nombreProducto} a {precio}");

            DispensarComidaYaComprada();
        }
        else
        {
            LogicaVentanaConfirmacion.Instance.ShowPopup("Comida: Tiempo insuficiente", $"El precio de {nombreProducto} es de {precio} minutos y no tienes suficiente.", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
        }
    }


    private void Start()
    {
        // Asigna el método que se ejecutará cuando el botón sea presionado
        boton.onClick.AddListener(OnClick);
    }

    // Método que se llama cuando el botón es presionado
    private void OnClick()
    {
      
            // Si ha pasado el intervalo de tiempo, dispense una nueva comida
            if (Time.time - tiempoUltimaDispensa > cooldown)
            {
                ComprarItem();
                tiempoUltimaDispensa = Time.time;
            }
       
    }

    // Método para dispensar comida de forma aleatoria
    private void DispensarComidaYaComprada()
    {
        Vector3 posicionAleatoria = GetRandomPositionInArea();
        Instantiate(item, posicionAleatoria, Quaternion.identity);
    }


    private Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(centroArea.x - tamañoArea.x / 2f, centroArea.x + tamañoArea.x / 2f);
        float y = Random.Range(centroArea.y - tamañoArea.y / 2f, centroArea.y + tamañoArea.y / 2f);
        return new Vector3(x, y, 10f); // Puedes ajustar el Z si es necesario
    }


    // Método para obtener una posición aleatoria dentro de un radio alrededor del botón
    //private Vector3 GetRandomPositionInCentralArea(float anchoArea, float altoArea)
    //{
    //    // Calcular los límites de la zona central
    //    float xMin = (Screen.width - anchoArea) / 2;
    //    float xMax = (Screen.width + anchoArea) / 2;
    //    float yMin = (Screen.height - altoArea) / 2;
    //    float yMax = (Screen.height + altoArea) / 2;

    //    // Generar una posición aleatoria dentro del área central
    //    float x = Random.Range(xMin, xMax);
    //    float y = Random.Range(yMin, yMax);

    //    // Convertir la posición aleatoria al espacio del mundo
    //    Vector3 posicionMundo = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane));

    //    // Devolver la posición en el espacio del mundo, asegurando el valor de Z correcto
    //    return new Vector3(posicionMundo.x, posicionMundo.y, 1f);
    //}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Naranja semi-transparente
        Gizmos.DrawCube(centroArea, tamañoArea);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centroArea, tamañoArea);
    }

}
