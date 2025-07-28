using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdopcionBoton : MonoBehaviour
{
    // Start is called before the first frame update

    public  void AbrirCofirrmarAdopcion()
    {
        LogicaVentanaConfirmacion.Instance.ShowPopup("Adoptar", "Hay cuatro tipos de rareza y especie. A mayor rareza, mayores son las recompensas que dan al igual que sus exigencias. Si no los cuidas, pueden morir.", () => { AdopcionManager.Instance.ProcesoAdoptarBotón(); },() => { Debug.Log("Cancelado"); });
    }



}
