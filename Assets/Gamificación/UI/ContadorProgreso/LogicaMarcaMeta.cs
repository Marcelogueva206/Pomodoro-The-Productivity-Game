using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class LogicaMarcaMeta : MonoBehaviour
{
    public GameObject BotonReclamarRecompensa;
    public TipoMeta Tipo;
    public Sprite spriteEstrellaCompletada; 
    public GameObject fondoEstrella;

    //public void ComprarGiro(int precio = 15)
    //{
    //    if (Gamificacion.Instance.TryConsumirTiempo(precio))
    //    {
    //        SistemaRecompensa.Instancia.IntentoRuletaGanado();
    //        SistemaRecompensa.Instancia.AbrirVentanaSistemaRecompensa();

    //    }
    //}
    public void TryReclamarRecompensa()
    {
        if(Contador.FaseActual == Contador.FasesContador.Inicio && GestorMetas.Instance.GetMetaPorTipo(Tipo).GetReclamado()== false)
        {
            int puntuacion = (int)GestorMetas.Instance.GetMetaPorTipo(Tipo).PuntuacionRequerida;

            if (puntuacion > 60)
            {
                // Siempre se da 1 boleto base
                SistemaRecompensa.Instancia.IntentoRuletaGanado();

                // Calculamos minutos extra
                int minutosExtra = puntuacion - 60;

                // Calculamos cuántos bloques de 30 minutos hay en los minutos extra
                int boletosExtra = minutosExtra / 30;

                // Otorgamos los boletos extra
                for (int i = 0; i < boletosExtra; i++)
                {
                    SistemaRecompensa.Instancia.IntentoRuletaGanado();
                }
            }
            else
            {
                // Si es 60 o menos, solo 1 boleto
                SistemaRecompensa.Instancia.IntentoRuletaGanado();
            }

            SistemaRecompensa.Instancia.AbrirVentanaSistemaRecompensa();


            GestorMetas.Instance.GetMetaPorTipo(Tipo).SetReclamado(true);
            //BotonReclamarRecompensa.SetActive(false);
            ContadorProgreso.Instance.GuardarProgreso();
            var img = fondoEstrella.GetComponent<Image>();
            if (img != null && spriteEstrellaCompletada != null)
            {
                img.sprite = spriteEstrellaCompletada;
            }
            else if (img == null)
            {
                UnityEngine.Debug.LogWarning("Image no encontrado en fondoEstrella.");
            }
        }
        else
        {
           

            LogicaVentanaConfirmacion.Instance.ShowPopup("Recompensa ya reclamada", $"Espera a mañana",  () => UnityEngine.Debug.Log("Cancelado"), () => UnityEngine.Debug.Log("Cancelado"));
        }
    

    }
 
    public void IntentarMostrarReclamarRecompensa()
    {
        if(ContadorProgreso.Instance == null || GestorMetas.Instance == null || BotonReclamarRecompensa == null)
        {
            ContadorProgreso.Instance.CargarProgreso();
        }
     

        if (GestorMetas.Instance.GetMetaPorTipo(Tipo).GetCompletado() == true)
        {
            BotonReclamarRecompensa.SetActive(true);
        }
        else
        {
            //LogicaVentanaConfirmacion.Instance.ShowPopup("No se puede reclamar recompensa", $"Ya fue reclamado o no se ha completado la meta. Puedes comprar comprar giro adicional a 15 minutos", () => ComprarGiro(), () => Debug.Log("Cancelado"));
            BotonReclamarRecompensa.SetActive(false);
        }
        
    }

    private void Start()
    {
        IntentarMostrarReclamarRecompensa();
    }
}
