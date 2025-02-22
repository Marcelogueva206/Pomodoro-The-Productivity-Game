using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaMarcaMeta : MonoBehaviour
{
    public GameObject BotonReclamarRecompensa;
    public TipoMeta Tipo;

    public void TryReclamarRecompensa()
    {
        SistemaRecompensa.Instancia.IntentoRuletaGanado();
        SistemaRecompensa.Instancia.AbrirVentanaSistemaRecompensa();      
        GestorMetas.Instance.GetMetaPorTipo(Tipo).SetReclamado(true);
        BotonReclamarRecompensa.SetActive(false);

    }
 
    public void IntentarMostrarReclamarRecompensa()
    {
        if(GestorMetas.Instance.GetMetaPorTipo(Tipo).GetReclamado() == false && GestorMetas.Instance.GetMetaPorTipo(Tipo).GetCompletado() == true)
        {
            BotonReclamarRecompensa.SetActive(true);
        }
        else
        {

            BotonReclamarRecompensa.SetActive(false);
        }
        
    }

    private void Start()
    {
        BotonReclamarRecompensa.SetActive(false);
    }
}
