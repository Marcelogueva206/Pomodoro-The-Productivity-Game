using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presentacion : MonoBehaviour
{
    [SerializeField] private float precio; //en pps
    [SerializeField] private string nombreProducto;
    [SerializeField] private GameObject item;







    public void ComprarItem()
    {
        if (Gamificacion.Instance.TryConsumirTiempo(precio))
        {
            Debug.Log($"Compraste un {nombreProducto} a {precio}");

            ObtenerItemYaComprado();
        }
        else
        {
            Debug.Log("No tienes sufiente pps para comprar esto");
        }




    }

    public void ObtenerItemYaComprado()
    {
        Instantiate(item, new Vector3(0,0,0), Quaternion.identity);
    }

}
