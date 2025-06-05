using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaMarcaMeta : MonoBehaviour
{
    public GameObject BotonReclamarRecompensa;
    public TipoMeta Tipo;
    public Sprite spriteEstrellaCompletada;
    public GameObject fondoEstrella;
    public void TryReclamarRecompensa()
    {
        if(Contador.FaseActual == Contador.FasesContador.Inicio)
        {
            SistemaRecompensa.Instancia.IntentoRuletaGanado();
            SistemaRecompensa.Instancia.AbrirVentanaSistemaRecompensa();
            GestorMetas.Instance.GetMetaPorTipo(Tipo).SetReclamado(true);
            BotonReclamarRecompensa.SetActive(false);
            var sr = fondoEstrella.GetComponent<SpriteRenderer>();
            if (sr != null && spriteEstrellaCompletada != null)
            {
                sr.sprite = spriteEstrellaCompletada;
            }
        }
        else
        {
            Debug.Log("*Sonido indicando que debes esperar para reclamar");
        }
    

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
