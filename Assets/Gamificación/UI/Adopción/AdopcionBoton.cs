using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdopcionBoton : MonoBehaviour
{
    // Start is called before the first frame update

    public  void AbrirCofirrmarAdopcion()
    {
        LogicaVentanaConfirmacion.Instance.ShowPopup("Estas seguro de querer adoptar un nuevo motivador?", "Al adoptar un nuevo motivador, tus metas diarias seran mas exigentes. Ten en cuenta que puedes perderlo si no le dedica el tiempo necesario. ",() => { AdopcionManager.Instance.ProcesoAdoptarBotón(); },() => { Debug.Log("Cancelado"); });
    }



}
