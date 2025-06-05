using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

[System.Serializable]
public class RegistroProductividad
{
    public string fecha; // Fecha en formato "YYYY-MM-DD"
    public double horas; // Total de horas trabajadas en ese día
}

public class ProductividadManager : MonoBehaviour
{
    private const double LAMBDA = 0.1; // Controla la rapidez con la que baja el promedio
    private List<RegistroProductividad> registros = new List<RegistroProductividad>();
    private string filePath; // Ruta del archivo JSON

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "productividad.json");
        CargarRegistros(); // Cargar datos al iniciar el juego
        MostrarRegistrosEnConsola();
        EstadisticasManager.Instance.ActualizarTextoRangoYPrimedioUI();

        //ReiniciarRegistroProductividad();
    }

    public void RegistrarHoras(double horas)
    {
        string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");

        // Verifica si ya existe un registro para el día actual
        RegistroProductividad registroExistente = registros.FirstOrDefault(r => r.fecha == fechaHoy);

        if (registroExistente != null)
        {
            registroExistente.horas += horas; // Acumular horas en el mismo registro
        }
        else
        {
            registros.Add(new RegistroProductividad { fecha = fechaHoy, horas = horas });
        }

        GuardarRegistros();
    }

    public double CalcularPromedio() // este es el real
    {
        DateTime hoy = DateTime.Now;
        double numerador = 0;
        double denominador = 0;

        foreach (var registro in registros)
        {
            DateTime fechaRegistro = DateTime.Parse(registro.fecha);
            double diasDesdeRegistro = (hoy - fechaRegistro).TotalDays;
            double peso = Math.Exp(-LAMBDA * diasDesdeRegistro);

            numerador += registro.horas * peso;
            denominador += peso;
        }

        return (denominador > 0) ? Math.Round(numerador / denominador, 1) : 0;
    }

    private void GuardarRegistros()
    {
        string json = JsonUtility.ToJson(new RegistroLista { registros = this.registros });
        File.WriteAllText(filePath, json);
    }

    private void CargarRegistros()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            registros = JsonUtility.FromJson<RegistroLista>(json).registros;
        }
    }

    public void MostrarRegistrosEnConsola()
    {
        if (registros.Count == 0)
        {
            Debug.Log("No hay registros de productividad guardados.");
            return;
        }

        Debug.Log("📊 Registros de productividad:");
        foreach (var registro in registros)
        {
            Debug.Log($"📅 Fecha: {registro.fecha} | ⏳ Horas: {registro.horas}");
        }
    }


    public void ModificarHorasDeFecha(string fechaObjetivo, double nuevasHoras)
    {
        RegistroProductividad registroExistente = registros.FirstOrDefault(r => r.fecha == fechaObjetivo);

        if (registroExistente != null)
        {
            registroExistente.horas = nuevasHoras; // Actualizar el valor
            Debug.Log($"✅ Se ha actualizado la productividad del {fechaObjetivo} a {nuevasHoras} horas.");
            GuardarRegistros(); // Guardar cambios en el archivo JSON
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró un registro para la fecha {fechaObjetivo}.");
        }
    }
    public void ReiniciarRegistroProductividad()
    {
        registros.Clear(); // Eliminar todos los registros en memoria

        if (File.Exists(filePath))
        {
            File.Delete(filePath); // Borrar el archivo JSON si existe
        }

        Debug.Log("🔄 Registro de productividad reiniciado. Ahora es como si el usuario ingresara por primera vez.");
    }
}

[System.Serializable]
public class RegistroLista
{
    public List<RegistroProductividad> registros;
}