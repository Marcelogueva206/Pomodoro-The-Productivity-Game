using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comida : MonoBehaviour
{



    //[SerializeField]private float cantidad;// 500 gramos == 500 pps == 0.1
    //[SerializeField] private float coeficienteSatisfactor; //  1/5000

    [SerializeField] private float satisfacion = 50f;
    [SerializeField] private TiposComidas TiposComida;
    /// <summary>
    /// 6 minutos => 100 pp
    /// 1 hora productiva ==> 1000 pp
    /// </summary>
    // max 5 CM -  5/10 horas
    // promedio 3 CM - 3/6 horas
    // min 1 CM -  1/2 horas

    // facil ==> 1 - 30  minutos promedio (para mantener estado de ánimo) (17 pp - 500 pp)
    // mediano ==> 30 - 60 (para aumentar un poco el estado de ánimo) (500 pp - 1000 pp)
    // dificil ==> 60 - 120 (para aumentar mucho el estado de ánimo) (1000 pp - 2000 pp)


    private bool isDragging = false;  // Para saber si el objeto está siendo arrastrado

    void Update()
    {
        // Si el objeto está siendo arrastrado, actualizamos su posición
        if (isDragging)
        {
            // Convertimos la posición del mouse en coordenadas del mundo 2D
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;  // Asegurarse de que no cambie el eje Z

            // Movemos el objeto a la posición del mouse
            transform.position = mousePosition;
        }
    }

    // Detectar cuando el botón del mouse se mantiene presionado sobre el objeto
    void OnMouseDown()
    {
        isDragging = true;  // Iniciamos el arrastre
    }

    // Detectar cuando se suelta el botón del mouse
    void OnMouseUp()
    {
        isDragging = false;  // Terminamos el arrastre
    }




    //[HideInInspector]private Vector3 diferenciaPosicion;
    //[HideInInspector]private float coordenadaZ;
    //[HideInInspector] private float velocidadArraste = 1137;


    //private void OnMouseDown()
    //{
    //    coordenadaZ = 0;

    //    diferenciaPosicion = transform.position - GetMouseWorldPos();
    //}

    //private void OnMouseDrag()
    //{
    //    transform.position = Vector3.Lerp(transform.position, GetMouseWorldPos() + diferenciaPosicion, Time.deltaTime*velocidadArraste);
    //}

    //private Vector3 GetMouseWorldPos()
    //{
    //    Vector3 posicionMouse = Input.mousePosition;

    //    posicionMouse.z = coordenadaZ;

    //    return Camera.main.ScreenToViewportPoint(posicionMouse);
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Caracter Motivador"))
        {
            Dinosaurio consumidor = collision.gameObject.GetComponent<Dinosaurio>();
            consumidor.AlterarEmocionalidad(satisfacion*(0.1f/50));


            foreach (ExigenciaTiempoProductivo exigencia in consumidor.exigencias)
            {
                if (TiposComida == exigencia.tiposComidaRequerida||exigencia.tiposComidaRequerida == TiposComidas.cualquierTipo)
                {
                    exigencia.ProgresoMeta += satisfacion;
                }
            }


            Destroy(gameObject);

        }
    }

}
