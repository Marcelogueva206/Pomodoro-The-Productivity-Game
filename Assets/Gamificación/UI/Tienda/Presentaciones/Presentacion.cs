using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Presentacion : MonoBehaviour
{
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
            LogicaVentanaConfirmacion.Instance.ShowPopup("Tiempo insuficiente", $"El precio es de {precio} minutos y no tienes suficiente. Sé productivo y consigue más minutos", () => Debug.Log("advertencia avisada"), () => Debug.Log("advertencia avisada"));
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
        // Obtener una posición aleatoria alrededor del botón
        Vector3 posicionAleatoria = GetRandomPositionInCentralArea(500, 500);

        // Instanciar la comida en esa posición
        Instantiate(item, posicionAleatoria, Quaternion.identity);
    }

    // Método para obtener una posición aleatoria dentro de un radio alrededor del botón
    private Vector3 GetRandomPositionInCentralArea(float anchoArea, float altoArea)
    {
        // Calcular los límites de la zona central
        float xMin = (Screen.width - anchoArea) / 2;
        float xMax = (Screen.width + anchoArea) / 2;
        float yMin = (Screen.height - altoArea) / 2;
        float yMax = (Screen.height + altoArea) / 2;

        // Generar una posición aleatoria dentro del área central
        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);

        // Convertir la posición aleatoria al espacio del mundo
        Vector3 posicionMundo = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane));

        // Devolver la posición en el espacio del mundo, asegurando el valor de Z correcto
        return new Vector3(posicionMundo.x, posicionMundo.y, 1f);
    }




}
